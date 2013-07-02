using System;
using System.Linq;

namespace ScrGen.Icon
{
    // icon classes conversion extensions
    static class IconConvertor
    {
        public static PEDirectoryEntry ToPE(this ICODirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            return new PEDirectoryEntry(entry);
        }

        public static ICODirectoryEntry ToICO(this PEDirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            return new ICODirectoryEntry(entry);
        }

        public static PEDirectory ToPE(this ICODirectory directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            if (directory.Entries == null || directory.Entries.Any(e => e == null))
                throw new InvalidOperationException("Directory Entries are null");

            var result = new PEDirectory(directory);

            result.Entries = new PEDirectoryEntry[directory.Entries.Length];

            for (var i = 0; i < directory.Entries.Length; i++)
                result.Entries[i] = directory.Entries[i].ToPE();

            return result;
        }

        public static ICODirectory ToICO(this PEDirectory directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            if (directory.Entries == null || directory.Entries.Any(e => e == null))
                throw new InvalidOperationException("Directory Entries are null");

            var result = new ICODirectory(directory);

            result.Entries = new ICODirectoryEntry[directory.Entries.Length];

            for (var i = 0; i < directory.Entries.Length; i++)
                result.Entries[i] = directory.Entries[i].ToICO();

            return result;
        }

        public static PEContainer ToPE(this ICOContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (container.Directory == null)
                throw new InvalidOperationException("Container Directory is null");

            if (container.Images == null)
                throw new InvalidOperationException("Container Images are null");

            return new PEContainer
            {
                Directory = container.Directory.ToPE(),
                Images = container.Images.Select(i => new IconImage(i)).ToArray()
            };
        }

        public static ICOContainer ToICO(this PEContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (container.Directory == null)
                throw new InvalidOperationException("Container Directory is null");

            if (container.Images == null)
                throw new InvalidOperationException("Container Images are null");

            return new ICOContainer
            {
                Directory = container.Directory.ToICO(),
                Images = container.Images.Select(i => new IconImage(i)).ToArray()
            };
        }
    }
}
