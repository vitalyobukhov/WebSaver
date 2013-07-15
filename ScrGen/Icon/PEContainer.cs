using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Common;
using System.ComponentModel;

namespace ScrGen.Icon
{
    // GRPICONDIR and ICONIMAGES container
    // http://msdn.microsoft.com/en-us/library/ms997538.aspx
    sealed class PeContainer : IconContainer
    {
        // related directory
        public PeDirectory Directory { get; set; }

        public override int Size
        {
            get
            {
                if (Directory == null)
                    throw new InvalidOperationException("Directory is null");

                return Directory.Size + ImagesSize;
            }
        }

        private void ValidateFilename(string filename)
        {
            if (filename == null)
                throw new ArgumentNullException("filename");

            if (!File.Exists(filename))
                throw new ArgumentException("File does not exist", "filename");
        }

        public PeContainer(string filename, ushort groupIconName)
        {
            ValidateFilename(filename);

            // load pe
            var datafile = PInvoke.LoadDatafile(filename);
            if (datafile == IntPtr.Zero)
                throw new Win32Exception("Unable to load datafile", PInvoke.GetLastWin32Exception());

            using (var disposableUpdate = new PInvoke.DisposableHandle(datafile, PInvoke.FreeLibrary))
            {
                // load group icon data
                var groupIconBytes = PInvoke.LoadResourceBytes(datafile,
                    (ushort)PInvoke.ResourceType.GROUP_ICON, groupIconName);

                // parse directory from data
                Directory = new PeDirectory(groupIconBytes);

                Images = new IconImage[Directory.Entries.Length];

                // parse images from data
                for (var i = 0; i < Directory.Entries.Length; i++)
                {
                    var imageBytes = PInvoke.LoadResourceBytes(datafile,
                        (ushort)PInvoke.ResourceType.ICON, Directory.Entries[i].Id);

                    Images[i] = new IconImage(imageBytes);
                }

                disposableUpdate.SuppressDispose();
            }
        }

        public PeContainer(IcoContainer container) :
            base(container)
        {
            if (container.Directory == null)
                throw new InvalidOperationException("Container Directory is null");

            Directory = new PeDirectory(container.Directory);
        }

        public PeContainer()
        {
            Directory = new PeDirectory();
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

        private IntPtr LoadDatafile(string filename)
        {
            var datafile = PInvoke.LoadDatafile(filename);
            if (datafile == IntPtr.Zero)
                throw new Win32Exception("Unable to load datafile", 
                    PInvoke.GetLastWin32Exception());

            return datafile;
        }

        // loads related directory from file
        private PeDirectory GetOldDirectory(string filename, ushort groupIconName)
        {
            var datafile = LoadDatafile(filename);

            using (new PInvoke.DisposableHandle(datafile, PInvoke.FreeLibrary))
            {
                var directoryExists = PInvoke.GetResourceNames(datafile,
                    (ushort)PInvoke.ResourceType.GROUP_ICON).Any(n => n == groupIconName);

                if (directoryExists)
                {
                    var oldDirectoryBytes = PInvoke.LoadResourceBytes(datafile,
                        (ushort)PInvoke.ResourceType.GROUP_ICON, groupIconName);
                    return new PeDirectory(oldDirectoryBytes);
                }

                return null;
            }
        }

        // gets related busy icon names from file
        private ushort[] GetBusyIconNames(string filename, ushort groupIconName)
        {
            var datafile = LoadDatafile(filename);

            ushort[] busyNames;
            PeDirectory oldDirectory = null;
            using (new PInvoke.DisposableHandle(datafile, PInvoke.FreeLibrary))
            {
                busyNames = PInvoke.GetResourceNames(datafile,
                    (ushort)PInvoke.ResourceType.ICON);

                var directoryExists = PInvoke.GetResourceNames(datafile,
                    (ushort)PInvoke.ResourceType.GROUP_ICON).Any(n => n == groupIconName);

                if (directoryExists)
                {
                    var directoryBytes = PInvoke.LoadResourceBytes(datafile,
                        (ushort)PInvoke.ResourceType.GROUP_ICON, groupIconName);
                    oldDirectory = new PeDirectory(directoryBytes);
                }
            }

            if (oldDirectory != null)
            {
                var oldNames = oldDirectory.Entries.Select(e => e.Id);
                busyNames = busyNames.Except(oldNames).ToArray();
            }

            return busyNames;
        }

        // arrange directory data
        public void Arrange(string filename, ushort groupIconName)
        {
            ValidateFilename(filename);
            Validate();

            // get existing names
            var busyIconNames = GetBusyIconNames(filename, groupIconName);

            // get free names
            var ids = PInvoke.GetFreeIconNames(busyIconNames).
                Take(Directory.Entries.Length).ToArray();

            if (ids.Length < Directory.Entries.Length)
                throw new InvalidOperationException("Datafile does not have enough free icon names");

            // set names to directory
            for (var i = 0; i < Directory.Entries.Length; i++)
                Directory.Entries[i].Id = ids[i];
        }

        public void ToFile(string filename, ushort groupIconName, 
            bool arrange = true, bool overwrite = false)
        {
            ValidateFilename(filename);
            Validate();

            var oldDirectory = GetOldDirectory(filename, groupIconName);
            if (overwrite && oldDirectory != null)
                throw new ArgumentException("Group Icon already exists", "groupIconName");

            if (arrange)
                Arrange(filename, groupIconName);
            else
            {
                var indexedEntries = Directory.Entries.
                    Select((e, i) => new KeyValuePair<int, PeDirectoryEntry>(i, e)).ToArray();
                if (indexedEntries.Any(ie1 => indexedEntries.Any(ie2 =>
                    ie1.Key != ie2.Key && ie1.Value.Id == ie2.Value.Id)))
                    throw new InvalidOperationException("Directory Entries contain the same Id");
            }

            var datafile = PInvoke.LoadDatafile(filename);
            if (datafile == IntPtr.Zero)
                throw new Win32Exception("Unable to load datafile", PInvoke.GetLastWin32Exception());

            var busyIconNames = GetBusyIconNames(filename, groupIconName);
            if (!arrange)
            {
                if (busyIconNames.Any(n => Directory.Entries.Any(e => e.Id == n)))
                    throw new InvalidOperationException("Datafile already contains icons with Directory Entries Id");
            }

            var updateHandle = PInvoke.BeginUpdateResource(filename);
            if (updateHandle == IntPtr.Zero)
                throw new Win32Exception("Unable to begin update datafile", PInvoke.GetLastWin32Exception());

            using (var disposableUpdate = new PInvoke.DisposableHandle(updateHandle, PInvoke.DiscardUpdateResource))
            {
                var groupIconBytes = Directory.ToBytes();

                // update group icon
                if (!PInvoke.UpdateResource(updateHandle, (ushort)PInvoke.ResourceType.GROUP_ICON,
                    groupIconName, groupIconBytes))
                    throw new Win32Exception("Unable to update group icon", PInvoke.GetLastWin32Exception());

                // update icons
                for (var i = 0; i < Directory.Entries.Length; i++)
                    if (!PInvoke.UpdateResource(updateHandle, (ushort)PInvoke.ResourceType.ICON,
                        Directory.Entries[i].Id, Images[i].ToBytes()))
                        throw new Win32Exception("Unable to update icon", PInvoke.GetLastWin32Exception());

                // delete icons
                if (oldDirectory != null)
                {
                    var deleteNames = oldDirectory.Entries.Select(e => e.Id).Except(Directory.Entries.Select(e => e.Id)).ToArray();
                    foreach (var deleteName in deleteNames)
                        if (!PInvoke.DeleteResource(updateHandle, (ushort)PInvoke.ResourceType.ICON, deleteName))
                            throw new Win32Exception("Unable to remove icon", PInvoke.GetLastWin32Exception());
                }

                if (!PInvoke.CompleteUpdateResource(updateHandle))
                    throw new Win32Exception("Unable to complete update datafile", PInvoke.GetLastWin32Exception());

                disposableUpdate.SuppressDispose();
            }
        }
    }
}
