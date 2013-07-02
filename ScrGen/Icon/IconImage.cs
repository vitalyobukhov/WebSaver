using System;
using System.IO;
using System.Linq;

namespace ScrGen.Icon
{
    // ICONIMAGE writer / reader
    sealed class IconImage
    {
        // raw data
        public byte[] Data { get; set; }

        public int Size
        {
            get
            {
                if (Data == null)
                    throw new InvalidOperationException("Data is null");

                return sizeof(byte) * Data.Length;
            }
        }


        private void Parse(Stream imageStream, int offset, SeekOrigin origin, int size)
        {
            imageStream.Seek(offset, origin);

            Data = new byte[size];

            imageStream.Read(Data, 0, size);
        }

        public IconImage(Stream imageStream, int offset, SeekOrigin origin, int size)
        {
            if (imageStream == null)
                throw new ArgumentNullException("imageStream");

            if (!(imageStream.CanRead && imageStream.CanSeek))
                throw new ArgumentException("Can't read or seek stream", "imageStream");

            if (size < 0)
                throw new ArgumentOutOfRangeException("size");

            if (origin != SeekOrigin.Current && offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            if (((origin == SeekOrigin.Begin || origin == SeekOrigin.End) && offset > imageStream.Length) ||
                (origin == SeekOrigin.Current && (imageStream.Position + offset < 0 || imageStream.Position + offset > imageStream.Length)))
                throw new ArgumentOutOfRangeException("offset");

            if ((origin == SeekOrigin.Begin && offset + size > imageStream.Length) ||
                (origin == SeekOrigin.End && size > offset) ||
                (origin == SeekOrigin.Current && (offset + size < 0 || offset + size > imageStream.Length)))
                throw new ArgumentOutOfRangeException("size");

            Parse(imageStream, offset, origin, size);
        }

        public IconImage(Stream imageStream)
        {
            if (imageStream == null)
                throw new ArgumentNullException("imageStream");

            if (!(imageStream.CanRead && imageStream.CanSeek))
                throw new ArgumentException("Can't read or seek stream", "imageStream");

            Parse(imageStream, 0, SeekOrigin.Current, 
                (int)(imageStream.Length - imageStream.Position));
        }

        public IconImage(byte[] imageBytes, int offset, int size)
        {
            if (imageBytes == null)
                throw new ArgumentNullException("imageBytes");

            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            if (size < 0 || imageBytes.Length < offset + size)
                throw new ArgumentOutOfRangeException("size");

            using (var imageStream = new MemoryStream(imageBytes, offset, size))
                Parse(imageStream, 0, SeekOrigin.Begin, size);
        }

        public IconImage(byte[] imageBytes)
        {
            if (imageBytes == null)
                throw new ArgumentNullException("imageBytes");

            using (var imageStream = new MemoryStream(imageBytes, 0, imageBytes.Length))
                Parse(imageStream, 0, SeekOrigin.Begin, imageBytes.Length);
        }

        public IconImage(IconImage image)
        {
            if (image == null)
                throw new ArgumentNullException("image");

            if (image.Data == null)
                throw new InvalidOperationException("Image Data is null");

            Data = image.Data.ToArray();
        }

        public IconImage()
        {
            Data = new byte[0];
        }


        public void ToStream(Stream imageStream)
        {
            if (imageStream == null)
                throw new ArgumentNullException("imageStream");

            if (!imageStream.CanWrite)
                throw new ArgumentException("Can't write to stream", "imageStream");

            if (Data == null)
                throw new InvalidOperationException("Data is null");

            imageStream.Write(Data, 0, Data.Length);
        }

        public byte[] ToBytes()
        {
            using (var imageStream = new MemoryStream(Size))
            {
                ToStream(imageStream);
                return imageStream.ToArray();
            }
        }
    }
}
