using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace AdventOfCode
{
    public interface IDailyChallenge
    {
    }

    public interface IDailyChallenge<T> : IDailyChallenge
    {
        T Part1(IEnumerable<string> inputLines);

        T Part2(IEnumerable<string> inputLines);
    }

    public class AdventOfCodeRunner
    {
        public static bool Verbose { get; set; }

        public static bool RunningDemo { get; set; }

        public static void Run<T>(object demo1Result, object demo2Result = null, bool runDemo = true, bool verbose = false)
            where T : IDailyChallenge
        {
            Verbose = verbose;

            var challenge = Activator.CreateInstance<T>();

            var part1Method = typeof(T).GetMethod(nameof(IDailyChallenge<object>.Part1));
            var part2Method = typeof(T).GetMethod(nameof(IDailyChallenge<object>.Part2));

            var sw = new Stopwatch();
            if (runDemo)
            {
                RunningDemo = true;

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
            RunningDemo = false;

            Input.UseDefault();

            sw.Restart();
            var part1 = part1Method.Invoke(challenge, [Input.LoadLines()]);
            sw.Stop();
            Output.Part1(part1, sw.Elapsed);

            sw.Restart();
            var part2 = part2Method.Invoke(challenge, [Input.LoadLines()]);
            Output.Part2(part2, sw.Elapsed);
        }


        public static void Write(params object[] toWrite)
        {
            if (Verbose)
            {
                Output.WriteInColor(toWrite);
            }
        }

        public static void WriteLine(params object[] toWrite)
        {
            if (Verbose)
            {
                Output.WriteLineInColor(toWrite);
            }
        }
    }
}
