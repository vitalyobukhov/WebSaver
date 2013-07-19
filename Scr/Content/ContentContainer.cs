using Common;
using System;
using System.IO;
using System.IO.Compression;
using System.Runtime.InteropServices;

namespace Scr.Content
{
    // disposable wrapper for temporary web content extraction
    class ContentContainer : IDisposable
    {
        private bool disposed;


        // path to temporary web content folder
        public string ContentPath { get; private set; }


        private ContentContainer(string contentPath)
        {
            if (string.IsNullOrWhiteSpace(contentPath))
                throw new CreateContentException("Content path is empty");

            if (!Directory.Exists(contentPath))
                throw new CreateContentException("Content path does not exist");

            ContentPath = contentPath;
        }

        ~ContentContainer()
        {
            Dispose(false);
        }


        // load web content from current screensaver module
        private static byte[] LoadContentBytes()
        {
            var self = IntPtr.Zero;

            var foundResource = PInvoke.FindResource(self, Constants.ContentResourceName, Constants.ContentResourceType);
            if (foundResource == IntPtr.Zero)
                throw new LoadContentBytesException(LoadContentBytesException.Details.FindResource,
                    "Unable to find content resource", PInvoke.GetLastWin32Exception());

            var resourceSize = (int)PInvoke.SizeofResource(self, foundResource);
            if (resourceSize == 0)
                throw new LoadContentBytesException(LoadContentBytesException.Details.SizeofResource,
                    "Unable to get size of content resource", PInvoke.GetLastWin32Exception());

            var loadedResource = PInvoke.LoadResource(self, foundResource);
            if (loadedResource == IntPtr.Zero)
                throw new LoadContentBytesException(LoadContentBytesException.Details.LoadResource,
                    "Unable to load content resource", PInvoke.GetLastWin32Exception());

            var lockedResource = PInvoke.LockResource(loadedResource);
            if (lockedResource == IntPtr.Zero)
                throw new LoadContentBytesException(LoadContentBytesException.Details.LockResource,
                    "Unable to lock content resource", PInvoke.GetLastWin32Exception());

            try
            {
                var contentBytes = new byte[resourceSize];

                Marshal.Copy(lockedResource, contentBytes, 0, resourceSize);

                return contentBytes;
            }
            catch (Exception ex)
            {
                throw new LoadContentBytesException(LoadContentBytesException.Details.CopyBytes,
                    "Unable to copy content bytes", ex);
            }
        }

        // parse web content as zip
        private static ZipArchive LoadContentArchive(byte[] contentBytes)
        {
            if (contentBytes == null || contentBytes.Length == 0)
                throw new LoadContentArchiveException(LoadContentArchiveException.Details.Bytes,
                    "Content bytes are empty");

            MemoryStream contentStream;
            try
            { 
                contentStream = new MemoryStream(contentBytes, false); 
            }
            catch (Exception ex)
            {
                throw new LoadContentArchiveException(LoadContentArchiveException.Details.Stream,
                    "Unable to create content stream from bytes", ex);
            }

            ZipArchive contentArchive;
            try
            { 
                contentArchive = new ZipArchive(contentStream); 
            }
            catch (Exception ex)
            {
                try
                {
                    contentStream.Dispose();
                }
                catch { }

                throw new LoadContentArchiveException(LoadContentArchiveException.Details.Archive,
                    "Unable to load content archive from stream", ex);
            }

            return contentArchive;
        }

        // extract zip to temp directory
        private static string ExtractContentArchive(ZipArchive contentArchive)
        {
            if (contentArchive == null)
                throw new ExtractContentArchiveException(ExtractContentArchiveException.Details.Archive,
                    "Content archive is empty");

            string tempPath;
            try
            { 
                tempPath = Path.GetTempPath(); 
            }
            catch (Exception ex)
            {
                throw new ExtractContentArchiveException(ExtractContentArchiveException.Details.Path,
                    "Unable to get temporary directory path", ex);
            }

            var contentDirectory = Guid.NewGuid().ToString("N");
            var contentPath = Path.Combine(tempPath, contentDirectory);

            try
            {
                if (Directory.Exists(contentPath))
                    Directory.Delete(contentPath, true);

                Directory.CreateDirectory(contentPath);
            }
            catch (Exception ex)
            {
                throw new ExtractContentArchiveException(ExtractContentArchiveException.Details.Directory,
                    "Unable to handle content directory", ex);
            }

            try
            { 
                contentArchive.ExtractToDirectory(contentPath); 
            }
            catch (Exception ex)
            {
                throw new ExtractContentArchiveException(ExtractContentArchiveException.Details.Extract,
                    "Unable to extract content archive to directory", ex);
            }

            return contentPath;
        }

        private void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    try
                    {
                        if (!string.IsNullOrWhiteSpace(ContentPath) && Directory.Exists(ContentPath))
                            Directory.Delete(ContentPath, true);
                    }
                    catch { }
                }

                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        // extract web content and create disposable wrapper
        public static ContentContainer Load()
        {
            var contentBytes = LoadContentBytes();
            var contentArchive = LoadContentArchive(contentBytes);
            var contentPath = ExtractContentArchive(contentArchive);
            return new ContentContainer(contentPath);
        }
    }
}
