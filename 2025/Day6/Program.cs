using AdventOfCode;
using AdventOfCode.Extensions;
using System.Text;

namespace Day6
{
    internal class Day6_2025 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var result = 0L;

            var processedLines = ProcessLines(inputLines);
            var numberOfColumns = processedLines[0].Count;
            
            for (var column = 0; column < numberOfColumns; column++)
            {
                var items = LoadColumn(column, processedLines);
                var numbers = items.Take(items.Count - 1).Select(x => long.Parse(x));

                var aggregated = items.Last() switch
                {
                    "+" => numbers.Sum(),
                    "*" => numbers.Aggregate(1L, (aggreageted, current) => current * aggreageted),
                };

                result += aggregated;
            }
            
            return result;
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var result = 0L;

            // Load data into 2D array
            var lines = inputLines.ToList();
            var rows = lines.Count;
            var columns = lines.Select(l => l.Length).Max();
            var buffer = new char[rows, columns];
            for (var y = 0; y < lines.Count; y++)
            {
                for (var x = 0; x < lines[y].Length; x++)
                {
                    buffer[y, x] = lines[y][x];
                }
            }

            // Find delimeters
            var delimeters = new List<int>();
            for (var x = 0; x < columns; x++)
            {
                if (Enumerable.Range(0, rows).All(y => buffer[y, x] == ' '))
                {
                    delimeters.Add(x);
                }
            }

            // Iterate over sub-buffers
            for (var delimeter = -1; delimeter < delimeters.Count; delimeter++)
            {
                var fromX = delimeter < 0 ? 0 : delimeters[delimeter] + 1;


                var toX =
                    delimeter == delimeters.Count - 1 ? columns : delimeters[delimeter + 1];

                var subBuffer = new char[rows, toX - fromX];
                for (var y = 0; y < rows; y++)
                {
                    for(var x = 0; x < toX - fromX; x++)
                    {
                        subBuffer[y, x] = buffer[y, fromX + x];
                    }
                }

                AdventOfCodeRunner.WriteLine(subBuffer.Render('.'));

                var numbers = new List<long>();
                for (var x = 0; x < toX - fromX; x++)
                {
                    var numberBuilder = new StringBuilder();
                    for (var y = 0; y < rows - 1; y++)
                    {
                        numberBuilder.Append(subBuffer[y, x]);
                    }

                    numbers.Add(long.Parse(numberBuilder.ToString()));


                }
                
                var operation = Enumerable.Range(0, toX - fromX).Select(x => subBuffer[rows -1, x]).Aggregate("", (current, aggregated) => aggregated + current).Trim();
                var aggregated = operation switch
                {
                    "+" => numbers.Sum(),
                    "*" => numbers.Aggregate(1L, (aggreageted, current) => current * aggreageted),
                };

                AdventOfCodeRunner.Write(string.Join($" {operation} ", numbers));
                AdventOfCodeRunner.WriteLine($" = {aggregated}");
                AdventOfCodeRunner.WriteLine();

                result += aggregated;
            }



            return result;
        }

        private List<List<string>> ProcessLines(IEnumerable<string> inputLines)
        {
            var processed = new List<List<string>>();

            foreach(var line in inputLines)
            {
                var items = line.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
                processed.Add(items);
            }

            return processed;
        }

        private List<string> LoadColumn(int index, List<List<string>> processedLines)
        {
            var items = new List<string>();
            foreach (var row in processedLines)
            {
                items.Add(row[index]);
            }

            return items;
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day6_2025>(4277556, 3263827, verbose: false);
        }
    }
}
