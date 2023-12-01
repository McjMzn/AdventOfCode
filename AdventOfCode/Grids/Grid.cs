using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Grids
{
    public abstract class Grid<T> where T : class
    {
        public abstract IEnumerable<T> Nodes { get; }

        public abstract T Get(int y, int x);
        
        public abstract T Set(int y, int x, T node);

        public T GetLeftFrom(T node)
        {
            (var y, var x) = GetIndices(node);
            return Get(y, x - 1);
        }

        public T GetRightFrom(T node)
        {
            (var y, var x) = GetIndices(node);
            return Get(y, x + 1);
        }

        public T GetDownFrom(T node)
        {
            (var y, var x) = GetIndices(node);
            return Get(y + 1, x);
        }

        public T GetLowerLeft(T node)
        {
            (var y, var x) = GetIndices(node);
            return Get(y + 1, x - 1);
        }

        public T GetLowerRight(T node)
        {
            (var y, var x) = GetIndices(node);
            return Get(y + 1, x + 1);
        }

        public T GetUpFrom(T node)
        {
            (var y, var x) = GetIndices(node);
            return Get(y - 1, x);
        }

        public List<T> GetDirectNeighbours(T node)
        {
            (var y, var x) = GetIndices(node);

            return
                new List<T>
                {
                    GetLeftFrom(node),
                    GetRightFrom(node),
                    GetUpFrom(node),
                    GetDownFrom(node),
                }
                .Where(n => n is not null)
                .ToList();
        }

        public abstract (int Y, int X) GetIndices(T node);
    }
}
