namespace AdventOfCode.TwentyTwentyTwo.Day12;

using AdventOfCode;
using AdventOfCode.Grids;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public class Terrain
{
    public Terrain(char c)
    {
        switch (c)
        {
            case 'S': this.IsStart = true; this.Height = 1; break;
            case 'E': this.IsEnd = true; this.Height = 'z' - 'a' + 1; break;
            default: this.Height = c - 'a' + 1; break;
        }
    }

    public int Height { get; set; }
    public bool IsStart { get; set; }
    public bool IsEnd { get; set; }

    public int TravelCost { get; set; } = int.MaxValue;
    public bool IsVisited { get; set; } = false;

    public override string ToString()
    {
        return $"{this.TravelCost,2} ";
    }
}

internal class Program
{
    private static ListGrid<Terrain> LoadMap(string input)
    {
        var map = new ListGrid<Terrain>();
        var lines = input.Split(Environment.NewLine);
        map.Height = lines.Count();
        map.Width = lines[0].Count();

        lines.SelectMany(x => x).Select(c => new Terrain(c)).ToList().ForEach(n => map.Add(n));
        map.Nodes.Single(n => n.IsStart).TravelCost = 0;

        return map;
    }

    private static int FindShortestPath(ListGrid<Terrain> map)
    {
        var neighbours =
            map
            .Nodes
            .ToDictionary(
                node => node,
                node => map.GetDirectNeighbours(node)
            );

        var endNode = map.Nodes.Single(n => n.IsEnd);
        map.Nodes.Single(n => n.IsStart).TravelCost = 0;

        while (!endNode.IsVisited)
        {
            var unvisited = map.Nodes.Where(n => !n.IsVisited);
            var current = unvisited.Where(n => n.TravelCost != int.MaxValue).OrderBy(n => n.TravelCost).FirstOrDefault();
            if (current is null)
            {
                return int.MaxValue;
            }

            foreach(var neighbour in neighbours[current])
            {
                if (neighbour.Height <= current.Height || current.Height + 1== neighbour.Height)
                {
                    neighbour.TravelCost = neighbour.TravelCost == int.MaxValue || current.TravelCost + 1 < neighbour.TravelCost ? current.TravelCost + 1 : neighbour.TravelCost;
                }
            }

            current.IsVisited = true;
        }

        return endNode.TravelCost;
    }

    private static void Main(string[] args)
    {
        // Demo
        Input.UseDemo();
        var demoInput = Input.Load();
        var demoMap = LoadMap(demoInput);
        var demoResult = FindShortestPath(demoMap);
        Console.WriteLine($"Demo: {demoResult} (should be 31).");

        // Part 1
        Input.UseDefault();
        var part1Input = Input.Load();
        var part1Map = LoadMap(part1Input);
        var part1Result = FindShortestPath(part1Map);
        Console.WriteLine($"Part 1: {part1Result}");

        // Part 2
        var part2Input = Input.Load().Replace("S", "a");
        var indices = Regex.Matches(part2Input, "a").ToList().Select(m => m.Index).ToList();
        var results = new List<int>();
        Parallel.ForEach(indices, new ParallelOptions { MaxDegreeOfParallelism = 8 }, index =>
        {
            var input = new StringBuilder(part2Input);
            input[index] = 'S';
            var map = LoadMap(input.ToString());
            var result = FindShortestPath(map);
            results.Add(result);
        });

        Console.WriteLine($"Part 2: {results.Where(a => a > 0).OrderBy(a => a).First()}");
    }
}