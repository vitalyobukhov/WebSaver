using System;
using System.IO;
using System.Linq;

namespace ScrGen.Icon
{
    // ICONDIR reader / writer
    sealed class IcoDirectory : IconDirectory
    {
        public IcoDirectoryEntry[] Entries { get; set; }

        public override int Size
        {
            get
            {
                if (Entries == null)
                    throw new InvalidOperationException("Entries are null");

                return BaseSize + IcoDirectoryEntry.Size * Entries.Length;
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
            Entries = new IcoDirectoryEntry[Count];

            for (var i = 0; i < Count; i++)
                Entries[i] = new IcoDirectoryEntry(icoStream);

            Validate();
        }

        public IcoDirectory(Stream icoStream) :
            base(icoStream)
        {
            if (icoStream.Length < icoStream.Position + IcoDirectoryEntry.Size * Count)
                throw new ArgumentOutOfRangeException("Stream contains insufficient data", "icoStream");

            Parse(icoStream);
        }

        public IcoDirectory(byte[] icoBytes) :
            base(icoBytes)
        {
            if (icoBytes.Length < BaseSize + IcoDirectoryEntry.Size * Count)
                throw new ArgumentOutOfRangeException("Array contains insufficient data", "icoStream");

            using (var icoStream = new MemoryStream(icoBytes, BaseSize, IcoDirectoryEntry.Size * Count))
                Parse(icoStream);
        }

        public IcoDirectory(IcoDirectory directory) :
            base(directory)
        {
            Entries = directory.Entries.Select(e => new IcoDirectoryEntry(e)).ToArray();
        }

        public IcoDirectory(IconDirectory directory) :
            base(directory)
        { }

        public IcoDirectory()
        {
            Entries = new IcoDirectoryEntry[0];
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

            foreach (var e in Entries)
                e.ToStream(icoStream);
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
