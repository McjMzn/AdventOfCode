using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Grids
{
    public class ArrayGrid<T> : Grid<T> where T : class
    {
        private T[,] NodesArray;
        public int NumberOfRows { get; }
        public int NumberOfColumns { get; }

        public ArrayGrid(int rows, int columns)
        {
            this.NodesArray = new T[rows, columns];
            this.NumberOfRows = rows;
            this.NumberOfColumns = columns;
        }

        public void Initialize(Func<int, int, T> objectFactory)
        {
            for(var y = 0; y < this.NumberOfRows; y++)
            {
                for(var x = 0; x < this.NumberOfColumns; x++)
                {
                    this.NodesArray[y, x] = objectFactory(y, x);
                }
            }
        }

        public void Swap(T a, T b)
        {
            var (aY, aX) = this.GetIndices(a);
            var (bY, bX) = this.GetIndices(b);

            this.Set(aY, aX, b);
            this.Set(bY, bX, a);

            if (a is IHaveCoordinates aWithCoordinates)
            {
                aWithCoordinates.X = bX;
                aWithCoordinates.Y = bY;
            }

            if (b is IHaveCoordinates bWithCoordinates)
            {
                bWithCoordinates.X = aX;
                bWithCoordinates.Y = aY;
            }
        }

        public override IEnumerable<T> Nodes { get { foreach (var item in this.NodesArray) yield return item; } }

        public override void Add(T node)
        {
            throw new NotImplementedException();
        }

        public override void Set(int y, int x, T node)
        {
            this.NodesArray[y, x] = node;
            if (node is IHaveCoordinates nodeWithCoordinates)
            {
                nodeWithCoordinates.Y = y;
                nodeWithCoordinates.X = x;
            }
        }

        public override T Get(int y, int x)
        {
            return this.NodesArray[y, x];
        }

        protected override (int Y, int X) GetIndices(T node)
        {
            for (var y = 0; y < this.NodesArray.GetLength(0); y++)
            {
                for (var x = 0; x < this.NodesArray.GetLength(1); x++)
                {
                    if (this.NodesArray[y, x] == node)
                    {
                        return (y, x);
                    }
                }
            }

            return (-1, -1);
        }

        public override string ToString()
        {
            var builder = new StringBuilder();
            for (var y = 0; y < this.NumberOfRows; y++)
            {
                for (var x = 0; x < this.NumberOfColumns; x++)
                {
                    builder.Append(this.NodesArray[y, x].ToString());
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }
    }
}
