namespace AdventOfCode.TwentyTwentyTwo.Day11;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

internal class Monkey
{
    public int Id { get; set; }
    public List<Item> Items { get; set; }

    public Func<ulong, ulong> Operation { get; set; }
    
    public ulong Divider { get; set; }
    public int TestFailedThrowDirection { get; set; }
    public int TestSucceededThrowDirection { get; set; }
    
    public ulong InspectionCounter { get; set; } = 0;
    
    public event Action<(Item Item, int Monkey)> ItemThrown;

    public void InspectAllItems()
    {
        var itemsToProcess = this.Items.ToList();

        checked
        {
            foreach (var item in itemsToProcess)
            {
                this.InspectionCounter++;
                item.WorryLevel = this.Operation(item.WorryLevel);
                item.WorryLevel = MonkeyMath.PostProcess(item.WorryLevel);

                var testResult = item.WorryLevel % this.Divider == 0;
                var throwTarget = testResult ? this.TestSucceededThrowDirection : this.TestFailedThrowDirection;

                this.Items.Remove(item);
                this.ItemThrown?.Invoke((item, throwTarget));
            }
        }
    }
}

internal class Item
{
    public Item(ulong worryLevel)
    {
        this.WorryLevel = worryLevel;
    }

    public ulong WorryLevel { get; set; }

    public override string ToString()
    {
        return $"{this.WorryLevel}";
    }
}

internal class MonkeyMath
{
    public static ulong ReliefFactor { get; set; } = 3;
    public static ulong SanityFactor { get; set; } = ulong.MaxValue;

    public static ulong Calculate(ulong input, string operation, string operand)
    {
        checked
        {
            ulong parsedOperand = operand == "old" ? input : ulong.Parse(operand);
            return operation switch
            {
                "+" => input + parsedOperand,
                "-" => input - parsedOperand,
                "*" => input * parsedOperand,
                _ => throw new NotImplementedException()
            };
        }
    }

    public static ulong PostProcess(ulong value)
    {
        value = value % MonkeyMath.SanityFactor;
        value = value / MonkeyMath.ReliefFactor;

        return value;
    }
}

internal static class Program
{
    public static void Main(string[] args)
    {
        // Part 1
        Console.WriteLine($"Part 1: {Run(3, 20)}");

        // Part 2
        Console.WriteLine($"Part 2: {Run(1, 10000)}");
    }

    private static ulong Run(ulong reliefFactor, int numberOfRounds, string inputFile = "input.txt")
    {
        MonkeyMath.ReliefFactor = reliefFactor;
        var monkeys = Load(inputFile);

        for (var i = 0; i < numberOfRounds; i++)
        {
            foreach (var monkey in monkeys)
            {
                monkey.InspectAllItems();
            }
        }

        return monkeys.Select(m => m.InspectionCounter).OrderByDescending(x => x).Take(2).Aggregate((a, b) => a * b);
    }

    private static List<Monkey> Load(string inputFile)
    {
        var monkeys = new List<Monkey>();
        Action<(Item Item, int Monkey)> onThrow = t =>
        {
            var monkey = monkeys.First(m => m.Id == t.Monkey);
            monkey.Items.Add(t.Item);
        };

        MonkeyMath.SanityFactor = 1;
        Input.Process(line =>
        {
            var newMonkeyMatch = Regex.Match(line, @"Monkey (?<id>\d+):");
            if (newMonkeyMatch.Success)
            {
                var id = int.Parse(newMonkeyMatch.Groups["id"].Value);
                var m = new Monkey() { Id = id };
                m.ItemThrown += onThrow;
                monkeys.Add(m);
                return;
            }

            var monkey = monkeys.Last();

            var startingItemsMatch = Regex.Match(line, @"Starting items: (?<items>.*)");
            if (startingItemsMatch.Success)
            {
                var items = startingItemsMatch.Groups["items"].Value.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(v => new Item(ulong.Parse(v))).ToList();
                monkey.Items = items;
                return;
            }

            var operationMatch = Regex.Match(line, @"new = old (?<operation>.) (?<operand>.*)");
            if (operationMatch.Success)
            {
                monkey.Operation = n => MonkeyMath.Calculate(n, operationMatch.Groups["operation"].Value, operationMatch.Groups["operand"].Value);
                return;
            }

            var testMatch = Regex.Match(line, @"Test: divisible by (?<divider>\d+)");
            if (testMatch.Success)
            {
                var divider = ulong.Parse(testMatch.Groups["divider"].Value);
                monkey.Divider = divider;
                MonkeyMath.SanityFactor *= divider;
                return;
            }

            var onTested = Regex.Match(line, @"If (?<result>true|false): throw to monkey (?<id>\d+)");
            if (onTested.Success)
            {
                var id = int.Parse(onTested.Groups["id"].Value);
                if (onTested.Groups["result"].Value == "true")
                {
                    monkey.TestSucceededThrowDirection = id;
                }
                else
                {
                    monkey.TestFailedThrowDirection = id;
                }
            }
        }, inputFile);

        return monkeys;
    }
}