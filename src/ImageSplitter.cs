using System.Drawing;

namespace ImageUnscrambler
{
    public static class ImageSplitter
    {
        public static Image[][] SplitImage(this Image image, int rows, int? columns = null)
        {
            var col = columns ?? rows;

            var images = new Image[rows][];
            var bmp = new Bitmap(image);

            var width = image.Width / col;
            var height = image.Height / rows;

            for (var y = 0; y < rows; y++)
            {
                images[y] = new Image[col];
                for (var x = 0; x < col; x++)
                {
                    var clone = bmp.Clone(new Rectangle(x * width, y * height, width, height), bmp.PixelFormat);
                    images[y][x] = clone;
                }
            }

            return images;
        }

        public static Image[][] InvertAxis(this Image[][] images)
        {
            var row = images.Length;
            var col = images[0].Length;
            var ret = new Image[row][];

            var x = 0;
            var y = 0;

            for (var i = 0; i < row; i++)
            {
                for (var j = 0; j < col; j++)
                {
                    ret[y] ??= new Image[col];

                    ret[y][x] = images[i][j];

                    y = (y + 1) % images.Length;
                    x = y == 0 ? x + 1 : x;
                }
            }

            return ret;
        }

        public static Image CombineImages(this Image[][] images, int width, int height)
        {
            var bitmap = new Bitmap(width, height);
            using (var g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                for (var y = 0; y < images.Length; y++)
                {
                    for (var x = 0; x < images[y].Length; x++)
                    {
                        g.DrawImage(images[y][x],
                            images[y][x].Width * x,
                            images[y][x].Height * y);
                    }
                }

                g.Save();
            }

            return bitmap;
        }
    }
}