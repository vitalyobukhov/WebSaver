using System;
using System.Linq;

namespace ScrGen.Icon
{
    // icon classes conversion extensions
    static class IconConvertor
    {
        public static PeDirectoryEntry ToPe(this IcoDirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            return new PeDirectoryEntry(entry);
        }

        public static IcoDirectoryEntry ToIco(this PeDirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            return new IcoDirectoryEntry(entry);
        }


        public static PeDirectory ToPe(this IcoDirectory directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            if (directory.Entries == null || directory.Entries.Any(e => e == null))
                throw new InvalidOperationException("Directory Entries are null");

            return new PeDirectory(directory)
            {
                Entries = directory.Entries.Select(e => e.ToPe()).ToArray()
            };
        }

        public static IcoDirectory ToIco(this PeDirectory directory)
        {
            if (directory == null)
                throw new ArgumentNullException("directory");

            if (directory.Entries == null || directory.Entries.Any(e => e == null))
                throw new InvalidOperationException("Directory Entries are null");

            return new IcoDirectory(directory)
            {
                Entries = directory.Entries.Select(e => e.ToIco()).ToArray()
            };
        }


        public static PeContainer ToPe(this IcoContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (container.Directory == null)
                throw new InvalidOperationException("Container Directory is null");

            if (container.Images == null)
                throw new InvalidOperationException("Container Images are null");

            return new PeContainer
            {
                Directory = container.Directory.ToPe(),
                Images = container.Images.Select(i => new IconImage(i)).ToArray()
            };
        }

        public static IcoContainer ToIco(this PeContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (container.Directory == null)
                throw new InvalidOperationException("Container Directory is null");

            if (container.Images == null)
                throw new InvalidOperationException("Container Images are null");

            return new IcoContainer
            {
                Directory = container.Directory.ToIco(),
                Images = container.Images.Select(i => new IconImage(i)).ToArray()
            };
        }
    }
}
