using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Grids
{
    public interface IHaveCoordinates
    {
        Coordinates Coordinates { get; set; }
    }
}
