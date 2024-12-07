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

        public static void Run<T>(object demo1Result, object demo2Result = null, bool runDemo = true, bool verbose = false)
            where T : IDailyChallenge
        {
            Verbose = verbose;

            var challenge = Activator.CreateInstance<T>();

            var part1Method = typeof(T).GetMethod(nameof(IDailyChallenge<T>.Part1));
            var part2Method = typeof(T).GetMethod(nameof(IDailyChallenge<T>.Part2));

            var sw = new Stopwatch();
            if (runDemo)
            {
                Console.WriteLine($"------ DEMO -----");
                Input.UseDemo();

                sw.Restart();
                var part1Demo = part1Method.Invoke(challenge, [Input.LoadLines()]);
                sw.Stop();
                Output.Part1(part1Demo, sw.Elapsed, demo1Result);

                sw.Restart();
                var part2Demo = part2Method.Invoke(challenge, [Input.LoadLines()]);
                sw.Stop();
                Output.Part2(part2Demo, sw.Elapsed, demo2Result);

                Console.WriteLine();
            }

            Console.WriteLine($"--- CHALLENGE ---");

            Input.UseDefault();

            sw.Restart();
            var part1 = part1Method.Invoke(challenge, [Input.LoadLines()]);
            sw.Stop();
            Output.Part1(part1, sw.Elapsed);

            sw.Restart();
            var part2 = part2Method.Invoke(challenge, [Input.LoadLines()]);
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
    }

    public interface IDailyChallenge<T> : IDailyChallenge
    {
        T Part1(IEnumerable<string> inputLines);

        T Part2(IEnumerable<string> inputLines);
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
