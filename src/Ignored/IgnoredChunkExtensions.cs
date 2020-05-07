using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace ImageUnscrambler.Ignored
{
    public static class IgnoredChunkExtensions
    {
        public static Image RemoveChunk(this Image image, Position position, int pixels, out Image removedChunk)
        {
            int retX = 0, retY = 0, retWidth = image.Width, retHeight = image.Height;
            int removedX = 0, removedY = 0, removedWidth = image.Width, removedHeight = image.Height;

            switch (position)
            {
                case Position.Top:
                case Position.Bottom:
                    retHeight -= pixels;
                    removedHeight = pixels;
                    break;

                case Position.Left:
                case Position.Right:
                    retWidth -= pixels;
                    removedWidth = pixels;
                    break;
            }

            switch (position)
            {
                case Position.Top:
                    retY = pixels;
                    break;
                case Position.Bottom:
                    removedY = image.Height - pixels;
                    break;
                case Position.Left:
                    retX = pixels;
                    break;
                case Position.Right:
                    removedX = image.Width - pixels;
                    break;
            }

            var bmp = new Bitmap(image);

            removedChunk = bmp.Clone(new Rectangle(removedX, removedY, removedWidth, removedHeight), bmp.PixelFormat);
            return bmp.Clone(new Rectangle(retX, retY, retWidth, retHeight), bmp.PixelFormat);
        }

        public static Image CombinePart(this Image image, IgnoredChunk chunk)
        {
            var width = image.Width;
            var height = image.Height;

            switch (chunk.Position)
            {
                case Position.Top:
                case Position.Bottom:
                    height += chunk.Image.Height;
                    break;
                case Position.Left:
                case Position.Right:
                    width += chunk.Image.Width;
                    break;
            }

            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                switch (chunk.Position)
                {
                    case Position.Top:
                        g.DrawImage(chunk.Image, 0, 0);
                        g.DrawImage(image, 0, chunk.Image.Height);
                        break;
                    case Position.Bottom:
                        g.DrawImage(image, 0, 0);
                        g.DrawImage(chunk.Image, 0, image.Height);
                        break;
                    case Position.Left:
                        g.DrawImage(chunk.Image, 0, 0);
                        g.DrawImage(image, chunk.Image.Width, 0);
                        break;
                    case Position.Right:
                        g.DrawImage(image, 0, 0);
                        g.DrawImage(chunk.Image, image.Width, 0);
                        break;
                }

                g.Save();
            }

            return bitmap;
        }

        public static Image CombineParts(this Image image, IEnumerable<IgnoredChunk> chunks) =>
            chunks.Aggregate(image, (current, chunk) => current.CombinePart(chunk));
    }
}