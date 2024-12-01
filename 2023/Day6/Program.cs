using AdventOfCode;

namespace Day6
{
    internal record Race(long Duration, long BestDistance)
    {
        public List<long> GetPossibleOutcomes()
        {
            var outcomes = new List<long>();

            for (var holdDuration = 0; holdDuration <= Duration; holdDuration++)
            {
                outcomes.Add(holdDuration * (Duration - holdDuration));
            }

            return outcomes;
        }

        public List<long> GetRecordScoringOutcomes()
        {
            return GetPossibleOutcomes().Where(o => o > BestDistance).ToList();
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Input.UseDemo();
            var input = Input.LoadLines();

            var times = input[0].Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList();
            var distances = input[1].Split(':').Last().Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToList();

            var races = new List<Race>();
            for (var i = 0; i < times.Count; i++)
            {
                races.Add(new Race(times[i], distances[i]));
            }

            var recordScoringOutcomes = races.Select(r => r.GetRecordScoringOutcomes().Count).ToList();
            var part1 = recordScoringOutcomes.Aggregate(1, (a, b) => a * b);

            Console.WriteLine($"Part 1: {part1}");
            Console.WriteLine($"Part 2:");

            var duration = long.Parse(input[0].Split(':').Last().Replace(" ", ""));
            var distance = long.Parse(input[1].Split(':').Last().Replace(" ", ""));
            var race = new Race(duration, distance);

            var outcomes = race.GetRecordScoringOutcomes();
        }
    }
}