﻿using System;
using System.IO;

namespace AdventOfCode
{
    public class Input
    {
        public static void Process(Action<string> onLineRead, string filePath = "input.txt")
        {
            var lines = File.ReadAllLines(filePath);
            foreach(var line in lines)
            {
                onLineRead(line);
            }
        }

        public static string Load(string filePath = "input.txt")
        {
            return File.ReadAllText(filePath);
        }
    }
}
