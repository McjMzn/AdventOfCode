using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Grids
{
    public class ListGrid<T> : Grid<T> where T : class
    {
        private List<T> nodes= new List<T>();
        
        public int Width { get; set; }
        public int Height { get; set; }

        public override IEnumerable<T> Nodes => this.nodes;

        public void Add(T node)
        {
            this.nodes.Add(node);
        }

        public override T Get(int y, int x)
        {
            if (y < 0 || y >= Height || x < 0 || x >= Width)
            {
                return null;
            }

            return this.nodes[y * Width + x];
        }

        public override (int Y, int X) GetIndices(T node)
        {
            var index = this.nodes.IndexOf(node);

            var y = index / Width;
            var x = index % Width;

            return (y, x);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var i = 1; i <= this.nodes.Count; i++)
            {
                builder.Append(this.nodes[i - 1].ToString());
                if (i % Width == 0)
                {
                    builder.Append(Environment.NewLine);
                }
            }

            return builder.ToString();
        }

        public override T Set(int y, int x, T node)
        {
            throw new NotImplementedException();
        }
    }
}
