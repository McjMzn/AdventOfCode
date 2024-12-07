using AdventOfCode;
using AdventOfCode.Grids;

namespace Day4
{
    internal class Day4_2024 : IDailyChallenge<int>
    {
        public int Part1(IEnumerable<string> inputLines)
        {
            var (grid, xPositions, _) = LoadToGrid(inputLines);

            var totalFound = 0;
            foreach (var startingPosition in xPositions)
            {
                var (y, x) = grid.GetIndices(startingPosition);
                totalFound += Find(grid, x, y, "XMAS", 0, AllowedDirections.All);
            }

            return totalFound;
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            var (grid, _, aPositions) = LoadToGrid(inputLines);

            var found = 0;
            foreach (var a in aPositions)
            {
                var (y, x) = grid.GetIndices(a);
                var firstDiagonalNeighbours = grid.GetNeighbours(y, x, AllowedDirections.UpperLeft | AllowedDirections.LowerRight);
                var secondDiagonalNeighbours = grid.GetNeighbours(y, x, AllowedDirections.UpperRight| AllowedDirections.LowerLeft);
                if (firstDiagonalNeighbours.Contains("M") && firstDiagonalNeighbours.Contains("S") && secondDiagonalNeighbours.Contains("M") && secondDiagonalNeighbours.Contains("S"))
                {
                    found++;
                }
            }

            return found;
        }

        private int Find(ListGrid<string> grid, int x, int y, string keyword, int progress, AllowedDirections allowedDirections)
        {
            var currentCharacter = grid.Get(y, x);
            if (currentCharacter != keyword[progress].ToString())
            {
                return 0;
            }

            if (progress == keyword.Length - 1)
            {
                return 1;
            }

            var neighbours = grid.GetNeighboursIndices(y, x, allowedDirections);

            var found = 0;
            foreach(var n in neighbours)
            {
                found += Find(grid, n.X, n.Y, keyword, progress + 1, n.Direction);
            }

            return found;
        }

        private (ListGrid<string>, List<int>, List<int>) LoadToGrid(IEnumerable<string> inputLines)
        {
            var grid = new ListGrid<string>();
            grid.Width = inputLines.First().Length;
            grid.Height = inputLines.Count();
            var counter = 0;
            var xPositions = new List<int>();
            var aPositions = new List<int>();

            foreach (var line in inputLines)
            {
                foreach (var character in line)
                {
                    if (character == 'X')
                    {
                        xPositions.Add(counter);
                    }

                    if (character == 'A')
                    {
                        aPositions.Add(counter);
                    }

                    grid.Add(character.ToString());
                    counter++;
                }
            }

            return (grid, xPositions, aPositions);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day4_2024>(18, 9);
        }
    }
}
