using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AdventOfCode
{
    public class AdventOfCodeRunner
    {
        public static bool Verbose { get; set; }

        public static void Run<T>(int demo1Result, int? demo2Result = null, bool runDemo = true, bool verbose = false)
            where T : IDailyChallenge
        {
            Verbose = verbose;

            var challenge = Activator.CreateInstance<T>();
            var sw = new Stopwatch();
            if (runDemo)
            {
                Console.WriteLine($"------ DEMO -----");
                Input.UseDemo();

                sw.Restart();
                var part1Demo = challenge.Part1(Input.LoadLines());
                sw.Stop();
                Output.Part1(part1Demo, sw.Elapsed, demo1Result);

                sw.Restart();
                var part2Demo = challenge.Part2(Input.LoadLines());
                sw.Stop();
                Output.Part2(part2Demo, sw.Elapsed, demo2Result);

                Console.WriteLine();
            }

            Console.WriteLine($"--- CHALLENGE ---");

            Input.UseDefault();

            sw.Restart();
            var part1 = challenge.Part1(Input.LoadLines());
            sw.Stop();
            Output.Part1(part1, sw.Elapsed);

            sw.Restart();
            var part2 = challenge.Part2(Input.LoadLines());
            Output.Part2(part2, sw.Elapsed);
        }

        public static void WriteLine(params object[] toWrite)
        {
            if (Verbose)
            {
                Output.WriteLineInColor(toWrite);
            }
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
