using System;
using System.IO;
using System.Linq;

namespace ScrGen.Icon
{
    // ICONDIR reader / writer
    sealed class ICODirectory : IconDirectory
    {
        public ICODirectoryEntry[] Entries { get; set; }

        public override int Size
        {
            get
            {
                if (Entries == null)
                    throw new InvalidOperationException("Entries are null");

                return BaseSize + ICODirectoryEntry.Size * Entries.Length;
            }
        }


        private void Validate()
        {
            if (Entries.Length == 0)
                return;

            var orderedEntries = Entries.OrderBy(e => e.ImageOffset).ToArray();
            var firstEntry = orderedEntries.First();
            if (firstEntry.ImageOffset < Size)
                throw new InvalidOperationException("Entry ImageOffset less than Size");

            for (var i = 0; i < orderedEntries.Length - 1; i++)
            {
                if (orderedEntries[i].ImageOffset + orderedEntries[i].BytesInRes > orderedEntries[i + 1].ImageOffset)
                    throw new InvalidOperationException("Entries have invalid ImageOffset and BytesInRes values");
            }
        }

        private void Parse(Stream icoStream)
        {
            Entries = new ICODirectoryEntry[Count];

            for (var i = 0; i < Count; i++)
                Entries[i] = new ICODirectoryEntry(icoStream);

            Validate();
        }

        public ICODirectory(Stream icoStream) :
            base(icoStream)
        {
            if (icoStream.Length < icoStream.Position + ICODirectoryEntry.Size * Count)
                throw new ArgumentOutOfRangeException("Stream contains insufficient data", "icoStream");

            Parse(icoStream);
        }

        public ICODirectory(byte[] icoBytes) :
            base(icoBytes)
        {
            if (icoBytes.Length < BaseSize + ICODirectoryEntry.Size * Count)
                throw new ArgumentOutOfRangeException("Array contains insufficient data", "icoStream");

            using (var icoStream = new MemoryStream(icoBytes, IconDirectory.BaseSize, ICODirectoryEntry.Size * Count))
                Parse(icoStream);
        }

        public ICODirectory(ICODirectory directory) :
            base(directory)
        {
            Entries = directory.Entries.Select(e => new ICODirectoryEntry(e)).ToArray();
        }

        public ICODirectory(IconDirectory directory) :
            base(directory)
        { }

        public ICODirectory()
        {
            Entries = new ICODirectoryEntry[0];
        }


        public override void ToStream(Stream icoStream)
        {
            if (Entries == null || Entries.Any(e => e == null))
                throw new InvalidOperationException("Entries are null");

            if (Count != Entries.Length)
                throw new InvalidOperationException("Count does not equal to Entries count");

            if (Entries.Length == 0)
            {
                base.ToStream(icoStream);
                return;
            }

            Validate();

            base.ToStream(icoStream);

            for (var i = 0; i < Entries.Length; i++)
                Entries[i].ToStream(icoStream);
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
