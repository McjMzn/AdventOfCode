using System;

namespace AdventOfCode
{
    public static class Output
    {
        public static void Part1(object answer)
        {
            WriteInColor(ConsoleColor.Gray, "Part 1: ", ConsoleColor.White, answer.ToString(), ConsoleColor.DarkYellow, " *");
        }

        public static void Part2(object answer)
        {
            WriteInColor(ConsoleColor.Gray, "Part 2: ", ConsoleColor.White, answer.ToString(), ConsoleColor.DarkYellow, " **");
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
