using System;
using System.Linq;

namespace ScrGen.Icon
{
    // DIR and ICONIMAGES container reader / writer base
    abstract class IconContainer
    {
        // related images
        public IconImage[] Images { get; set; }

        protected int ImagesSize
        {
            get 
            {
                if (Images == null)
                    throw new InvalidOperationException("Images are null");

                return Images.Sum(i => i.Size);
            }
        }

        // child container calculated size
        public abstract int Size { get; }


        protected IconContainer(IconContainer container)
        {
            if (container == null)
                throw new ArgumentNullException("container");

            if (container.Images == null)
                throw new InvalidOperationException("Container Images are null");

            Images = Images.Select(i => new IconImage(i)).ToArray();
        }

        protected IconContainer()
        {
            Images = new IconImage[0];
        }
    }
}
