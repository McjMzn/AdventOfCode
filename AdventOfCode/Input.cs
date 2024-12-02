using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class AdventOfCodeRunner
    {
        public static void Run<T>(int demo1Result, int? demo2Result = null)
            where T : IDailyChallenge
        {
            var challenge = Activator.CreateInstance<T>();

            Console.WriteLine($"------ DEMO -----");
            Input.UseDemo();

            var part1Demo = challenge.Part1(Input.LoadLines());
            Output.Part1(part1Demo, demo1Result);

            var part2Demo = challenge.Part2(Input.LoadLines());
            Output.Part2(part2Demo, demo2Result);

            Console.WriteLine();
            Console.WriteLine($"--- CHALLENGE ---");

            Input.UseDefault();
            
            var part1 = challenge.Part1(Input.LoadLines());
            Output.Part1(part1);
            
            var part2 = challenge.Part2(Input.LoadLines());
            Output.Part2(part2);
        }
    }

    public interface IDailyChallenge
    {
        int Part1(IEnumerable<string> inputLines);

        int Part2(IEnumerable<string> inputLines);
    }

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
