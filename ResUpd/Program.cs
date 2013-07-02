using Common;
using System;
using System.IO;
using System.ComponentModel;

namespace ResUpd
{
    // main logic
    class Program
    {
        private static void ValidateOperationArgs(OperationArgs args)
        {
            // source pe file should exist
            if (!File.Exists(args.SourcePath))
            {
                var message = string.Format(Localization.ValidateArgsSourceFile, args.SourcePath);
                throw new IOException(message);
            }

            // output pe file should not exist
            if (args.SourcePath != args.OutputPath && File.Exists(args.OutputPath))
            {
                var message = string.Format(Localization.ValidateArgsOutputFile, args.OutputPath);
                throw new IOException(message);
            }
        }

        private static void ValidateExtractArgs(ExtractArgs args)
        {
            ValidateOperationArgs(args);
        }

        private static void ValidateInjectArgs(InjectArgs args)
        {
            ValidateOperationArgs(args);

            // embedding resource file should exist
            if (!File.Exists(args.ResourcePath))
            {
                var message = string.Format(Localization.ValidateArgsResourceFile, args.ResourcePath);
                throw new IOException(message);
            }
        }

        private static void ValidateDeleteArgs(DeleteArgs args)
        {
            ValidateOperationArgs(args);
        }

        // selects type-dependent args validation
        private static void ValidateArgs(OperationArgs args)
        {
            if (args is ExtractArgs)
                ValidateExtractArgs((ExtractArgs)args);
            else if (args is InjectArgs)
                ValidateInjectArgs((InjectArgs)args);
            else if (args is DeleteArgs)
                ValidateDeleteArgs((DeleteArgs)args);
        }

        private static IntPtr LoadSource(string sourcePath)
        {
            var sourceHandle = PInvoke.LoadDatafile(sourcePath);
            if (sourceHandle == IntPtr.Zero)
            {
                var ex = PInvoke.GetLastWin32Exception();
                var message = string.Format(Localization.WriteOutputLoadDatafile,
                    sourcePath, ex.Message);
                throw new IOException(message, ex);
            }

            return sourceHandle;
        }

        private static byte[] LoadResource(IntPtr sourceHandle, ushort resourceType, 
            ushort resourceName)
        {
            try
            {
                return PInvoke.LoadResourceBytes(sourceHandle, 
                    resourceType, resourceName);
            }
            catch (Win32Exception ex)
            {
                var message = string.Format(Localization.WriteOutputLoadResourceBytes,
                    ex.Message);
                throw new IOException(message, ex);
            }
        }

        private static void WriteResource(byte[] resourceData, string outputPath)
        {
            try
            {
                File.WriteAllBytes(outputPath, resourceData);
            }
            catch (Exception ex)
            {
                var message = string.Format(Localization.WriteOutputWriteResource,
                    outputPath, ex.Message);
                throw new IOException(message, ex);
            }
        }

        private static byte[] ReadResource(string resourcePath)
        {
            try
            {
                return File.ReadAllBytes(resourcePath);
            }
            catch (Exception ex)
            {
                var message = string.Format(Localization.WriteOutputFileRead,
                    resourcePath, ex.Message);
                throw new IOException(message, ex);
            }
        }

        private static void TryCopySource(string sourcePath, string outputPath)
        {
            if (sourcePath != outputPath)
            {
                try
                {
                    File.Copy(sourcePath, outputPath, false);
                }
                catch (Exception ex)
                {
                    var message = string.Format(Localization.WriteOutputFileCopy,
                        sourcePath, outputPath, ex.Message);
                    throw new IOException(message, ex);
                }
            }
        }

        private static IntPtr BeginUpdateResource(string outputPath)
        {
            var updateHandle = PInvoke.BeginUpdateResource(outputPath);
            if (updateHandle == IntPtr.Zero)
            {
                var ex = PInvoke.GetLastWin32Exception();
                var message = string.Format(Localization.WriteOutputBeginUpdateResource, 
                    outputPath, ex.Message);
                throw new IOException(message, ex);
            }

            return updateHandle;
        }

        private static void UpdateResource(IntPtr updateHandle, ushort resourceType,
            ushort resourceName, byte[] resourceData, string outputPath)
        {
            if (!PInvoke.UpdateResource(updateHandle, resourceType, resourceName, resourceData))
            {
                var ex = PInvoke.GetLastWin32Exception();
                var message = string.Format(Localization.WriteOutputUpdateResource, 
                    outputPath, ex.Message);
                throw new IOException(message, ex);
            }
        }

        private static void DeleteResource(IntPtr updateHandle, ushort resourceType, 
            ushort resourceName, string outputPath)
        {
            if (!PInvoke.DeleteResource(updateHandle, resourceType, resourceName))
            {
                var ex = PInvoke.GetLastWin32Exception();
                var message = string.Format(Localization.WriteOutputDeleteResource, 
                    outputPath, ex.Message);
                throw new IOException(message, ex);
            }
        }

        private static void CompleteUpdateResource(IntPtr updateHandle, string outputPath)
        {
            if (!PInvoke.CompleteUpdateResource(updateHandle))
            {
                var ex = PInvoke.GetLastWin32Exception();
                var message = string.Format(Localization.WriteOutputCompleteUpdateResource, 
                    outputPath, ex.Message);
                throw new IOException(message, ex);
            }
        }

        private static void WriteExtractOutput(ExtractArgs args)
        {
            // load pe
            var sourceHandle = LoadSource(args.SourcePath);

            // load resource data
            var resourceData = LoadResource(sourceHandle, 
                args.ResourceType, args.ResourceName);

            // write resource data
            WriteResource(resourceData, args.OutputPath);
        }

        private static void WriteInjectOutput(InjectArgs args)
        {
            // load resource data
            var resourceData = ReadResource(args.ResourcePath);

            // try to copy pe
            TryCopySource(args.SourcePath, args.OutputPath);

            // update pe
            var updateHandle = BeginUpdateResource(args.OutputPath);

            using (var updateDisposable = new PInvoke.DisposableHandle(updateHandle,
                h => PInvoke.DiscardUpdateResource(updateHandle)))
            {            
                UpdateResource(updateHandle, args.ResourceType,
                    args.ResourceName, resourceData, args.OutputPath);

                CompleteUpdateResource(updateHandle, args.OutputPath);

                updateDisposable.SuppressDispose();
            }
        }

        private static void WriteDeleteOutput(DeleteArgs args)
        {
            // try copy pe
            TryCopySource(args.SourcePath, args.OutputPath);

            // update pe
            var updateHandle = BeginUpdateResource(args.OutputPath);

            using (var updateDisposable = new PInvoke.DisposableHandle(updateHandle,
                h => PInvoke.DiscardUpdateResource(updateHandle)))
            {
                DeleteResource(updateHandle, args.ResourceType,
                    args.ResourceName, args.OutputPath);

                CompleteUpdateResource(updateHandle, args.OutputPath);

                updateDisposable.SuppressDispose();
            }
        }

        // selects args type-dependent write logic
        private static void WriteOutput(OperationArgs args)
        {
            if (args is ExtractArgs)
                WriteExtractOutput((ExtractArgs)args);
            else if (args is InjectArgs)
                WriteInjectOutput((InjectArgs)args);
            else if (args is DeleteArgs)
                WriteDeleteOutput((DeleteArgs)args);
        }

        private static int Main(string[] arguments)
        {
            const int successCode = 0, errorCode = 1;

            // parse args
            var args = StartupArgsHandler.Parse(arguments,
                new StartupArgs[] { new ExtractArgs(), new InjectArgs(), 
                    new DeleteArgs() }) as OperationArgs;

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

            // write result
            Console.WriteLine(Localization.MainWriteOutput);
            try
            { 
                WriteOutput(args); 
            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
                return errorCode;
            }

            Console.WriteLine(Localization.MainSuccess, args.OutputPath);
            return successCode;
        }
    }
}
