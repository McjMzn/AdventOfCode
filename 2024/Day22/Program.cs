using AdventOfCode;

namespace Day22
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day22_2024>(37327623);
        }
    }

    internal class Day22_2024 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var result = 0L;
            foreach (var line in inputLines)
            {
                var price = long.Parse(line);
                for (var i = 0; i < 2000; i++)
                {
                    price = Evolve(price);
                }

                result += price;
            }

            return result;
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var result = 0L;

            HashSet<(int, int, int, int)> sequences = new();

            var data = new Dictionary<(long InitialValue, (int S1, int S2, int S3, int S4) Sequence), int>();

            foreach (var line in inputLines)
            {
                var prices = new List<int>();

                var price = long.Parse(line);
                
                prices.Add((int)(price % 10));

                for (var i = 0; i < 2000; i++)
                {
                    price = Evolve(price);
                    prices.Add((int)(price % 10));
                }

                for (var i = 4; i < 2000; i++)
                {
                    var sequence =
                    (
                        prices[i - 3] - prices[i - 4],
                        prices[i - 2] - prices[i - 3],
                        prices[i - 1] - prices[i - 2],
                        prices[i - 0] - prices[i - 1]
                    );

                    sequences.Add(sequence);

                    if (!data.ContainsKey((price, sequence)))
                    {
                        data.Add((price, sequence), prices[i]);
                    }
                }
            }

            var counter = 0;
            var lockObject = new object();

            Parallel.ForEach(sequences, new ParallelOptions { MaxDegreeOfParallelism = 22 }, sequence =>
            {
                var matching = data.Where(kvp => kvp.Key.Sequence == sequence).ToList();
                var score = matching.Select(kvp => kvp.Value).Sum();
                lock(lockObject)
                {
                    if (score > result)
                    {
                        result = score;
                    }
                }
            });

            return result;
        }

        private long Evolve(long secret)
        {
            secret = Mix(secret, secret * 64);
            secret = Prune(secret);

            secret = Mix(secret, secret / 32);
            secret = Prune(secret);

            secret = Mix(secret, secret * 2048);
            secret = Prune(secret);

            return secret;
        }

        private long Mix(long a, long b) => a ^ b;

        private long Prune(long a) => a % 16777216;
    }
}
