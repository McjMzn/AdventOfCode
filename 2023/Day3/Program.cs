using AdventOfCode;
using AdventOfCode.Grids;
using System.Linq;
using System.Text;

namespace Day3
{
    internal record EnginePart
    {
        public EnginePart(char symbol)
        {
            Character = symbol;
            IsEmpty = symbol == '.';
            IsNumber = Character >= '0' && Character <= '9';
            IsSymbol = !IsNumber && !IsEmpty;
            Value = IsNumber ? int.Parse($"{Character}") : null;
        }

        public char Character { get; }
        
        public bool IsNumber { get; }

        public bool IsEmpty { get; }
        public bool IsSymbol { get; }
        public int? Value { get; }

        public List<int> AdjacentNumbers { get; }

        public override string ToString()
        {
            return Character.ToString();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Demo should give 4361
            // Input.UseDemo();

            var size = Input.GetSize();
            var grid = new ArrayGrid<EnginePart>(size.Height, size.Width);

            Input.Process(line =>
            {
                line.ToList().ForEach(c => grid.AddNext(new EnginePart(c)));
            });

            var partNumbers = new List<int>();

            var numbers = new List<(int Number, int Row, int Column)>();

            for (var row = 0; row < grid.NumberOfRows; row++)
            {
                var numberBuilder = new StringBuilder();
                for (var column = 0; column < grid.NumberOfColumns; column++)
                {
                    var part = grid.Get(row, column);
                    if (part.IsNumber)
                    {
                        numberBuilder.Append(part.Character);
                    }

                    if (!part.IsNumber || column == grid.NumberOfColumns - 1)
                    {
                        if (numberBuilder.Length > 0)
                        {
                            var number = int.Parse(numberBuilder.ToString());
                            numbers.Add((number, row, column - numberBuilder.Length));
                        }

                        numberBuilder.Clear();
                    }
                }
            }

            var symbolIndices = 
                grid
                    .Nodes
                    .Where(n => n.IsSymbol)
                    .Select(grid.GetIndices)
                    .ToList();

            var part1Result =
                numbers
                    .Where(number =>
                        {
                            return
                                symbolIndices.Any(s =>
                                    s.X >= number.Column - 1 &&
                                    s.X <= number.Column + Math.Floor(Math.Log10(number.Number)) + 1 &&
                                    s.Y >= number.Row - 1 &&
                                    s.Y <= number.Row + 1
                                );
                        }
                    )
                    .Select(n => n.Number)
                    .Sum();

            Console.WriteLine($"Part 1: {part1Result}");

            var gearIndices =
                grid
                    .Nodes
                    .Where(n => n.Character == '*')
                    .Select(grid.GetIndices)
                    .ToList();

            var part2Result = 0;
            foreach(var gear in gearIndices)
            {
                var gearRatioComponents =
                    numbers
                        .Where(number =>
                            gear.X >= number.Column - 1 &&
                            gear.X <= number.Column + Math.Floor(Math.Log10(number.Number)) + 1 &&
                            gear.Y >= number.Row - 1 &&
                            gear.Y <= number.Row + 1
                        )
                        .Select(n => n.Number)
                        .ToList();

                if (gearRatioComponents.Count != 2)
                {
                    continue;
                }

                part2Result += gearRatioComponents.Aggregate(1, (a, b) => a * b);
            }

            Console.WriteLine($"Part 2: {part2Result}");
        }
    }
}
