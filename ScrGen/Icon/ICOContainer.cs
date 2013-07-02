using System;
using System.IO;
using System.Linq;

namespace ScrGen.Icon
{
    // ICONDIR and ICONIMAGES container reader / writer
    // http://msdn.microsoft.com/en-us/library/ms997538.aspx
    sealed class ICOContainer : IconContainer
    {
        // related directory
        public ICODirectory Directory { get; set; }

        public override int Size
        {
            get
            {
                if (Directory == null)
                    throw new InvalidOperationException("Directory is null");

                return Directory.Size + ImagesSize;
            }
        }


        private void Parse(Stream icoStream)
        {
            Directory = new ICODirectory(icoStream);

            Images = new IconImage[Directory.Entries.Length];

            for (var i = 0; i < Directory.Entries.Length; i++)
                Images[i] = new IconImage(icoStream, 
                    (int)Directory.Entries[i].ImageOffset, SeekOrigin.Begin,
                    (int)Directory.Entries[i].BytesInRes);
        }

        public ICOContainer(Stream icoStream)
        {
            if (icoStream == null)
                throw new ArgumentNullException("icoStream");

            Parse(icoStream);
        }

        public ICOContainer(byte[] icoBytes)
        {
            if (icoBytes == null)
                throw new ArgumentNullException("icoBytes");

            using (var icoStream = new MemoryStream(icoBytes))
                Parse(icoStream);
        }

        public ICOContainer(string icoFilename)
        {
            if (string.IsNullOrWhiteSpace(icoFilename))
                throw new ArgumentNullException("icoFilename");

            if (!File.Exists(icoFilename))
                throw new ArgumentException("File does not exist", "icoFilename");

            byte[] icoBytes;
            try
            { icoBytes = File.ReadAllBytes(icoFilename); }
            catch (Exception ex)
            { throw new IOException("Unable to read file", ex); }

            using (var icoStream = new MemoryStream(icoBytes))
                Parse(icoStream);
        }

        public ICOContainer(ICOContainer container) :
            base(container)
        {
            if (container.Directory == null)
                throw new InvalidOperationException("Container Directory is null");

            Directory = new ICODirectory(container.Directory);
        }

        public ICOContainer()
        {
            Directory = new ICODirectory();
        }


        private void Validate()
        {
            if (Directory == null)
                throw new InvalidOperationException("Directory is null");

            if (Images == null)
                throw new InvalidOperationException("Images are null");

            if (Directory.Entries.Length != Images.Length)
                throw new InvalidOperationException("Directory Entries count does not equal to Images count");
        }

        // returns ordered by offset entry / image pairs
        private Tuple<ICODirectoryEntry, IconImage>[] GetEntryImageOrderedPairs()
        {
            return Directory.Entries.
                Select((e, i) => new Tuple<ICODirectoryEntry, IconImage>(e, Images[i])).
                OrderBy(t => t.Item1.ImageOffset).ToArray();
        }

        // arrange directory data
        public void Arrange()
        {
            Validate();

            var pairs = GetEntryImageOrderedPairs();

            for (int i = 0, offset = Directory.Size; i < pairs.Length; i++)
            {
                var entry = pairs[i].Item1;
                var image = pairs[i].Item2;

                // set entry offset from current one
                entry.ImageOffset = (uint)offset;

                // set entry size from image size
                entry.BytesInRes = (uint)image.Size;
                offset += image.Size;
            }
        }

        public void ToStream(Stream icoStream, bool arrange = true)
        {
            if (icoStream == null)
                throw new ArgumentNullException("icoStream");

            if (!icoStream.CanWrite)
                throw new ArgumentException("Can't write to stream", "icoStream");

            Validate();

            var pairs = GetEntryImageOrderedPairs();

            if (arrange)
                Arrange();
            else
            {
                if (pairs.Any(t => t.Item1.BytesInRes != (uint)t.Item2.Size))
                    throw new InvalidOperationException("Directory Entries BytesInRes does not equal to Images Data count");
            }

            // write directory
            Directory.ToStream(icoStream);

            // write images
            for (int i = 0, offset = Directory.Size; i < pairs.Length; i++)
            {
                var entry = pairs[i].Item1;
                var image = pairs[i].Item2;

                var zeros = new byte[(int)entry.ImageOffset - offset];
                icoStream.Write(zeros, 0, zeros.Length);

                image.ToStream(icoStream);
                offset = (int)entry.ImageOffset + image.Size;
            }
        }

        public byte[] ToBytes(bool arrange = true)
        {
            using (var icoStream = new MemoryStream())
            {
                ToStream(icoStream,arrange);
                return icoStream.ToArray();
            }
        }

        public void ToFile(string filename, bool overwrite = false, bool arrange = true)
        {
            if (string.IsNullOrWhiteSpace(filename))
                throw new ArgumentNullException("filename");

            if (!overwrite && File.Exists(filename))
                throw new ArgumentException("File already exists", "filename");

            var icoBytes = ToBytes(arrange);
            try
            { 
                File.WriteAllBytes(filename, icoBytes); 
            }
            catch (Exception ex)
            { 
                throw new ArgumentException("Unable to write file", "filename", ex); 
            }
        }
    }
}
