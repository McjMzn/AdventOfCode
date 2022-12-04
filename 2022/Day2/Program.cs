using AdventOfCode;
using System;
using System.Collections.Generic;

namespace AdventOfCode.TwentyTwentyTwo.Day2
{
    internal enum Outcome
    {
        Lose = 0,
        Draw = 3,
        Win = 6
    }

    internal enum Figure
    {
        Rock = 1,
        Paper = 2,
        Scisors = 3
    }

    internal static class FigureExtensions
    {
        public static Figure GetOneThatItWinsWith(this Figure figure) => figure switch
        {
            Figure.Rock => Figure.Scisors,
            Figure.Paper => Figure.Rock,
            Figure.Scisors => Figure.Paper
        };

        public static Figure GetOneThatItLosesWith(this Figure figure) => figure switch
        {
            Figure.Rock => Figure.Paper,
            Figure.Paper => Figure.Scisors,
            Figure.Scisors => Figure.Rock
        };
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            var opponentInputSymbolToFigure = new Dictionary<char, Figure>
            {
                ['A'] = Figure.Rock,
                ['B'] = Figure.Paper,
                ['C'] = Figure.Scisors
            };

            var playerInputSymbolToFigure = new Dictionary<char, Figure>
            {
                ['X'] = Figure.Rock,
                ['Y'] = Figure.Paper,
                ['Z'] = Figure.Scisors
            };

            var playerSymbolToDesiredOutcome = new Dictionary<char, Outcome>
            {
                ['X'] = Outcome.Lose,
                ['Y'] = Outcome.Draw,
                ['Z'] = Outcome.Win,
            };

            var part1score = 0;
            var part2score = 0;
            
            Input.Process(line =>
            {
                var opponent = opponentInputSymbolToFigure[line[0]];
                
                var playerPart1 = playerInputSymbolToFigure[line[2]];
                var playerPart2 = ChoosePlayerFigureToGetOutcome(opponent, playerSymbolToDesiredOutcome[line[2]]);

                part1score += CalculateDuelScore(playerPart1, opponent);
                part2score += CalculateDuelScore(playerPart2, opponent);
            });

            
            // Part 1
            Console.WriteLine($"Part 1: {part1score}");

            // Part 2
            Console.WriteLine($"Part 2: {part2score}");
        }

        private static Figure ChoosePlayerFigureToGetOutcome(Figure opponent, Outcome outcome)
        {
            return outcome switch
            {
                Outcome.Lose => opponent.GetOneThatItWinsWith(),
                Outcome.Draw => opponent,
                Outcome.Win => opponent.GetOneThatItLosesWith()
            };
        }

        private static int CalculateDuelScore(Figure player, Figure opponent)
        {
            if (player == opponent)
            {
                return (int)player + (int)Outcome.Draw;
            }

            if (
                player == Figure.Rock && opponent == Figure.Scisors ||
                player == Figure.Paper && opponent == Figure.Rock ||
                player == Figure.Scisors && opponent == Figure.Paper
                )
            {
                return (int)player + (int)Outcome.Win;
            }

            return (int)player + (int)Outcome.Lose;
        }
    }
}