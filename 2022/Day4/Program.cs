namespace AdventOfCode.TwentyTwentyTwo.Day4;

using System;
using System.Linq;

internal class SectionsRange
{
    public SectionsRange(string rangeString)
    {
        var ids = rangeString.Split('-');
        this.Start = int.Parse(ids[0]);
        this.End = int.Parse(ids[1]);
    }

    public int Start { get; set; }
    public int End { get; set; }

    public bool Contains(SectionsRange otherRange) => otherRange.Start >= this.Start && otherRange.End <= this.End;
    public bool Overlaps(SectionsRange otherRange)
    {
        if (this.Start <= otherRange.Start)
        {
            return this.End >= otherRange.Start;
        }

        return this.Start <= otherRange.End;
    }
}

internal class Program
{
    private static void Main(string[] args)
    {
        // Part 1
        var part1Answer = 0;

        Input.Process(line =>
        {
            var ranges = line.Split(',').Select(rangeString => new SectionsRange(rangeString)).ToList();
            if (ranges[0].Contains(ranges[1]) || ranges[1].Contains(ranges[0]))
            {
                part1Answer++;
            }
        });

        Console.WriteLine($"Part 1: {part1Answer}");

        // Part 2
        var part2Answer = 0;

        Input.Process(line =>
        {
            var ranges = line.Split(',').Select(rangeString => new SectionsRange(rangeString)).ToList();
            if (ranges[0].Overlaps(ranges[1]))
            {
                part2Answer++;
            }
        });

        Console.WriteLine($"Part 2: {part2Answer}");
    }
}