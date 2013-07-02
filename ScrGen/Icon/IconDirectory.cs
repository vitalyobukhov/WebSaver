using System;
using System.IO;
using System.Text;

namespace ScrGen.Icon
{
    // DIR reader / writer base
    abstract class IconDirectory
    {
        // base DIR size
        public const int BaseSize = 6;

        // child DIR calculated size
        public abstract int Size { get; }


        // DIR headers
        public ushort Reserved { get; set; }
        public ushort Type { get; set; }

        // DIRENTRY count
        public ushort Count { get; set; }


        private void Parse(Stream iconStream)
        {
            if (iconStream.Length < iconStream.Position + BaseSize)
                throw new ArgumentOutOfRangeException("Stream contains insufficient data", "iconStream");

            using (var reader = new BinaryReader(iconStream, Encoding.Default, true))
            {
                Reserved = reader.ReadUInt16();
                Type = reader.ReadUInt16();
                Count = reader.ReadUInt16();
            }
        }

        protected IconDirectory(Stream iconStream)
        {
            if (iconStream == null)
                throw new ArgumentNullException("iconStream");

            if (!(iconStream.CanRead && iconStream.CanSeek))
                throw new ArgumentException("Can't read or seek stream", "iconStream");

            Parse(iconStream);
        }

        protected IconDirectory(byte[] iconBytes)
        {
            if (iconBytes == null)
                throw new ArgumentNullException("iconBytes");

            using (var iconStream = new MemoryStream(iconBytes))
                Parse(iconStream);
        }

        protected IconDirectory(IconDirectory directory)
        {
            Reserved = directory.Reserved;
            Type = directory.Type;
            Count = directory.Count;
        }

        protected IconDirectory()
        { }


        public virtual void ToStream(Stream iconStream)
        {
            if (iconStream == null)
                throw new ArgumentNullException("iconStream");

            if (!iconStream.CanWrite)
                throw new ArgumentException("Can't write to stream", "iconStream");

            using (var iconWriter = new BinaryWriter(iconStream, Encoding.Default, true))
            {
                iconWriter.Write(Reserved);
                iconWriter.Write(Type);
                iconWriter.Write(Count);
            }
        }

        public virtual byte[] ToBytes()
        {
            using (var iconStream = new MemoryStream(BaseSize))
            {
                ToStream(iconStream);
                return iconStream.ToArray();
            }
        }
    }
}
