using AdventOfCode;
using AdventOfCode.Grids;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace Day17
{
    internal class Shape
    {
        public int Offset { get; set; }

        public int Width => Bottom.Length;
    
        public int[] Bottom { get; set; }

        public int[] Height { get; set; }

        public override string ToString()
        {
            return $"{string.Join(", ", this.Bottom)} => {this.Offset}";
        }

        public static Shape Minus => new Shape
        {
            Bottom = new int[] { 0, 0, 0, 0 },
            Height = new int[] { 1, 1, 1, 1 }
        };

        public static Shape Plus => new Shape
        {
            Bottom = new int[] { 1, 0, 1 },
            Height = new int[] { 1, 3, 1 }
        };

        public static Shape L => new Shape
        {
            Bottom = new int[] { 0, 0, 0 },
            Height = new int[] { 1, 1, 3 }
        };

        public static Shape I => new Shape
        {
            Bottom = new int[] { 0 },
            Height = new int[] { 4 }
        };

        public static Shape Box => new Shape
        {
            Bottom = new int[] { 0, 0 },
            Height = new int[] { 2, 2 }
        };
    }

    internal class Chamber
    {
        public int[] Floor { get; set; } = new int[] { 0, 0, 0, 0, 0, 0, 0 };

        public List<int>[] Hollows { get; set; } = new List<int>[] { new(), new(), new(), new(), new(), new(), new(), };

        public int ShapesSpawned { get; set; } = 0;

        public Shape ActiveShape { get; set; }

        public void SpawnShape(int counterOverride = -1)
        {
            const int numberOfShapes = 5;
            var shape = (counterOverride >= 0 ? counterOverride : this.ShapesSpawned) switch
            {
                0 => Shape.Minus,
                1 => Shape.Plus,
                2 => Shape.L,
                3 => Shape.I,
                4 => Shape.Box,
            };

            this.ShapesSpawned = (this.ShapesSpawned + 1) % numberOfShapes;

            var maxHeight = this.Floor.Max();
            for (var i = 0; i < shape.Width; i++)
            {
                shape.Bottom[i] = shape.Bottom[i] + maxHeight + 3 + 1;
            }

            shape.Offset = 2;
            this.ActiveShape = shape;
        }

        public void MoveLeft()
        {
            if (this.ActiveShape.Offset == 0)
            {
                return;
            }

            this.MoveHorizontal(-1);
        }

        public void MoveRight()
        {
            if (this.ActiveShape.Offset + this.ActiveShape.Width == 7)
            {
                return;
            }

            this.MoveHorizontal(1);
        }

        public void MoveHorizontal(int move)
        {
            var range = move < 0 ? Enumerable.Range(0, this.ActiveShape.Width) : Enumerable.Range(0, this.ActiveShape.Width).Reverse();
            foreach(var i in range)
            {
                var targetIndex = this.ActiveShape.Offset + i + move;
                if (this.Floor[targetIndex] == this.ActiveShape.Bottom[i])
                {
                    return;
                }

                if (this.Floor[targetIndex] > this.ActiveShape.Bottom[i])
                {
                    var height = this.ActiveShape.Height[i];
                    for (var h = 0; h < height; h++)
                    {
                        if (this.Hollows[targetIndex].Contains(this.ActiveShape.Bottom[i] + h))
                        {
                            continue;
                        }

                        return;
                    }
                }
            }

            this.ActiveShape.Offset += move;
        }

        public bool MoveDown()
        {
            var offset = this.ActiveShape.Offset;
            for (var i = 0; i < this.ActiveShape.Width; i++)
            {
                var floorIndex = offset + i;
                var floorLevel = this.Floor[floorIndex];
                var hollows = this.Hollows[floorIndex];
                var rockBottom = this.ActiveShape.Bottom[i];

                // Regular fall
                if (floorLevel < rockBottom - 1)
                {
                    continue;
                }

                // Hollow fall
                if (hollows.Contains(rockBottom - 1))
                {
                    continue;
                }

                // Can't move
                this.FreezeShape();
                return true;
            }

            // Lower.
            for (var i = 0; i < this.ActiveShape.Width; i++)
            {
                this.ActiveShape.Bottom[i] -= 1;
            }

            return false;
        }

        public override string ToString()
        {
            return $"[ {string.Join(", ", this.Floor)} ]";

        }

        private void FreezeShape()
        {
            for (var i = 0; i < this.ActiveShape.Width; i++)
            {
                for (var h = 0; h < this.ActiveShape.Height[i]; h++)
                {
                   this.Hollows[this.ActiveShape.Offset + i].Remove(this.ActiveShape.Bottom[i] + h);
                }
            }

            for (var i = 0; i < this.ActiveShape.Width; i++)
            {
                var floorLevel = this.Floor[this.ActiveShape.Offset + i];
                var rockLevel = this.ActiveShape.Bottom[i];


                if (rockLevel < floorLevel)
                {
                    // Hollow removal already done.
                    continue;
                }


                if (floorLevel < rockLevel - 1)
                {
                    for (var h = floorLevel + 1; h < rockLevel; h++)  
                    {
                        this.Hollows[this.ActiveShape.Offset + i].Add(h);
                    }
                }

                this.Floor[this.ActiveShape.Offset + i] = this.ActiveShape.Bottom[i] + this.ActiveShape.Height[i] - 1;
            }
            
            this.ActiveShape = null;
        }
    }
    
    internal partial class Program
    {
        static string Stringify(Chamber chamber, int n)
        {
            var activeShape = new List<(int row, int col)>();
            if (chamber.ActiveShape is not null)
            {
                for(var i = 0; i < chamber.ActiveShape.Width; i++)
                {
                    for (var h = 0; h < chamber.ActiveShape.Height[i]; h++)
                    {
                        activeShape.Add((chamber.ActiveShape.Bottom[i] + h, chamber.ActiveShape.Offset + i));
                    }
                }
            }

            var sb = new StringBuilder();


            for(var row = n; row >0; row--)
            {
                sb.Append("|");
                for (var i = 0; i < 7; i++)
                {
                    if (activeShape.Any(c => c.row == row && c.col == i))
                    {
                        sb.Append("@");
                    }
                    else if (chamber.Hollows[i].Contains(row) || chamber.Floor[i] == 0 || chamber.Floor[i] < row )
                    {
                        sb.Append(".");
                    }
                    else
                    {
                        sb.Append("#");
                    }
                }
                sb.AppendLine("|");
            }

            sb.AppendLine("+-------+");
            sb.AppendLine("");

            return sb.ToString();
        }

        static void Main(string[] args)
        {
            var input = Input.Load();

            var chamber = new Chamber();

            var spawned = 0;
            var jetIndex = 0;

            Int64 steps = 200000;
            // steps = 1_000_000_000_000;

            var sw = Stopwatch.StartNew();

            while (spawned < steps)
            {
                chamber.SpawnShape();
                spawned++;
                if (spawned % 100_000 == 0)
                {
                    Console.WriteLine($"{(double)spawned / steps} (in {sw.ElapsedMilliseconds / 1000})");
                }

                do
                {
                    var jet = input[jetIndex++];
                    jetIndex = jetIndex % input.Length;
                    switch (jet)
                    {
                        case '<': chamber.MoveLeft(); break;
                        case '>': chamber.MoveRight(); break;
                    }
                }
                while (!chamber.MoveDown());
            }

            File.WriteAllText("out.txt", Stringify(chamber, 200000));
            Console.WriteLine($"Part 1: {chamber.Floor.Max()}");
            Console.ReadKey();
        }
    }


}