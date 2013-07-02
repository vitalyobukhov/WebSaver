using System;
using System.IO;
using System.Linq;

namespace ScrGen.Icon
{
    // GRPICONDIRENTRIES
    sealed class PEDirectory : IconDirectory
    {
        public PEDirectoryEntry[] Entries { get; set; }

        public override int Size
        {
            get
            {
                if (Entries == null)
                    throw new InvalidOperationException("Entries are null");

                return BaseSize + PEDirectoryEntry.Size * Entries.Length;
            }
        }



        private void Parse(Stream peStream)
        {
            Entries = new PEDirectoryEntry[Count];

            for (var i = 0; i < Count; i++)
                Entries[i] = new PEDirectoryEntry(peStream);
        }

        public PEDirectory(Stream peStream) :
            base(peStream)
        {
            if (peStream.Length < peStream.Position + PEDirectoryEntry.Size * Count)
                throw new ArgumentOutOfRangeException("Stream contains insufficient data", "peStream");

            Parse(peStream);
        }

        public PEDirectory(byte[] peBytes) :
            base(peBytes)
        {
            if (peBytes.Length < BaseSize + PEDirectoryEntry.Size * Count)
                throw new ArgumentOutOfRangeException("Array contains insufficient data", "peBytes");

            using (var peStream = new MemoryStream(peBytes, IconDirectory.BaseSize, PEDirectoryEntry.Size * Count))
                Parse(peStream);
        }

        public PEDirectory(PEDirectory directory) :
            base(directory)
        {
            Entries = directory.Entries.Select(e => new PEDirectoryEntry(e)).ToArray();
        }

        public PEDirectory(IconDirectory directory) :
            base(directory)
        { }

        public PEDirectory()
        {
            Entries = new PEDirectoryEntry[0];
        }


        public override void ToStream(Stream peStream)
        {
            if (Entries == null || Entries.Any(e => e == null))
                throw new InvalidOperationException("Entries are null");

            if (Count != Entries.Length)
                throw new InvalidOperationException("Count does not equal to Entries count");

            base.ToStream(peStream);

            for (var i = 0; i < Entries.Length; i++)
                Entries[i].ToStream(peStream);
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
