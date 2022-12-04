using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.TwentyTwentyTwo.Day1
{
    public class Elf
    {
        public List<int> Food { get; set; } = new();
        public int TotalCalories => this.Food.Sum();
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var elves = new List<Elf>();
            var elf = new Elf();

            Input.Process(line =>
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    elves.Add(elf);
                    elf = new Elf();
                }
                else
                {
                    var calories = int.Parse(line);
                    elf.Food.Add(calories);
                }
            });

            // Part 1
            var part1 = GetCaloriesOfLeaders(elves, 1);
            Console.WriteLine($"Part 1: {part1}");

            // Part 2
            var part2 = GetCaloriesOfLeaders(elves, 3);
            Console.WriteLine($"Part 2: {part2}");
        }

        private static int GetCaloriesOfLeaders(IEnumerable<Elf> elves, int numberOfLeaders)
        {
            return
                elves
                    .OrderByDescending(e => e.TotalCalories)
                    .Take(numberOfLeaders)
                    .Select(e => e.TotalCalories)
                    .Sum();
        }
    }
}