using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace AdventOfCode
{
    public class Input
    {
        public static string FilePath { get; set; } = "input.txt";

        public static void UseDemo() => FilePath = "demo_input.txt";

        public static void UseDefault() => FilePath = "input.txt";

        public static void Process(Action<string> onLineRead)
        {
            var lines = File.ReadAllLines(FilePath);
            foreach(var line in lines)
            {
                onLineRead(line);
            }
        }

        public static string Load()
        {
            return File.ReadAllText(FilePath);
        }

        public static List<string> LoadLines()
        {
            return File.ReadLines(FilePath).ToList();
        }

        public static(int Height, int Width) GetSize()
        {
            var height = 0;
            var width = 0;
            Process(line =>
            {
                height++;
                width = Math.Max(width, line.Length);
            });

            return (height, width);
        }
    }
}
