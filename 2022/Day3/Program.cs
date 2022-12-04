namespace AdventOfCode.TwentyTwentyTwo.Day3;

using System;
using System.Collections.Generic;
using System.Linq;

public static class ItemPriority
{
    public static int Get(char itemSymbol)
    {
        return
            itemSymbol >= 'a' ?
            itemSymbol - 'a' + 1 :
            itemSymbol - 'A' + 27;
    }
}

public class Rucksack
{
    public Rucksack(string itemsString)
    {
        var span = itemsString.AsSpan();
        var half = span.Length / 2;

        this.AllItems = span.ToArray();
        this.FirstCompartment = span[0..half].ToArray();
        this.SecondCompartment = span[half..].ToArray();
    }

    public char[] AllItems { get; }
    public char[] FirstCompartment { get; }
    public char[] SecondCompartment { get; }
    public char[] CommonItems => this.AllItems.Distinct().Where(item => this.FirstCompartment.Contains(item) && this.SecondCompartment.Contains(item)).ToArray();
}

public class RucksackGroup
{
    public List<Rucksack> Rucksacks { get; set; } = new();
    public char Badge => this.Rucksacks.SelectMany(r => r.AllItems).Distinct().Single(item => this.Rucksacks.All(r => r.AllItems.Contains(item)));
}

internal class Program
{
    static void Main(string[] args)
    {
        // Part 1
        var part1Answer = 0;
        Input.Process(line =>
        {
            var rucksack = new Rucksack(line);
            part1Answer += rucksack.CommonItems.Select(item => ItemPriority.Get(item)).Sum();
        });

        Console.WriteLine($"Part 1: {part1Answer}");

        // Part 2
        const int groupSize = 3;
        var part2Answer = 0;
        var i = 0;
        var group = new RucksackGroup();

        Input.Process(line =>
        {
            group.Rucksacks.Add(new Rucksack(line));
            if (i++ % groupSize == groupSize - 1)
            {
                part2Answer += ItemPriority.Get(group.Badge);
                group = new RucksackGroup();
            }
        });

        Console.WriteLine($"Part 2: {part2Answer}");
    }
}
