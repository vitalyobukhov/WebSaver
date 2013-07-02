using Common;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.ComponentModel;
using ScrGen.Icon;

namespace ScrGen
{
    class Program
    {
        private static void ValidateUpdateArgs(IUpdateArgs args)
        {
            // output screensaver file should not exist
            if (File.Exists(args.OutputPath))
            {
                var message = string.Format(Localization.ValidateArgsOutputFile, args.OutputPath);
                throw new IOException(message);
            }
        }

        private static void ValidateUpdateContentArgs(IUpdateContentArgs args)
        {
            // web content directory should exist
            if (!Directory.Exists(args.ContentPath))
            {
                var message = string.Format(Localization.ValidateArgsSourceDirectory, args.ContentPath);
                throw new IOException(message);
            }

            // main web content file should exist
            var contentPath = Path.Combine(args.ContentPath, Constants.ContentFilename);
            if (!File.Exists(contentPath))
            {
                var message = string.Format(Localization.ValidateArgsContentFile, contentPath);
                throw new IOException(message);
            }
        }

        private static void ValidateUpdateCaptionArgs(IUpdateCaptionArgs args)
        {
            // caption file should exist
            if (!File.Exists(args.CaptionPath))
            {
                var message = string.Format(Localization.ValidateArgsCaptionFile, args.CaptionPath);
                throw new IOException(message);
            }
        }

        private static void ValidateUpdateIconArgs(IUpdateIconArgs args)
        {
            // icon file should exist
            if (!File.Exists(args.IconPath))
            {
                var message = string.Format(Localization.ValidateArgsIconFile, args.IconPath);
                throw new IOException(message);
            }
        }

        // selects type-dependent args validation
        private static void ValidateArgs(IUpdateArgs args)
        {
            ValidateUpdateArgs(args);

            if (args is IUpdateContentArgs)
                ValidateUpdateContentArgs((IUpdateContentArgs)args);

            if (args is IUpdateCaptionArgs)
                ValidateUpdateCaptionArgs((IUpdateCaptionArgs)args);

            if (args is IUpdateIconArgs)
                ValidateUpdateIconArgs((IUpdateIconArgs)args);
        }

        // extracts screensaver module to ouput path
        private static void ExtractModule(string outputPath)
        {
            byte[] module;
            try
            {
                module = PInvoke.LoadResourceBytes(IntPtr.Zero, 
                    Constants.ModuleResourceType, Constants.ModuleResourceName);
            }
            catch(Win32Exception ex)
            {
                var message = string.Format(Localization.ExtractModuleLoadResourceBytes,
                    ex.Message);
                throw new IOException(message, ex);
            }

            try
            {
                File.WriteAllBytes(outputPath, module);
            }
            catch(Exception ex)
            {
                var message = string.Format(Localization.ExtractModuleFileWrite,
                    ex.Message);
                throw new IOException(message, ex);
            }
        }

        // updates file resource using provided resource type, name and data
        private static void InjectResource(string outputPath, ushort resourceType,
            ushort resourceName, byte[] resourceData)
        {
            var updateHandle = PInvoke.BeginUpdateResource(outputPath);
            if (updateHandle == IntPtr.Zero)
            {
                var ex = PInvoke.GetLastWin32Exception();
                var message = string.Format(Localization.InjectResourceBeginUpdateResource, 
                    outputPath, ex.Message);
                throw new IOException(message, ex);
            }

            if (!PInvoke.UpdateResource(updateHandle, resourceType, 
                resourceName, resourceData))
            {
                PInvoke.DiscardUpdateResource(updateHandle);
                var ex = PInvoke.GetLastWin32Exception();
                var message = string.Format(Localization.InjectResourceUpdateResource, 
                    outputPath, ex.Message);
                throw new IOException(message, ex);
            }

            if (!PInvoke.CompleteUpdateResource(updateHandle))
            {
                var ex = PInvoke.GetLastWin32Exception();
                var message = string.Format(Localization.InjectResourceCompleteUpdateResource, 
                    outputPath, ex.Message);
                throw new IOException(message, ex);
            }
        }

        // adds web content to screensaver
        private static void InjectContent(IUpdateContentArgs args)
        {
            string tmpPath;
            try
            {
                tmpPath = Path.GetTempFileName();
            }
            catch (Exception ex)
            {
                var message = string.Format(Localization.InjectContentGetTemp, ex.Message);
                throw new IOException(message);
            }

            try
            {
                // create zip from provided web content via temporary path
                File.Delete(tmpPath);
                ZipFile.CreateFromDirectory(args.ContentPath, tmpPath, CompressionLevel.Optimal, false);

                // remove default caption and icon files from zip
                using (var zip = ZipFile.Open(tmpPath, ZipArchiveMode.Update))
                {
                    var caption = zip.GetEntry(Constants.DefaultCaptionFilename);
                    if (caption != null)
                        caption.Delete();

                    var icon = zip.GetEntry(Constants.DefaultIconFilename);
                    if (icon != null)
                        icon.Delete();
                }
            }
            catch (Exception ex)
            {
                var message = string.Format(Localization.InjectContentCreateArchive, 
                    tmpPath, args.ContentPath, ex.Message);
                throw new IOException(message, ex);
            }

            byte[] content;
            try
            { 
                // read temporary zip
                content = File.ReadAllBytes(tmpPath); 
            }
            catch (Exception ex)
            {
                try
                { File.Delete(tmpPath); }
                catch { }

                var message = string.Format(Localization.InjectContentReadArchive, 
                    tmpPath, ex.Message);
                throw new IOException(message, ex);
            }

            try
            { 
                // remove temporary zip
                File.Delete(tmpPath); 
            }
            catch (Exception ex)
            {
                var message = string.Format(Localization.InjectContentDeleteArchive, 
                    tmpPath, ex.Message);
                throw new IOException(message, ex);
            }

            // add read zip to screensaver
            InjectResource(args.OutputPath, Constants.ContentResourceType,
                Constants.ContentResourceName, content);
        }

        // adds user-friendly caption to screensaver
        private static void InjectCaption(IUpdateCaptionArgs args)
        {
            string caption;
            try
            {
                // read caption from file
                caption = File.ReadAllText(args.CaptionPath, Constants.CaptionEncoding);
            }
            catch (Exception ex)
            {
                var message = string.Format(Localization.InjectCaptionFileRead, 
                    args.CaptionPath, ex.Message);
                throw new IOException(message, ex);
            }

            // convert text to string table data
            var stringTableRow = PInvoke.GetStringTable(new[] { null, caption }).Single();

            // add string table data to screensaver
            InjectResource(args.OutputPath, (ushort)PInvoke.ResourceType.STRING,
                stringTableRow.Key, stringTableRow.Value);
        }

        // adds main icon to screensaver
        private static void InjectIcon(IUpdateIconArgs args)
        {
            byte[] icon;
            try
            {
                // read icon data
                icon = File.ReadAllBytes(args.IconPath);
            }
            catch (Exception ex)
            {
                var message = string.Format(Localization.InjectIconFileRead, 
                    args.IconPath, ex.Message);
                throw new IOException(message, ex);
            }

            ICOContainer ico;
            try
            {
                // parse ico from data
                ico = new ICOContainer(icon);
            }
            catch (Exception ex)
            {
                var message = string.Format(Localization.InjectIconParseICO,
                    args.IconPath, ex.Message);
                throw new IOException(message, ex);
            }

            try
            {
                // convert ico representation to pe
                var pe = ico.ToPE();

                // update pe icon
                pe.ToFile(args.OutputPath, Constants.MainGroupIconName);
            }
            catch (Exception ex)
            {
                var message = string.Format(Localization.InjectIconPEToFile,
                    args.OutputPath, ex.Message);
                throw new IOException(message, ex);
            }
        }

        private static int Main(string[] arguments)
        {
            const int successCode = 0, errorCode = 1;

            // parse args
            var args = StartupArgsHandler.Parse(arguments,
                new StartupArgs[] { }, UpdateArgsResolver.Resolve) as IUpdateArgs;

            if (args == null)
            {
                Console.WriteLine(Localization.Usage);
                return errorCode;
            }

            // validate
            Console.WriteLine(Localization.MainValidateArgs);
            try
            { 
                ValidateArgs(args);
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return errorCode;
            }

            // extract screensaver module 
            Console.WriteLine(Localization.MainExtractModule);
            try
            { 
                ExtractModule(args.OutputPath); 
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return errorCode;
            }

            // add web content to screensaver module
            if (args is IUpdateContentArgs)
            {
                Console.WriteLine(Localization.MainInjectContent);
                try
                {
                    InjectContent((IUpdateContentArgs)args);
                }
                catch (IOException ex)
                {
                    try { File.Delete(args.OutputPath); }
                    catch { }

                    Console.WriteLine(ex.Message);
                    return errorCode;
                }
            }

            // add user-friendly caption to screensaver module
            if (args is IUpdateCaptionArgs)
            {
                Console.WriteLine(Localization.MainInjectCaption);
                try
                {
                    InjectCaption((IUpdateCaptionArgs)args);
                }
                catch (IOException ex)
                {
                    try { File.Delete(args.OutputPath); }
                    catch { }

                    Console.WriteLine(ex.Message);
                    return errorCode;
                }
            }

            // add main icon to screensaver module
            if (args is IUpdateIconArgs)
            {
                Console.WriteLine(Localization.MainInjectIcon);
                try
                {
                    InjectIcon((IUpdateIconArgs)args);
                }
                catch (IOException ex)
                {
                    try { File.Delete(args.OutputPath); }
                    catch { }

                    Console.WriteLine(ex.Message);
                    return errorCode;
                }
            }

            Console.WriteLine(Localization.MainSuccess, args.OutputPath);
            return successCode;
        }
    }
}
