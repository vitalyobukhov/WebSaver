using System;
using System.IO;
using System.Text;

namespace ScrGen.Icon
{
    // GRPICONDIRENTRY reader / writer
    sealed class PEDirectoryEntry : IconDirectoryEntry
    {
        // additional header size
        private const int additionalSize = sizeof(ushort);

        // GRPICONDIRENTRY size
        public const int Size = BaseSize + additionalSize;


        // related ICONIMAGE id
        public ushort Id { get; set; }


        private void Parse(Stream peStream)
        {
            using (var reader = new BinaryReader(peStream, Encoding.Default, true))
            {
                Id = reader.ReadUInt16();
            }
        }

        public PEDirectoryEntry(Stream peStream) :
            base(peStream)
        {
            if (peStream.Length < peStream.Position + additionalSize)
                throw new ArgumentOutOfRangeException("Stream contains insufficient data", "peStream");

            Parse(peStream);
        }

        public PEDirectoryEntry(byte[] peBytes) :
            base(peBytes)
        {
            if (peBytes.Length < Size)
                throw new ArgumentException("Array contains insufficient data", "peBytes");

            using (var peStream = new MemoryStream(peBytes, BaseSize, additionalSize))
                Parse(peStream);
        }

        public PEDirectoryEntry(PEDirectoryEntry entry) :
            base(entry)
        {
            Id = entry.Id;
        }

        public PEDirectoryEntry(IconDirectoryEntry entry) :
            base(entry)
        { }

        public PEDirectoryEntry()
        { }


        public override void ToStream(Stream peStream)
        {
            base.ToStream(peStream);

            using (var peWriter = new BinaryWriter(peStream, Encoding.Default, true))
            {
                peWriter.Write(Id);
            }
        }

        public override byte[] ToBytes()
        {
            using (var peStream = new MemoryStream(Size))
            {
                ToStream(peStream);
                return peStream.ToArray();
            }
        }
    }
}
