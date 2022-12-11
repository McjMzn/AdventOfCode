
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;

namespace AdventOfCode.TwentyTwentyTwo.Day11;

internal class Monkey
{
    public event Action<(Item Item, int Monkey)> ItemThrown;

    public int Id { get; set; }
    public List<Item> Items { get; set; }
    public Func<BigInteger, BigInteger> Operation { get; set; }
    public Predicate<BigInteger> Test { get; set; }
    public int TestFailedThrowDirection { get; set; }
    public int TestSucceededThrowDirection { get; set; }

    public int InspectionCounter { get; private set; } = 0;

    public void InspectAllItems()
    {
        var itemsToProcess = this.Items.ToList();

        foreach(var item in itemsToProcess)
        {
            this.InspectionCounter++;
            item.WorryLevel = this.Operation(item.WorryLevel);
            
            // Part 1
            // item.WorryLevel = item.WorryLevel / 3;
            
            // Part 2
            item.WorryLevel = item.WorryLevel;

            var throwTarget = this.Test(item.WorryLevel) ? this.TestSucceededThrowDirection : this.TestFailedThrowDirection;
            this.Items.Remove(item);
            this.ItemThrown?.Invoke((item, throwTarget));
        }
    }
}

internal class Item
{
    public Item(ulong worryLevel)
    {
        this.WorryLevel = worryLevel;
    }

    public BigInteger WorryLevel { get; set; }

    public override string ToString()
    {
        return $"{this.WorryLevel}";
    }
}

internal static class Program
{
    public static void Main(string[] args)
    {
        const int numberOfRounds = 10000;
        var monkeys = Load();

        for(var i = 0; i < numberOfRounds; i++)
        {
            foreach(var monkey in monkeys)
            {
                monkey.InspectAllItems();
            }
        }

        // Part 1
        var monkeyBusiness = monkeys.Select(m => m.InspectionCounter).OrderByDescending(x => x).Take(2).Aggregate((a, b) => a * b);
        Console.WriteLine($"Part 1: {monkeyBusiness}");
    }

    private static List<Monkey> Load()
    {
        var monkeys = new List<Monkey>();
        Action<(Item Item, int Monkey)> onThrow = t =>
        {
            var monkey = monkeys.First(m => m.Id == t.Monkey);
            monkey.Items.Add(t.Item);
        };

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

            var operationMatch = Regex.Match(line, @"new = old (?<operation>.) (?<value>.*)");
            if (operationMatch.Success)
            {
                if (operationMatch.Groups["value"].Value == "old")
                {
                    monkey.Operation = operationMatch.Groups["operation"].Value switch
                    {
                        "+" => n => n + n,
                        "-" => n => n - n,
                        "*" => n => n * n,
                        "/" => n => n / n,
                        _ => throw new NotImplementedException()
                    };

                    return;
                }

                var value = ulong.Parse(operationMatch.Groups["value"].Value);
                monkey.Operation = operationMatch.Groups["operation"].Value switch
                {
                    "+" => n => n + value,
                    "-" => n => n - value,
                    "*" => n => n * value,
                    "/" => n => n / value,
                    _ => throw new NotImplementedException()
                };

                return;
            }

            var testMatch = Regex.Match(line, @"Test: divisible by (?<value>\d+)");
            if (testMatch.Success)
            {
                var value = ulong.Parse(testMatch.Groups["value"].Value);
                monkey.Test = n => n % value == 0;
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
        });

        return monkeys;
    }
}