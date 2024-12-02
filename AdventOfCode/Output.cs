using System;

namespace AdventOfCode
{
    public static class Output
    {
        public static void Part1(int answer, int? expectedAnswer = null) => WritePrefixed(1, answer, expectedAnswer);

        public static void Part2(int answer, int? expectedAnswer = null) => WritePrefixed(2, answer, expectedAnswer);

        private static void WritePrefixed(int partNumber, int answer, int? expectedAnswer = null)
        {
            if (expectedAnswer is null)
            {
                WriteInColor(ConsoleColor.Gray, $"Part {partNumber}: ", ConsoleColor.White, answer, ConsoleColor.DarkYellow, $" {new string('*', partNumber)}");
                return;
            }

            if (expectedAnswer is not null && answer == expectedAnswer)
            {
                WriteInColor(ConsoleColor.Gray, $"Part {partNumber}: ", ConsoleColor.Green, answer, ConsoleColor.DarkYellow, $" {new string('*', partNumber)}");
                return;
            }

            WriteInColor(ConsoleColor.Gray, $"Part {partNumber}: ", ConsoleColor.Red, answer, ConsoleColor.DarkRed, $" (expected {expectedAnswer})");
        }

        private static void WriteInColor(params object[] toWrite)
        {
            foreach (var x in toWrite)
            {
                switch (x)
                {
                    case ConsoleColor color:
                        Console.ForegroundColor = color;
                        break;

                    default:
                        Console.Write(x.ToString());
                        break;
                }
            }

            Console.ResetColor();
            Console.WriteLine();
        }
    }
}
