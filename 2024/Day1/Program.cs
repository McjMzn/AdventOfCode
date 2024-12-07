using AdventOfCode;

namespace Day1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var leftColumn = new List<int>();
            var rightColumn = new List<int>();

            Input.LoadLines().ForEach(line =>
            {
                var split = line.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                leftColumn.Add(int.Parse(split[0]));
                rightColumn.Add(int.Parse(split[1]));
            });

            Part1(leftColumn, rightColumn);

            Part2(leftColumn, rightColumn);
        }

        static void Part1(List<int> leftColumn, List<int> rightColumn)
        {
            leftColumn.Sort();
            rightColumn.Sort();

            var distancesSum = leftColumn.Zip(rightColumn).Select(x => Math.Abs(x.First - x.Second)).Sum();

            Output.Part1(distancesSum, TimeSpan.Zero);
        }

        static void Part2(List<int> leftColumn, List<int> rightColumn)
        {
            var similarity = 0;
            foreach (var leftItem in leftColumn)
            {
                similarity += leftItem * rightColumn.Count(x => x == leftItem);
            }

            Output.Part2(similarity, TimeSpan.Zero);
        }
    }
}
