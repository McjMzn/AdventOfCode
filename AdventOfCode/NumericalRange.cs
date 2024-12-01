using System;
using System.Collections.Generic;

namespace AdventOfCode
{
    public record NumericalRange(long Start, long End)
    {
        public long Length { get; } = End - Start + 1;


        public bool Contains(long number)
        {
            return number >= Start && number <= End;
        }

        public NumericalRange AddOffset(long number)
        {
            return new NumericalRange(Start + number, End + number);
        }

        public NumericalRange GetIntersection(NumericalRange other)
        {
            if (Start > other.End || other.Start > End)
            {
                return null;
            }

            var start = Math.Max(Start, other.Start);
            var end = Math.Min(End, other.End);

            return new NumericalRange(start, end);
        }

        public (NumericalRange Intersection, List<NumericalRange> Leftovers) GetIntersectionAndLeftovers(NumericalRange other)
        {
            var intersection = GetIntersection(other);
            var leftovers = new List<NumericalRange>();

            if (intersection is null)
            {
                return (null, new() { this });
            }


            if (Start < intersection.Start)
            {
                leftovers.Add(new NumericalRange(Start, intersection.Start - 1));
            }

            if (intersection.End < End)
            {
                leftovers.Add(new NumericalRange(intersection.End + 1, End));
            }

            return (intersection, leftovers);
        }

        public override string ToString()
        {
            return $"({Start}..{End})";
        }
    }
}
