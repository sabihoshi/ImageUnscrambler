﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Enumeration;

namespace ImageUnscrambler
{
    internal class Program
    {
        private static Point Point(int x, int y)
        {
            return new Point(x, y);
        }

        private static void Main(string[] args)
        {
            while (true)
            {
                var rows = Prompt<int>("Enter rows", int.TryParse);
                var cols = Prompt<int>("Enter columns", int.TryParse);

                var path = Prompt("Enter path");
                if (Directory.Exists(path))
                {
                    var isRecursive = Prompt<bool>("Recursive?", bool.TryParse);
                    var isFoldered = Prompt<bool>("Output folder?", bool.TryParse);

                    var images = GetImagesFromFolder(path, isRecursive);
                    foreach (var image in images) UnscrambleImage(rows, cols, image, isFoldered);
                }
                else if (File.Exists(path))
                    UnscrambleImage(rows, cols, path, false);
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

        private static void UnscrambleImage(int rows, int cols, string path, bool isFoldered)
        {
            var image = Image.FromFile(path);
            image = image
                .SplitImage(rows, cols)
                .InvertAxis()
                .CombineImages(image.Width, image.Height);

            SaveImage(image, path, isFoldered);
        }

        private static IEnumerable<string> GetImagesFromFolder(string path, bool isRecursive = false)
        {
            var options = new EnumerationOptions {RecurseSubdirectories = isRecursive};
            return new FileSystemEnumerable<string>(path, ToImage, options)
            {
                ShouldIncludePredicate = IsImage
            };

            static string ToImage(ref FileSystemEntry entry)
            {
                return entry.ToFullPath();
            }

            static bool IsImage(ref FileSystemEntry entry)
            {
                return entry.FileName.EndsWith(".png") || entry.FileName.EndsWith(".jpg") ||
                       entry.FileName.EndsWith(".jpeg");
            }
        }

        private static string Prompt(string question)
        {
            Console.Write($"{question} > ");
            return Console.ReadLine();
        }

        private static T Prompt<T>(string question, TryParseEnum<T> tryParse)
        {
            while (true)
            {
                if (tryParse(Prompt(question), out var result))
                    return result;
            }
        }

        private delegate bool TryParseEnum<T>(string input, out T result);
    }
}