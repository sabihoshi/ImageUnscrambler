using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Enumeration;
using ImageUnscrambler.Ignored;

namespace ImageUnscrambler
{
    internal class Program
    {
        private static Point Point(int x, int y) => new Point(x, y);

        private static void Main(string[] args)
        {
            while (true)
            {
                var rows = "Enter rows".Prompt<int>(int.TryParse);
                var cols = "Enter columns".Prompt<int>(int.TryParse);

                var ignores = new List<IgnoreChunkData>();
                var hasIgnore = "Ignore part of image?".Prompt<bool>(bool.TryParse);
                if (hasIgnore)
                {
                    Console.WriteLine("Take note that this operation is done in order.");
                    while (true)
                    {
                        if (IgnoreChunkData.TryParse(out var ignore)) ignores.Add(ignore);

                        if ("Stop?".Prompt<bool>(bool.TryParse))
                            break;
                    }
                }

                var path = "Enter path".Prompt();
                if (Directory.Exists(path))
                {
                    var isRecursive = "Recursive?".Prompt<bool>(bool.TryParse);
                    var isFoldered = "Output folder?".Prompt<bool>(bool.TryParse);

                    var images = GetImagesFromFolder(path, isRecursive);
                    foreach (var image in images)
                    {
                        UnscrambleImage(rows, cols, image, isFoldered, ignores);
                    }
                }
                else if (File.Exists(path))
                    UnscrambleImage(rows, cols, path, false, ignores);
                else
                    Console.WriteLine("No image was found.");
            }
        }

        private static void SaveImage(Image result, string path, bool isFoldered)
        {
            if (isFoldered)
            {
                var dir = Path.Combine(Path.GetDirectoryName(path)!, "output");
                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);
                path = Path.Combine(dir, Path.GetFileNameWithoutExtension(path) + ".png");
            }
            else
            {
                path = Path.Combine(
                    Path.GetDirectoryName(path)!,
                    Path.GetFileNameWithoutExtension(path) +
                    "_reversed.png");
            }

            result.Save(path, ImageFormat.Png);
        }

        private static void UnscrambleImage(int rows, int cols, string path, bool isFoldered,
            IEnumerable<IgnoreChunkData> ignores)
        {
            var image = Image.FromFile(path);

            var ignoredChunks = new List<IgnoredChunk>();
            foreach (var ignore in ignores)
            {
                image = image.RemoveChunk(ignore.Position, ignore.Pixels, out var removedChunk);
                ignoredChunks.Add(new IgnoredChunk(removedChunk, ignore.Position));
            }

            image = image
                .SplitImage(rows, cols)
                .InvertAxis()
                .CombineImages(image.Width, image.Height)
                .CombineParts(ignoredChunks);

            SaveImage(image, path, isFoldered);
        }

        private static IEnumerable<string> GetImagesFromFolder(string path, bool isRecursive = false)
        {
            var options = new EnumerationOptions {RecurseSubdirectories = isRecursive};
            return new FileSystemEnumerable<string>(path, ToImage, options)
            {
                ShouldIncludePredicate = IsImage
            };

            static string ToImage(ref FileSystemEntry entry) => entry.ToFullPath();

            static bool IsImage(ref FileSystemEntry entry) =>
                entry.FileName.EndsWith(".png") || entry.FileName.EndsWith(".jpg") ||
                entry.FileName.EndsWith(".jpeg");
        }
    }
}