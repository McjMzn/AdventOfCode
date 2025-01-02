using AdventOfCode;
using System.Reflection.Emit;

namespace Day19
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day19_2024>(6, 16, runDemo: false);
        }
    }

    internal class Day19_2024 : IDailyChallenge<long>
    {
        private Dictionary<char, List<string>> _towelsDictionary;

        private List<string> _towels;

        private Dictionary<string, bool> _cache = new();

        private Dictionary<string, long> _countCache = new();

        public long Part1(IEnumerable<string> inputLines)
        {
            var (towels, patterns) = ProcessInput(inputLines);

            var possiblePatternsCount = 0;

            foreach (var pattern in patterns)
            {
                if (Check(pattern, 0))
                {
                    possiblePatternsCount++;
                }
            };

            return possiblePatternsCount;
        }

        public long Part2(IEnumerable<string> inputLines)
        {
            var (towels, patterns) = ProcessInput(inputLines);

            var result = 0L;
            foreach (var pattern in patterns)
            {
                var count = Count(pattern);
                result += count;
            };

            // too low: 767140201
            // too low: 784859102

            return result;
        }

        private bool Check(string pattern, int level)
        {
            if (_cache.ContainsKey(pattern))
            {
                return _cache[pattern];
            }

            if (!_towelsDictionary.ContainsKey(pattern[0]))
            {
                _cache[pattern] = false;
                return false;
            }

            var towels = _towelsDictionary[pattern[0]];
            foreach (var towel in towels)
            {
                if (towel.Length > pattern.Length)
                {
                    _cache[pattern] = false;
                    return false;
                }

                if (towel == pattern)
                {
                    _cache[pattern] = true;
                    return true;
                }

                if (pattern.StartsWith(towel) && Check(pattern.Substring(towel.Length), level + 1))
                {
                    _cache[pattern] = true;
                    return true;
                }
            }

            _cache[pattern] = false;
            return false;
        }

        private long Count(string pattern)
        {
            if (_countCache.ContainsKey(pattern))
            {
                return _countCache[pattern];
            }

            var count = 0L;
            foreach (var towel in _towels)
            {
                if (towel == pattern)
                {
                    count++;
                    continue;
                }

                if (pattern.StartsWith(towel))
                {
                    var subcount = Count(pattern.Substring(towel.Length));
                    count += subcount;
                }
            }

            _countCache[pattern] = count;
            return count;
        }

        private (List<string> AvailableTowels, List<string> Patterns) ProcessInput(IEnumerable<string> inputLines)
        {
            var lines = inputLines.ToList();

            var towels = lines[0].Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
            var patterns = lines.Skip(2).ToList();

            _towels = towels;

            _towelsDictionary = new Dictionary<char, List<string>>();
            foreach (var towel in towels)
            {
                if (!_towelsDictionary.ContainsKey(towel[0]))
                {
                    _towelsDictionary[towel[0]] = new();
                }

                _towelsDictionary[towel[0]].Add(towel);
            }

            foreach (var key in _towelsDictionary.Keys)
            {
                _towelsDictionary[key] = _towelsDictionary[key].OrderBy(x => x.Length).ToList();
            }

            return (towels, patterns);
        }
    }
}
