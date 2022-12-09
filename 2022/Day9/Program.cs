using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace AdventOfCode.TwentyTwentyTwo.Day9
{
    internal record Point
    {
        public int X { get; set; }
        public int Y { get; set; }
    }
    
    internal interface IRopeSegment
    {
        char Symbol { get; set; }
        Point Position { get; set; }
    }

    internal class Knot : IRopeSegment
    {
        public char Symbol { get; set; }

        public Point Position { get; set; }

        public void PullToHeaed(IRopeSegment head)
        {
            if (this.AreTouching(head))
            {
                return;
            }
            else if (this.AreStraighTwoStepsAway(head))
            {
                this.MoveTailStraigh(head);
            }
            else
            {
                this.MoveTailDiagonally(head);
            }
        }

        private void MoveTailDiagonally(IRopeSegment head)
        {
            this.Position.Y += this.Position.Y > head.Position.Y ? -1 : 1;
            this.Position.X += this.Position.X > head.Position.X ? -1 : 1;
        }

        private void MoveTailStraigh(IRopeSegment head)
        {
            if (this.Position.X == head.Position.X)
            {
                this.Position.Y += this.Position.Y > head.Position.Y ? -1 : 1;
            }
            else
            {
                this.Position.X += this.Position.X > head.Position.X ? -1 : 1;
            }
        }

        private bool AreTouching(IRopeSegment head)
        {
            return Math.Abs(head.Position.X - this.Position.X) <= 1 && Math.Abs(head.Position.Y - this.Position.Y) <= 1;
        }

        private bool AreStraighTwoStepsAway(IRopeSegment head)
        {
            return
                head.Position.X == this.Position.X && Math.Abs(head.Position.Y - this.Position.Y) == 2 ||
                head.Position.Y == this.Position.Y && Math.Abs(head.Position.X - this.Position.X) == 2;
        }
    }

    internal class Rope : IRopeSegment
    {
        public Rope()
        {
            this.Segments = new List<IRopeSegment> { this };
            this.Changed += () =>
            {
                if (TailTrack.ContainsKey(this.Tail))
                {
                    TailTrack[this.Tail]++;
                }
                else
                {
                    TailTrack.Add(this.Tail, 1);
                }
            };
        }

        public event Action Changed;

        public Point Position { get; set; } = new Point { X = 0, Y = 0 };
        public List<IRopeSegment> Segments { get; set; }
        public Point Tail => this.Segments.Last().Position;
        public Dictionary<Point, int> TailTrack { get; set; } = new();
        public char Symbol { get; set; } = 'H';

        public void AddKnot(char symbol)
        {
            var knot = new Knot
            {
                Position = new Point { X = 0, Y = 0 },
                Symbol = symbol
            };

            this.Segments.Add(knot);
        }

        public void Move(string input)
        {
            var match = Regex.Match(input, @"(?<direction>.) (?<steps>\d+)");
            var steps = int.Parse(match.Groups["steps"].Value);
            var direction = match.Groups["direction"].Value;

            for (var i = 0; i < steps; i++)
            {
                switch (direction)
                {
                    case "R":
                        this.Position.X++;
                        break;
                    case "L":
                        this.Position.X--;
                        break;
                    case "U":
                        this.Position.Y--;
                        break;
                    case "D":
                        this.Position.Y++;
                        break;
                }

                this.UpdateKnots();
                this.Changed?.Invoke();
            }
        }

        private void UpdateKnots()
        {
            for(var i = 1; i < this.Segments.Count; i++)
            {
                (this.Segments[i] as Knot).PullToHeaed(this.Segments[i - 1]);
            }
        }
    }

    internal class Program
    {
        static void Render(Rope rope)
        {
            for (var y = -5; y <= 5; y++)
            {
                for (var x = -5; x <= 5; x++)
                {
                    var rendered = false;
                    foreach(var segment in rope.Segments)
                    {
                        if (segment.Position.X == x && segment.Position.Y == y)
                        {
                            Console.Write(segment.Symbol);
                            rendered = true;
                            break;
                        }
                    }

                    if (!rendered && x == 0 && y == 0)
                    {
                        Console.Write("s");
                        rendered = true;
                    }

                    if(!rendered)
                    {
                        Console.Write(".");
                    }
                }

                Console.WriteLine();
            }

            Console.WriteLine();
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            var rope = new Rope();
            rope.AddKnot('1');
            rope.AddKnot('2');
            rope.AddKnot('3');
            rope.AddKnot('4');
            rope.AddKnot('5');
            rope.AddKnot('6');
            rope.AddKnot('7');
            rope.AddKnot('8');
            rope.AddKnot('9');

            Render(rope);

            rope.Changed += () => Render(rope);

            rope.Move("R 4");
            rope.Move("U 4");
            rope.Move("L 3");
            rope.Move("D 1");
            rope.Move("R 4");
            rope.Move("D 1");
            rope.Move("L 5");
            rope.Move("R 2");

            Console.WriteLine($"Intro: Tail visited {rope.TailTrack.Count} positions.");

            // Part 1
            rope = new();
            rope.AddKnot('T');

            Input.Process(line => rope.Move(line));
            Console.WriteLine($"Part 1: Tail visited {rope.TailTrack.Count} positions.");

            // Part 2
            rope = new();
            rope.AddKnot('1');
            rope.AddKnot('2');
            rope.AddKnot('3');
            rope.AddKnot('4');
            rope.AddKnot('5');
            rope.AddKnot('6');
            rope.AddKnot('7');
            rope.AddKnot('8');
            rope.AddKnot('9');

            Input.Process(line => rope.Move(line));
            Console.WriteLine($"Part 2: Tail visited {rope.TailTrack.Count} positions.");
        }
    }
}