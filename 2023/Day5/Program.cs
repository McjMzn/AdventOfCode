using AdventOfCode;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;

namespace Day5
{
    internal record Mapping(NumericalRange Input, NumericalRange Output)
    {
        public long Offset { get; } = Output.Start - Input.Start;

        public override string ToString()
        {
            return $"{Input} {(Offset >= 0 ? "+" : "")}{Offset} {Output}";
        }
    }

    internal class MappingLevel
    {
        public List<Mapping> Mappings { get; set; } = new();

        public void AddMappingFromText(string text)
        {
            var match = Regex.Match(text, @"(?<destinationRangeStart>\d+) (?<sourceRangeStart>\d+) (?<rangeLength>\d+)");

            var sourceRangeStart = long.Parse(match.Groups["sourceRangeStart"].Value);
            var destinationRangeStart = long.Parse(match.Groups["destinationRangeStart"].Value);
            var rangeLength = long.Parse(match.Groups["rangeLength"].Value);

            var input = new NumericalRange(sourceRangeStart, sourceRangeStart + rangeLength - 1);
            var output = new NumericalRange(destinationRangeStart, destinationRangeStart + rangeLength - 1);
         
            Mappings.Add(new (input, output));
        }

        public static MappingLevel Parse(string text)
        {
            var mappingLevel = new MappingLevel();
            var lines = text.Replace("\r", "").Split('\n').ToList();
            foreach (var line in lines)
            {
                mappingLevel.AddMappingFromText(line);
            }

            return mappingLevel;
        }

        public override string ToString()
        {
            return string.Join(", ", Mappings);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            // Input.UseDemo();

            List<long> seeds = new List<long>();
            List<NumericalRange> seedRanges = new List<NumericalRange>();

            List<MappingLevel> mappingLevels = new List<MappingLevel>();
            MappingLevel current = null;

            Input.Process(line =>
            {
                if (line.StartsWith("seeds:"))
                {
                    seeds = line.Split(" ", StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Skip(1).Select(long.Parse).ToList();
                    for (var i = 0; i < seeds.Count; i += 2)
                    {
                        seedRanges.Add(new NumericalRange(seeds[i], seeds[i] + seeds[i + 1]));
                    }
                    
                    return;
                }

                if (line.Length == 0)
                {
                    return;
                }

                if (line.Contains("-to-"))
                {
                    current = new MappingLevel();
                    mappingLevels.Add(current);

                    return;
                }

                current.AddMappingFromText(line);
            });


            var results = new List<long>();
            var progress = 0;

            seeds.ForEach(seed =>
            {
                results.Add(ProcessSeed(seed, mappingLevels));
            });

            Console.WriteLine($"Part 1: {results.Min()}");

            var locationRanges = ProcessSeedsRange(seedRanges, mappingLevels);
            Console.WriteLine($"Part 2: {locationRanges.Select(r => r.Start).Min()}");
        }

        private static long ProcessSeed(long input, List<MappingLevel> mappingLevels)
        {
            var current = mappingLevels.First();
            var nextStepMappnigLevels = mappingLevels.Skip(1).ToList();

            var appliable = current.Mappings.Where(m => m.Input.Contains(input)).ToList();
            
            var mapping = appliable.SingleOrDefault();
            
            var nextInput = input;

            if (mapping is not null)
            {
                var offset = input - mapping.Input.Start;
                nextInput = mapping.Output.Start + offset;
            }

            if (nextStepMappnigLevels.Count > 0)
            {
                return ProcessSeed(nextInput, nextStepMappnigLevels);
            }

            return nextInput;
        }

        private static List<NumericalRange> ProcessSeedsRange(List<NumericalRange> seedRanges, List<MappingLevel> mappingLevels, int indentLevel = 0)
        {
            var indent = new string(' ', indentLevel * 2);
            var currentMappingLevel = mappingLevels.First();

            Console.WriteLine($"{indent}Ranges:   {string.Join(", ", seedRanges)}");
            Console.WriteLine($"{indent}Mappings: {string.Join(", ", currentMappingLevel.Mappings)}");

            var rangesToProcess = seedRanges.ToList();
            var foundMatches = new Dictionary<NumericalRange, Mapping>();

            foreach (var mapping in currentMappingLevel.Mappings)
            {
                foreach(var range in rangesToProcess.ToList())
                {
                    var intersectionAndLeftovers = range.GetIntersectionAndLeftovers(mapping.Input);
                    if (intersectionAndLeftovers.Intersection is not null)
                    {
                        foundMatches.Add(intersectionAndLeftovers.Intersection, mapping);
                    }

                    rangesToProcess.Remove(range);

                    rangesToProcess.AddRange(intersectionAndLeftovers.Leftovers);
                }
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            foreach(var match in foundMatches)
            {
                Console.WriteLine($"{indent} Mapping {match.Key} using {match.Value}");
            }

            foreach (var range in rangesToProcess)
            {
                Console.WriteLine($"{indent} Leaving {range} unmodified");
            }

            Console.ResetColor();

            var nextStepMappingLevels = mappingLevels.Skip(1).ToList();
            var nextStepRanges =
                foundMatches
                    .Select(kvp => kvp.Key.AddOffset(kvp.Value.Offset))
                    .Concat(rangesToProcess)
                    .ToList();

            if (nextStepMappingLevels.Count == 0)
            {
                return nextStepRanges;
            }

            return ProcessSeedsRange(nextStepRanges, nextStepMappingLevels, indentLevel + 1);
        }
    }
}