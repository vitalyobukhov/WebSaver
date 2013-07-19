using System;
using System.IO;
using System.Text;

namespace ScrGen.Icon
{
    // DIRENTRY reader / writer base
    abstract class IconDirectoryEntry
    {
        // base DIRENTRY size
        public const int BaseSize = sizeof(byte) * 4 + sizeof(ushort) * 2 + sizeof(uint);


        // DIRENTRY headers
        public byte Width { get; set; }
        public byte Height { get; set; }
        public byte ColorCount { get; set; }
        public byte Reserved { get; set; }
        public ushort Planes { get; set; }
        public ushort BitCount { get; set; }

        // related ICONIMAGE data size
        public uint BytesInRes { get; set; }


        private void Parse(Stream iconStream)
        {
            if (iconStream.Length < iconStream.Position + BaseSize)
                throw new ArgumentOutOfRangeException("iconStream", "Stream contains insufficient data");

            using (var reader = new BinaryReader(iconStream, Encoding.Default, true))
            {
                Width = reader.ReadByte();
                Height = reader.ReadByte();
                ColorCount = reader.ReadByte();
                Reserved = reader.ReadByte();
                Planes = reader.ReadUInt16();
                BitCount = reader.ReadUInt16();
                BytesInRes = reader.ReadUInt32();
            }
        }

        protected IconDirectoryEntry(Stream iconStream)
        {
            if (iconStream == null)
                throw new ArgumentNullException("iconStream");

            if (!(iconStream.CanRead && iconStream.CanSeek))
                throw new ArgumentException("Can't read or seek stream", "iconStream");

            Parse(iconStream);
        }

        protected IconDirectoryEntry(byte[] iconBytes)
        {
            if (iconBytes == null)
                throw new ArgumentNullException("iconBytes");

            using (var iconStream = new MemoryStream(iconBytes))
                Parse(iconStream);
        }

        protected IconDirectoryEntry(IconDirectoryEntry entry)
        {
            if (entry == null)
                throw new ArgumentNullException("entry");

            Width = entry.Width;
            Height = entry.Height;
            ColorCount = entry.ColorCount;
            Reserved = entry.Reserved;
            Planes = entry.Planes;
            BitCount = entry.BitCount;
            BytesInRes = entry.BytesInRes;
        }

        protected IconDirectoryEntry()
        { }


        public virtual void ToStream(Stream iconStream)
        {
            if (iconStream == null)
                throw new ArgumentNullException("iconStream");

            if (!iconStream.CanWrite)
                throw new ArgumentException("Can't write to stream", "iconStream");

            using (var iconWriter = new BinaryWriter(iconStream, Encoding.Default, true))
            {
                iconWriter.Write(Width);
                iconWriter.Write(Height);
                iconWriter.Write(ColorCount);
                iconWriter.Write(Reserved);
                iconWriter.Write(Planes);
                iconWriter.Write(BitCount);
                iconWriter.Write(BytesInRes);
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
