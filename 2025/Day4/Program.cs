using AdventOfCode;
using AdventOfCode.Grids;

namespace Day4
{
    internal class Node(char symbol)
    {
        public bool IsPaper { get; set; } = symbol == '@';

        public bool IsAccessible { get; set; }

        public override string ToString()
        {
            if (IsAccessible)
            {
                return "x";
            }

            if (IsPaper)
            {
                return "@";
            }

            return ".";
        }
    }

    internal class Day4_2025 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var grid = LoadGrid(inputLines);
            foreach (var node in grid.Nodes)
            {
                var position = grid.GetIndices(node);
                var neighbours = grid.GetNeighbours(position.Y, position.X, AllowedDirections.All).Where(n => n is not null);
                node.IsAccessible = node.IsPaper && neighbours.Count(n => n.IsPaper) < 4;
            }

            return grid.Nodes.Count(n => n.IsAccessible);
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var result = 0L;
            var grid = LoadGrid(inputLines);

            var removedInThisIteration = 0;
            do
            {
                removedInThisIteration = 0;

                foreach (var node in grid.Nodes)
                {
                    var position = grid.GetIndices(node);
                    var neighbours = grid.GetNeighbours(position.Y, position.X, AllowedDirections.All).Where(n => n is not null);
                    if (node.IsPaper && neighbours.Count(n => n.IsPaper) < 4)
                    {
                        node.IsPaper = false;
                        removedInThisIteration++;
                        result++;
                    }
                }

            }
            while (removedInThisIteration != 0);
            
            return result;
        }

        private ListGrid<Node> LoadGrid(IEnumerable<string> inputLines)
        {
            var grid = new ListGrid<Node>();
            grid.Width = inputLines.First().Length;
            grid.Height = inputLines.Count();

            foreach (var line in inputLines)
            {
                line.ToCharArray().Select(c => new Node(c)).ToList().ForEach(node => grid.Add(node));
            }

            return grid;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day4_2025>(13, 43, verbose: false);
        }
    }
}
