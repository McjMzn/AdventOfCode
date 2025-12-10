using AdventOfCode;
using AdventOfCode.Grids;

namespace Day7
{
    internal class Node : IHaveCoordinates
    {
        public Node(char symbol)
        {
            Symbol = symbol;
        }
        public char Symbol { get; set; }

        public bool IsEmptySpace => Symbol == '.';

        public bool IsBeam => Symbol == '|';

        public bool IsSplitter => Symbol == '^';

        public bool IsStart => Symbol == 'S';

        public Coordinates Coordinates { get; set; }

        public override string ToString()
        {
            return Symbol.ToString();
        }
    }

    internal class Day7_2025 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var result = 0L;

            var grid = LoadGrid(inputLines);
            AdventOfCodeRunner.WriteLine(grid.ToString());

            var beams = 0;
            var updatedBeams = 0;
            do
            {
                beams = updatedBeams;
                updatedBeams = UpdateGrid(grid);
                AdventOfCodeRunner.WriteLine(grid.ToString());
            }
            while (beams != updatedBeams);

            return grid.Nodes.Count(n => n.IsSplitter && grid.GetUpFrom(n).IsBeam);
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var grid = LoadGrid(inputLines);
            var start = grid.Nodes.First(n => n.IsStart);

            return CountPaths(grid, start);
        }

        
        private Dictionary<Node, long> _cache = new();

        private long CountPaths(Grid<Node> grid, Node current)
        {
            if (_cache.ContainsKey(current))
            {
                return _cache[current];
            }

            var next = grid.Get(current.Coordinates.Y + 1, current.Coordinates.X);

            switch (next)
            {
                case null:
                    _cache[current] = 1;
                    return _cache[current];

                case { IsSplitter: true }:
                    _cache[current] = 0;
                    var left = grid.Get(next.Coordinates.Y, next.Coordinates.X - 1);
                    if (left is not null)
                    {
                        _cache[current] += CountPaths(grid, left);
                    }

                    var right = grid.Get(next.Coordinates.Y, next.Coordinates.X + 1);
                    if (right is not null)
                    {
                        _cache[current] += CountPaths(grid, right);
                    }

                    return _cache[current];

                default:
                    _cache[current] = CountPaths(grid, next);
                    return _cache[current];
            }
        }
        
        private int UpdateGrid(Grid<Node> grid)
        {
            var start = grid.Nodes.First(n => n.IsStart);

            var belowStart = grid.GetDownFrom(start);
            if (belowStart.IsEmptySpace)
            {
                belowStart.Symbol = '|';
                return grid.Nodes.Count(n => n.IsBeam);
            }

            var beams = grid.Nodes.Where(n => n.IsBeam).ToList();
            foreach (var beam in beams)
            {
                var below = grid.GetDownFrom(beam);
                switch (below)
                {
                    case { IsEmptySpace: true }:
                        below.Symbol = '|';
                        break;

                    case { IsSplitter: true }:
                        var left = grid.GetLeftFrom(below);
                        var right = grid.GetRightFrom(below);
                        if (left.IsEmptySpace)
                        {
                            left.Symbol = '|';
                        }

                        if (right.IsEmptySpace)
                        {
                            right.Symbol = '|';
                        }

                        break;
                }
            }

            return grid.Nodes.Count(n => n.IsBeam);
        }

        private ListGrid<Node> LoadGrid(IEnumerable<string> inputLines)
        {
            var grid = new ListGrid<Node>();
            grid.Width = inputLines.First().Length;
            grid.Height = inputLines.Count();

            foreach(var line in inputLines)
            {
                line.ToCharArray().ToList().Select(c => new Node(c)).ToList().ForEach(n => grid.Add(n));
            }

            return grid;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day7_2025>(21, 40, verbose: false);
        }
    }
}
