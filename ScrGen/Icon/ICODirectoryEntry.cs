using System;
using System.IO;
using System.Text;

namespace ScrGen.Icon
{
    // ICONDIRENTRY reander / writer
    sealed class IcoDirectoryEntry : IconDirectoryEntry
    {
        // additional header size
        private const int additionalSize = sizeof(uint);

        // ICONDIRENTRY size
        public const int Size = BaseSize + additionalSize;


        // related ICONIMAGE offset
        public uint ImageOffset { get; set; }


        private void Parse(Stream icoStream)
        {
            using (var reader = new BinaryReader(icoStream, Encoding.Default, true))
            {
                ImageOffset = reader.ReadUInt32();
            }
        }

        public IcoDirectoryEntry(Stream icoStream) :
            base(icoStream)
        {
            if (icoStream.Length < icoStream.Position + additionalSize)
                throw new ArgumentOutOfRangeException("icoStream", "Stream contains insufficient data");

            Parse(icoStream);
        }

        public IcoDirectoryEntry(byte[] icoBytes) :
            base(icoBytes)
        {
            if (icoBytes.Length < Size)
                throw new ArgumentException("Array contains insufficient data", "icoBytes");

            using (var icoStream = new MemoryStream(icoBytes, BaseSize, additionalSize))
                Parse(icoStream);
        }

        public IcoDirectoryEntry(IcoDirectoryEntry entry) :
            base(entry)
        {
            ImageOffset = entry.ImageOffset;
        }

        public IcoDirectoryEntry(IconDirectoryEntry entry) :
            base(entry)
        { }

        public IcoDirectoryEntry()
        { }



        public override void ToStream(Stream icoStream)
        {
            base.ToStream(icoStream);

            using (var icoWriter = new BinaryWriter(icoStream, Encoding.Default, true))
            {
                icoWriter.Write(ImageOffset);
            }
        }

        public override byte[] ToBytes()
        {
            using (var icoStream = new MemoryStream(Size))
            {
                ToStream(icoStream);
                return icoStream.ToArray();
            }
        }
    }
}
