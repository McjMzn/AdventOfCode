using AdventOfCode;
using System.Text.RegularExpressions;

namespace Day4
{
    internal class Scratchcard
    {
        public int Id { get; set; }

        public List<int> WinningNumbers { get; set; }

        public List<int> PlayingNumbers { get; set; }

        public int Matches => WinningNumbers.Intersect(PlayingNumbers).Count();

        public int Points => (int)Math.Pow(2, Matches - 1);

        public int Count { get; set; } = 1;

        public static Scratchcard Parse(string cardText)
        {
            var match = Regex.Match(cardText, @"Card\s+(?<id>\d+): (?<winningNumbers>[^|]*)\|(?<playingNumbers>.*)");
            var card = new Scratchcard();
            card.Id = int.Parse(match.Groups["id"].Value);

            card.WinningNumbers = match.Groups["winningNumbers"].Value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList();
            card.PlayingNumbers = match.Groups["playingNumbers"].Value.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList();

            return card;
        }

        public override string ToString()
        {
            return $"{Count}x Card {Id}: {string.Join(' ', WinningNumbers)} | {string.Join(' ', PlayingNumbers)}";
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Input.UseDemo();

            var cards = new List<Scratchcard>();
            Input.Process(line =>
            {
                cards.Add(Scratchcard.Parse(line));
            });

            var points = cards.Select(x => x.Points).Sum();
            Console.WriteLine($"Part 1: {points}");

            foreach (var card in cards)
            {
                cards.Where(c => c.Id > card.Id && c.Id <= card.Id + card.Matches).ToList().ForEach(c => c.Count += card.Count);
            }

            Console.WriteLine($"Part 2: {cards.Sum(c => c.Count)}");
        }
    }
}