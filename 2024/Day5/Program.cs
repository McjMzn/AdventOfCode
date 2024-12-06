using AdventOfCode;
using System.Text.Json;

namespace Day5
{
    internal class Day5_2024 : IDailyChallenge
    {
        private List<List<int>> _incorrectUpdates = new List<List<int>>();


        public int Part1(IEnumerable<string> inputLines)
        {
            var result = 0;
            var (rules, updates) = ProcessInput(inputLines);

            foreach (var update in updates)
            {
                var ruleset = CreateLocalRuleset(rules, update);

                // Check the numbers against the ruleset
                var added = new HashSet<int>();
                foreach (var number in update)
                {
                    //  If there's no rule for the number just move on.
                    if (!ruleset.ContainsKey(number))
                    {
                        added.Add(number);
                        continue;
                    }

                    // If any requirement is not met, stop checking. 
                    if (ruleset[number].Any(requirement => !added.Contains(requirement)))
                    {
                        _incorrectUpdates.Add(update);
                        break;
                    }

                    added.Add(number);
                }

                if (added.Count == update.Count)
                {
                    var midIndex = (int)Math.Floor(update.Count / 2.0);
                    result += update[midIndex];
                }
            }

            return result;
        }

        public int Part2(IEnumerable<string> inputLines)
        {
            var result = 0;
            var (rules, updates) = ProcessInput(inputLines);

            foreach (var update in _incorrectUpdates)
            {
                var ruleset = CreateLocalRuleset(rules, update);
                var reorderedPages = ReorderPages(ruleset, update);

                var midIndex = (int)Math.Floor(update.Count / 2.0);
                result += reorderedPages[midIndex];
            }

            return result;
        }

        private List<int> ReorderPages(Dictionary<int, List<int>> ruleset, List<int> pagesToInsert)
        {
            AdventOfCodeRunner.WriteLine(JsonSerializer.Serialize(pagesToInsert));
            foreach (var r in ruleset.OrderBy(r => r.Value.Count()))
            {
                AdventOfCodeRunner.WriteLine($"  {r.Key} requires {JsonSerializer.Serialize(r.Value)}");
            }

            var reordered = new List<int>();
            foreach(var page in pagesToInsert)
            {
                if (reordered.Contains(page))
                {
                    continue;
                }

                if (!ruleset.ContainsKey(page))
                {
                    reordered.Add(page);
                    continue;
                }

                reordered.AddRange(ruleset[page].Except(reordered));
                reordered.Add(page);
            }

            AdventOfCodeRunner.WriteLine(JsonSerializer.Serialize(reordered));
            AdventOfCodeRunner.WriteLine();

            return reordered;
        }

        private Dictionary<int, List<int>> CreateLocalRuleset(List<(int RequiredPrecedent, int Number)> rules, List<int> pages)
        {
            var rulesDictionary = new Dictionary<int, List<int>>();
            foreach (var rule in rules)
            {
                if (!pages.Contains(rule.Number) || !pages.Contains(rule.RequiredPrecedent))
                {
                    continue;
                }

                if (!rulesDictionary.ContainsKey(rule.Number))
                {
                    rulesDictionary[rule.Number] = new List<int>();
                }

                rulesDictionary[rule.Number].Add(rule.RequiredPrecedent);
            }

            foreach (var key in rulesDictionary.Keys)
            {
                rulesDictionary[key] = rulesDictionary[key].OrderBy(v => (rulesDictionary.ContainsKey(v) ? rulesDictionary[v].Count : 0)).ToList();
            }

            return rulesDictionary;
        }

        private (List<(int RequiredPrecedent, int Number)> Rules, List<List<int>> Updates) ProcessInput(IEnumerable<string> inputLines)
        {
            var loadingRules = true;

            var rules = new List<(int RequiredPrecedent, int Number)>();
            var updates = new List<List<int>>();

            foreach(var line in inputLines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    loadingRules = false;
                    continue;
                }

                if (loadingRules)
                {
                    var numbers = line.Split('|').Select(int.Parse).ToList();
                    rules.Add((numbers[0], numbers[1]));
                    continue;
                }

                updates.Add(line.Split(',').Select(int.Parse).ToList());
            }

            return (rules, updates);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            AdventOfCodeRunner.Run<Day5_2024>(143, 123, runDemo: false, verbose: true);
        }
    }
}
