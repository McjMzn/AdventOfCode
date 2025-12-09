using System;

namespace AdventOfCode
{
    public static class Output
    {
        public static void Part1(object answer, TimeSpan elapsed, object? expectedAnswer = null) => WritePrefixed(1, answer, elapsed, expectedAnswer);

        public static void Part2(object answer, TimeSpan elapsed, object? expectedAnswer = null) => WritePrefixed(2, answer, elapsed, expectedAnswer);

        private static void WritePrefixed(int partNumber, object answer, TimeSpan elapsed, object? expectedAnswer = null)
        {
            if (expectedAnswer is null)
            {
                WriteLineInColor(
                    ConsoleColor.Gray, $"Part {partNumber}: ",
                    ConsoleColor.White, answer,
                    ConsoleColor.DarkCyan, $" ({elapsed.ToString(@"ss\.ffff")}s)",
                    ConsoleColor.DarkYellow, expectedAnswer is not null ? $" {new string('*', partNumber)}" : string.Empty
                );
                return;
            }

            if (expectedAnswer is not null && (answer.Equals(expectedAnswer) || answer.ToString() == expectedAnswer.ToString()))
            {
                WriteLineInColor(
                    ConsoleColor.Gray, $"Part {partNumber}: ",
                    ConsoleColor.Green, answer,
                    ConsoleColor.DarkCyan, $" ({elapsed.ToString(@"ss\.ffff")}s)",
                    ConsoleColor.DarkYellow, expectedAnswer is not null ? $" {new string('*', partNumber)}" : string.Empty
                );
                return;
            }

            WriteLineInColor(ConsoleColor.Gray, $"Part {partNumber}: ", ConsoleColor.Red, answer, ConsoleColor.DarkRed, $" (expected {expectedAnswer})");
        }


        public static void WriteInColor(params object[] toWrite)
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
        }

        public static void WriteLineInColor(params object[] toWrite)
        {
            WriteInColor(toWrite);
            Console.WriteLine();
        }
    }
}
