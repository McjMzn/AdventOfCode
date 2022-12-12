using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdventOfCode
{
    public class Grid<T> where T : class
    {
        public List<T> Nodes { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }

        public T GetLeftFrom(T node)
        {
            (var y, var x) = this.GetIndices(node);
            return x == 0 ? null : this.Get(y, x - 1);
        }

        public T GetRightFrom(T node)
        {
            (var y, var x) = this.GetIndices(node);
            return x == this.Width - 1 ? null : this.Get(y, x + 1);
        }

        public T GetDownFrom(T node)
        {
            (var y, var x) = this.GetIndices(node);
            return y == 0 ? null : this.Get(y - 1, x);
        }

        public T GetUpFrom(T node)
        {
            (var y, var x) = this.GetIndices(node);
            return y == this.Height - 1 ? null : this.Get(y + 1, x);
        }

        public List<T> GetNeighbours(T node)
        {
            (var y, var x) = this.GetIndices(node);

            return
                new List<T>
                {
                    this.GetLeftFrom(node),
                    this.GetRightFrom(node),
                    this.GetUpFrom(node),
                    this.GetDownFrom(node),
                }
                .Where(n => n is not null)
                .ToList();
        }

        public T Get(int y, int x)
        {
            return this.Nodes[y * this.Width + x];
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var i = 1; i <= this.Nodes.Count; i++)
            {
                builder.Append(this.Nodes[i - 1].ToString());
                if (i % this.Width == 0)
                {
                    builder.Append(Environment.NewLine);
                }
            }

            return builder.ToString();
        }

        private (int Y, int X) GetIndices(T node)
        {
            var index = this.Nodes.IndexOf(node);

            var y = index / this.Width;
            var x = index % this.Width;

            return (y, x);
        }
    }
}
