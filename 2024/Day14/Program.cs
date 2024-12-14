using AdventOfCode;
using System.Text.RegularExpressions;

namespace Day14
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day14_2024>(12);
        }
    }

    internal record class Robot
    {
        public Robot(int x, int y, int vx, int vy)
        {
            X = x;
            Y = y;
            Vx = vx;
            Vy = vy;
        }

        public int X { get; set; }
        public int Y { get; set; }
        public int Vx { get; set; }
        public int Vy { get; set; }

        public void Update(int width, int height)
        {
            X = Modulo(X + Vx, width);
            Y = Modulo(Y + Vy, height);
        }

        private int Modulo(int a, int b)
        {
            return ((a % b) + b) % b;
        }
    }

    internal class Day14_2024 : IDailyChallenge<int>
    {
        public int Part1(IEnumerable<string> inputLines)
        {
            var width = AdventOfCodeRunner.RunningDemo ? 11 : 101;
            var height = AdventOfCodeRunner.RunningDemo ? 7 : 103;

            var robots = ProcessInput(inputLines);

            for (var i = 0; i < 100; i++)
            {
                robots.ForEach(r => r.Update(width, height));
            }

            var q1 = robots.Count(i => i.X < width / 2 && i.Y < height / 2);
            var q2 = robots.Count(i => i.X > width / 2 && i.Y < height / 2);
            var q3 = robots.Count(i => i.X < width / 2 && i.Y > height / 2);
            var q4 = robots.Count(i => i.X > width / 2 && i.Y > height / 2);

            var result = q1 * q2 * q3 * q4;

            return result;
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            var width = 101;
            var height = 103;

            var robots = ProcessInput(inputLines);
            
            using var consoleStream = Console.OpenStandardOutput();
            using var consoleWriter = new StreamWriter(consoleStream);
            using var fileWriter = File.AppendText("log.txt");

            for (var i = 1; i <= 10000; i++)
            {
                robots.ForEach(r => r.Update(width, height));
                
                // Logged the board to a file.
                // Noticed a pattern happening every 103 iterations (i.e. 509, 612, 715....)
                // Checked only those steps.
                // Found a christmas tree at 6587.
                if (!AdventOfCodeRunner.RunningDemo && i == 6587)
                {
                    Print(robots, width, height, consoleWriter);
                    return i;
                }
            }

            return 0;
        }

        private void Print(List<Robot> robots, int width, int height, StreamWriter writer)
        {
            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var robotsCount = robots.Where(r => r.X == x && r.Y == y).Count();

                    writer.Write(robotsCount == 0 ? "." : robotsCount.ToString());
                }

                writer.WriteLine();
            }
         
            writer.WriteLine();
        }

        private List<Robot> ProcessInput(IEnumerable<string> inputLines)
        {
            var regex = new Regex(@"p=(?<x>-?\d+),(?<y>-?\d+) v=(?<vx>-?\d+),(?<vy>-?\d+)");

            return
                inputLines
                .Select(line => regex.Match(line))
                .Where(match => match.Success)
                .Select(match => new Robot(
                    int.Parse(match.Groups["x"].Value),
                    int.Parse(match.Groups["y"].Value),
                    int.Parse(match.Groups["vx"].Value),
                    int.Parse(match.Groups["vy"].Value)
                ))
                .ToList();
        }
    }

}
