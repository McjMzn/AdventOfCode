using AdventOfCode;

namespace Day11
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day11_2024>(55312);
        }
    }

    internal class Day11_2024 : IDailyChallenge<long>
    {
        public long Part1(IEnumerable<string> inputLines)
        {
            var stones = inputLines.Single().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(long.Parse).ToList();
            for (var i = 0; i < 25; i++)
            {
                var updated = new List<long>();
                for (var stone = 0; stone < stones.Count; stone++)
                {
                    if (stones[stone] == 0)
                    {
                        updated.Add(1);
                        continue;
                    }

                    var stringified = stones[stone].ToString();
                    if (stringified.Length % 2 == 0)
                    {
                        updated.Add(long.Parse(stringified.Substring(0, stringified.Length / 2)));
                        updated.Add(long.Parse(stringified.Substring(stringified.Length / 2)));
                        continue;
                    }

                    updated.Add(stones[stone] * 2024);

                }

                stones = updated;
            }

            return stones.Count();
        }
        
        public long Part2(IEnumerable<string> inputLines)
        {
            var cache = new Dictionary<long, Dictionary<long, long>>();
            
            return
                inputLines
                    .Single()
                    .Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Select(long.Parse)
                    .Select(value => GetStonesCountAfterBlinks(value, 75, cache))
                    .Sum();
        }

        private long Cached(long numberOfStones, long initialValue, long numberOfBlinks, Dictionary<long, Dictionary<long, long>> cache)
        {
            if (!cache.ContainsKey(initialValue))
            {
                cache[initialValue] = new();
            }

            cache[initialValue][numberOfBlinks] = numberOfStones;
            return numberOfStones;
        }
        private long GetStonesCountAfterBlinks(long initialValue, long numberOfBlinks, Dictionary<long, Dictionary<long, long>> cache)
        {
            if (cache.ContainsKey(initialValue) && cache[initialValue].ContainsKey(numberOfBlinks))
            {
                return cache[initialValue][numberOfBlinks];
            }

            // The stone does not change if you don't blink.
            if (numberOfBlinks == 0)
            {
                return 1;
            }

            if (initialValue == 0)
            {
                return Cached(GetStonesCountAfterBlinks(1, numberOfBlinks - 1, cache), initialValue, numberOfBlinks, cache);
            }

            var stringified = initialValue.ToString();
            if (stringified.Length % 2 == 0)
            {
                var a = int.Parse(stringified.Substring(0, stringified.Length / 2));
                var b = int.Parse(stringified.Substring(stringified.Length / 2));

                var aResult = GetStonesCountAfterBlinks(a, numberOfBlinks - 1, cache);
                var bResult = GetStonesCountAfterBlinks(b, numberOfBlinks - 1, cache);
             
                return Cached(aResult + bResult, initialValue, numberOfBlinks, cache);
            }

            return Cached(GetStonesCountAfterBlinks(2024 * initialValue, numberOfBlinks - 1, cache), initialValue, numberOfBlinks, cache);
        }
    }
}
