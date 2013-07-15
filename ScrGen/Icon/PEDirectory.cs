using System;
using System.IO;
using System.Linq;

namespace ScrGen.Icon
{
    // GRPICONDIRENTRIES
    sealed class PeDirectory : IconDirectory
    {
        public PeDirectoryEntry[] Entries { get; set; }

        public override int Size
        {
            get
            {
                if (Entries == null)
                    throw new InvalidOperationException("Entries are null");

                return BaseSize + PeDirectoryEntry.Size * Entries.Length;
            }
        }



        private void Parse(Stream peStream)
        {
            Entries = new PeDirectoryEntry[Count];

            for (var i = 0; i < Count; i++)
                Entries[i] = new PeDirectoryEntry(peStream);
        }

        public PeDirectory(Stream peStream) :
            base(peStream)
        {
            if (peStream.Length < peStream.Position + PeDirectoryEntry.Size * Count)
                throw new ArgumentOutOfRangeException("Stream contains insufficient data", "peStream");

            Parse(peStream);
        }

        public PeDirectory(byte[] peBytes) :
            base(peBytes)
        {
            if (peBytes.Length < BaseSize + PeDirectoryEntry.Size * Count)
                throw new ArgumentOutOfRangeException("Array contains insufficient data", "peBytes");

            using (var peStream = new MemoryStream(peBytes, BaseSize, PeDirectoryEntry.Size * Count))
                Parse(peStream);
        }

        public PeDirectory(PeDirectory directory) :
            base(directory)
        {
            Entries = directory.Entries.Select(e => new PeDirectoryEntry(e)).ToArray();
        }

        public PeDirectory(IconDirectory directory) :
            base(directory)
        { }

        public PeDirectory()
        {
            Entries = new PeDirectoryEntry[0];
        }


        public override void ToStream(Stream peStream)
        {
            if (Entries == null || Entries.Any(e => e == null))
                throw new InvalidOperationException("Entries are null");

            if (Count != Entries.Length)
                throw new InvalidOperationException("Count does not equal to Entries count");

            base.ToStream(peStream);

            foreach (var e in Entries)
                e.ToStream(peStream);
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
