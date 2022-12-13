using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Grids
{
    public interface IHaveCoordinates
    {
        int X { get; set; }
        int Y { get; set; }
    }
}
