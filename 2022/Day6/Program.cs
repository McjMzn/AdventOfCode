using System;
using System.Linq;

namespace AdventOfCode.TwentyTwentyTwo.Day6;

internal class Program
{
    static void Main(string[] args)
    {
        var input = Input.Load();

        // Part 1
        Console.WriteLine($"Part 1: {FindEndOfMarker(input, 4)}");

        // Part 2
        Console.WriteLine($"Part 2: {FindEndOfMarker(input, 14)}");
    }

    private static int FindEndOfMarker(string input, int markerLength)
    {
        for (var i = 0; i < input.Length - markerLength; i++)
        {
            if (input.Substring(i, markerLength).ToHashSet().Count == markerLength)
            {
                return i + markerLength;
            }
        }

        throw new Exception("Marker not found.");
    }
}
