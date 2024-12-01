using AdventOfCode;
using System.Diagnostics;

namespace Day7
{
    internal class Hand : IComparable<Hand>
    {
        public Dictionary<char, int> Cards { get; private set; } = new()
        {
            ['X'] = 0,
            ['2'] = 0,
            ['3'] = 0,
            ['4'] = 0,
            ['5'] = 0,
            ['6'] = 0,
            ['7'] = 0,
            ['8'] = 0,
            ['9'] = 0,
            ['T'] = 0,
            ['J'] = 0,
            ['Q'] = 0,
            ['K'] = 0,
            ['A'] = 0,
        };

        public string CardsString { get; private set; }

        public void Promote()
        {
            if (Cards['J'] == 0)
            {
                return;
            }

            CardsString = CardsString.Replace('J', 'X');

            var max = Cards.Where(kvp => kvp.Key != 'J').Select(kvp => kvp.Value).Max();
            var best = Cards.Where(kvp => kvp.Value == max && kvp.Key != 'J').Last();

            Cards[best.Key] += Cards['J'];
            Cards['J'] = 0;
        }

        public int GetScore()
        {
            // High card
            if (Cards.Count(x => x.Value == 1) == 5)
            {
                return 1;
            }

            // One pair
            if (Cards.Count(x => x.Value == 2) == 1 && Cards.Count(x => x.Value == 1) == 3)
            {
                return 2;
            }

            // Two pair
            if (Cards.Count(x => x.Value == 2) == 2)
            {
                return 3;
            }

            // Three of a kind
            if (Cards.Count(x => x.Value == 3) == 1 && Cards.Count(x => x.Value == 1) == 2)
            {
                return 4;
            }

            // Full house
            if (Cards.Count(x => x.Value == 3) == 1 && Cards.Count(x => x.Value == 2) == 1)
            {
                return 5;
            }

            // Four of a kind
            if (Cards.Count(x => x.Value == 4) == 1)
            {
                return 6;
            }

            // Three of a kind
            if (Cards.Count(x => x.Value == 5) == 1)
            {
                return 7;
            }

            throw new UnreachableException("Illegal state!");
        }

        public static Hand FromString(string text)
        {
            var hand = new Hand();
            hand.CardsString = text;
            foreach (var symbol in text)
            {
                hand.Cards[symbol]++;
            }

            return hand;
        }

        public int CompareTo(Hand? other)
        {
            var score = GetScore();
            var otherScore = other.GetScore();

            if (score > otherScore)
            {
                return 1;
            }

            if (score < otherScore)
            {
                return -1;
            }

            for (var i = 0; i < 5; i++)
            {
                var index = Cards.Keys.ToList().IndexOf(CardsString[i]);
                var otherIndex = Cards.Keys.ToList().IndexOf(other.CardsString[i]);

                if (index > otherIndex)
                {
                    return 1;
                }

                if (index < otherIndex)
                {
                    return -1;
                }
            }

            return 0;
        }

        public override string ToString()
        {
            return $"{CardsString} = {GetScore()}";
        }
    }

    internal class Bid
    {
        public int Value { get; set; }
        public Hand Cards { get; set; }

        public override string ToString()
        {
            return $"${Value}|{Cards}";
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            var bids = new List<Bid>();
            
            Input.Process(line =>
            {
                var split = line.Split(' ');
                var bid = new Bid
                {
                    Cards = Hand.FromString(split.First()),
                    Value = int.Parse(split.Last())
                };

                bids.Add(bid);
            });

            Console.WriteLine($"Part 1: {GetResult(bids)}");

            bids.ForEach(b => b.Cards.Promote());

            Console.WriteLine($"Part 2: {GetResult(bids)}");
        }

        static int GetResult(List<Bid> bids)
        {
            var sorted =
                bids
                    .OrderBy(x => x.Cards)
                    .ToList();

            return
                sorted
                    .Select((x, index) => x.Value * (index + 1))
                    .Sum();
        }
    }
}