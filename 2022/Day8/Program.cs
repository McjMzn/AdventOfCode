using AdventOfCode;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.TwentyTwentyTwo.Day8
{
    internal class Tree
    {
        public Tree(int height)
        {
            this.Height = height;
            this.IsVisible = false;
        }

        public int Row { get; set; }
        public int Column { get; set; }
        public int Height { get; set; }
        public bool IsVisible { get; set; }

        public int ScenicScore { get; set; } = 0;

        public override string ToString()
        {
            return $"({this.Row},{this.Column}){(this.IsVisible ? "*" : "")} {this.Height}";
        }
    }

    internal enum Direction
    {
        Left,
        Right,
        Up,
        Down
    }

    internal class Forest
    {
        private int? numberOfRows = null;
        private int? numberOfColumns = null;

        public int NumberOfRows
        {
            get
            {
                numberOfRows ??= this.Trees.DistinctBy(t => t.Row).Count();
                return numberOfRows.Value;
            }
        }
        public int NumberOfColumns
        {
            get
            {
                numberOfColumns ??= this.Trees.DistinctBy(t => t.Column).Count();
                return numberOfColumns.Value;
            }
        
        }

        public List<Tree> Trees { get; set; } = new();

        public void CalculateScenicScores()
        {
            foreach (var tree in this.Trees)
            {
                var left = this.CheckViewDistance(tree, Direction.Left);
                var right = this.CheckViewDistance(tree, Direction.Right);
                var up = this.CheckViewDistance(tree, Direction.Up);
                var down = this.CheckViewDistance(tree, Direction.Down);

                tree.ScenicScore = left * right * up * down;
            }
        }

        private Tree GetFromLeft(Tree tree) => tree.Column == 0 ? null : this.GetByPosition(tree.Row, tree.Column - 1);
        private Tree GetFromRight(Tree tree) => tree.Column == this.NumberOfColumns - 1 ? null : this.GetByPosition(tree.Row, tree.Column + 1);
        private Tree GetFromUp(Tree tree) => tree.Row == 0 ? null : this.GetByPosition(tree.Row - 1, tree.Column);
        private Tree GetFromDown(Tree tree) => tree.Row == this.NumberOfRows - 1 ? null : this.GetByPosition(tree.Row + 1, tree.Column);
        
        private int CheckViewDistance(Tree tree, Direction direction)
        {
            var steps = 0;
            var neighbour = tree;
            do
            {
                neighbour = direction switch
                {
                    Direction.Left => this.GetFromLeft(neighbour),
                    Direction.Right => this.GetFromRight(neighbour),
                    Direction.Down => this.GetFromDown(neighbour),
                    Direction.Up => this.GetFromUp(neighbour)
                };

                if (neighbour is null)
                {
                    break;
                }

                steps++;
            } while (neighbour.Height < tree.Height);

            return steps;
        }

        private Tree GetByPosition(int row, int column)
        {
            return this.Trees.First(t => t.Row == row && t.Column == column);
        }
    }

    internal class Program
    {
        static List<Tree> SetVisibility(List<Tree> traversed, Tree current)
        {
            if (!current.IsVisible)
            {
                current.IsVisible = traversed.All(t => t.Height < current.Height);
            }

            traversed.Add(current);

            return traversed;
        }

        static void Main(string[] args)
        {
            var forest = new Forest();

            var row = 0;
            var column = 0;
            Input.Process(line =>
            {
                var trees = line.ToCharArray().Select(c => new Tree(c - '0') { Column = column++, Row = row}).ToList();
                forest.Trees.AddRange(trees);

                column = 0;
                row++;
            });

            // Part 1
            forest.Trees.Where(t => t.Row == 0 || t.Row == forest.NumberOfRows - 1 || t.Column == 0 || t.Column == forest.NumberOfColumns - 1).ToList().ForEach(tree => tree.IsVisible = true);

            for(var i = 0; i < forest.NumberOfRows; i++)
            {
                var r = forest.Trees.Where(t => t.Row == i);
                r.OrderBy(r => r.Column).Aggregate(new List<Tree>(), (traversed, current) => SetVisibility(traversed, current));
                r.OrderByDescending(r => r.Column).Aggregate(new List<Tree>(), (traversed, current) => SetVisibility(traversed, current));
            }

            for(var i = 0; i < forest.NumberOfColumns; i++)
            {
                var c = forest.Trees.Where(t => t.Column == i);
                c.OrderBy(c => c.Row).Aggregate(new List<Tree>(), (traversed, current) => SetVisibility(traversed, current));
                c.OrderByDescending(c => c.Row).Aggregate(new List<Tree>(), (traversed, current) => SetVisibility(traversed, current));
            }

            Console.WriteLine($"Part 1: {forest.Trees.Count(t => t.IsVisible)}");

            // Part 2
            forest.CalculateScenicScores();
            Console.WriteLine($"Part 2: {forest.Trees.Select(t => t.ScenicScore).Max()}");
        }
    }
}