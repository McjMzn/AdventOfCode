using AdventOfCode;
using System.Text.RegularExpressions;

namespace Day2
{
    internal enum CubeColor
    {
        Red,
        Green,
        Blue
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var limits = new Dictionary<CubeColor, int>()
            {
                [CubeColor.Red] = 12,
                [CubeColor.Green] = 13,
                [CubeColor.Blue] = 14,
            };

            var drawRegex = new Regex(@"((?<count>\d+) (?<color>red|green|blue),?)+");

            var possibleGamesIdSum = 0;
            var powerSum = 0;

            Input.Process(line =>
            {
                var split = line.Split(':');
                var game = split[0];
                var gameId = int.Parse(game.Split(' ')[1]);

                var drawsCombined = split[1];
                var draws = drawsCombined.Split(";");

                var isPossible = true;

                var required = new Dictionary<CubeColor, int>()
                {
                    [CubeColor.Red] = 1,
                    [CubeColor.Green] = 1,
                    [CubeColor.Blue] = 1,
                };

                foreach(var draw in draws)
                {
                    var matches = drawRegex.Matches(draw.Trim()).Cast<Match>().ToList();
                    foreach(var match in matches)
                    {
                        var color = (CubeColor)Enum.Parse(typeof(CubeColor), match.Groups["color"].Value, true);
                        var count = int.Parse(match.Groups["count"].Value);

                        if (count > limits[color])
                        {
                            isPossible = false;
                        }

                        required[color] = count > required[color] ? count : required[color];
                    }
                }

                if (isPossible)
                {
                    possibleGamesIdSum += gameId;
                }

                powerSum += required[CubeColor.Red] * required[CubeColor.Green] * required[CubeColor.Blue];
            });

            Console.WriteLine($"Part 1: {possibleGamesIdSum}");
            Console.WriteLine($"Part 2: {powerSum}");
        }
    }
}