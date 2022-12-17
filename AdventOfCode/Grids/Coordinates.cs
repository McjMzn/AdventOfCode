using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Grids
{
    public record Coordinates
    {
        public int X { get; set; }

        public Coordinates(int y, int x)
        {
            X = x;
            Y = y;
        }

        public int Y { get; set; }

        public override string ToString()
        {
            return $"({this.Y}, {this.X})";
        }
    }
}
