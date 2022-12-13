namespace AdventOfCode.TwentyTwentyTwo.Day13;

using System;
using System.Linq;
using System.Collections.Generic;
using System.Text.Json;
using AdventOfCode;

public enum CompareStatus
{
    Valid,
    Invalid,
    Undefined,
    Illegal
}

public class Packet
{
    public bool IsDivider { get; set; }
    public List<object> Data { get; set; } = new List<object>();
    public static Packet From(string text, bool isDivier = false)
    {
        var jsonData = JsonSerializer.Deserialize<List<JsonElement>>(text);
        var data = ConvertToNonJsonTypes(jsonData);
        return new Packet() { Data = data, IsDivider = isDivier };
    }

    private static List<object> ConvertToNonJsonTypes(List<JsonElement> data)
    {
        var newData = new List<object>();

        foreach (JsonElement item in data)
        {
            switch (item)
            {
                case { ValueKind: JsonValueKind.Number }:
                    newData.Add(item.GetInt32());
                    break;

                case { ValueKind: JsonValueKind.Array }:
                    newData.Add(ConvertToNonJsonTypes(item.EnumerateArray().ToList()));
                    break;
            }
        }

        return newData;
    }
}

internal class PacketComparer : Comparer<Packet>
{
    public override int Compare(Packet x, Packet y)
    {
        var result = Compare(x.Data, y.Data);
        return result switch
        {
            CompareStatus.Valid => -1,
            CompareStatus.Undefined => 0,
            CompareStatus.Invalid => 1,
        };
    }

    private static CompareStatus Compare(List<object> left, List<object> right)
    {
        var steps = Math.Max(left.Count, right.Count);

        for (var i = 0; i < steps; i++)
        {
            // Left list run out of items.
            if (i == left.Count)
            {
                return CompareStatus.Valid;
            }

            // Right list run out of items.
            if (i == right.Count)
            {
                return CompareStatus.Invalid;
            }

            // Both items are integers.
            var result = CompareAsIntegers(left[i], right[i]);
            if (result is CompareStatus.Illegal)
            {
                var leftList = left[i] is List<object> ? left[i] as List<object> : new List<object> { left[i] };
                var rightList = right[i] is List<object> ? right[i] as List<object> : new List<object> { right[i] };

                result = Compare(leftList, rightList);
            }

            if (result is not CompareStatus.Undefined and not CompareStatus.Illegal)
            {
                return result;
            }
        }

        return CompareStatus.Undefined;
    }

    private static CompareStatus CompareAsIntegers(object left, object right)
    {
        if (left is not int leftInt || right is not int rightInt)
        {
            return CompareStatus.Illegal;
        }

        return
            leftInt < rightInt ? CompareStatus.Valid :
            leftInt > rightInt ? CompareStatus.Invalid :
            CompareStatus.Undefined;
    }
}

internal class Program
{
    static void Main(string[] args)
    {
        var input = Input.Load();
        var comparer = new PacketComparer();
        var packets = new List<Packet>();

        // Part 1
        var part1Result = 0;
        var pairIndex = 1;
        input.Split(Environment.NewLine).ToList().ForEach(line =>
        {
            if (string.IsNullOrEmpty(line))
            {
                packets.Clear();
                pairIndex++;
                return;
            }

            packets.Add(Packet.From(line));
            if (packets.Count == 2)
            {
                var valid = comparer.Compare(packets[0], packets[1]) < 0;
                part1Result = valid ? part1Result + pairIndex: part1Result;
            }
        });

        Console.WriteLine($"Part 1: {part1Result}");

        // Part 2
        packets = input.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Select(line => Packet.From(line)).ToList();
        packets.Add(Packet.From("[[2]]", true));
        packets.Add(Packet.From("[[6]]", true));
        packets.Sort(new PacketComparer());
        var part2Result = packets.Where(p => p.IsDivider).Select(p => packets.IndexOf(p) + 1).ToList().Aggregate((a, b) => a * b);
     
        Console.WriteLine($"Part 2: {part2Result}");
    }
}