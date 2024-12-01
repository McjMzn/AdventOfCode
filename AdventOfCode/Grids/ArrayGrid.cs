using System;
using System.Collections.Generic;
using System.Text;

namespace AdventOfCode.Grids
{
    public class ArrayGrid<T> : Grid<T> where T : class
    {
        private int elementsAdded = 0;

        private T[,] NodesArray;
        public int NumberOfRows { get; }
        public int NumberOfColumns { get; }

        public ArrayGrid(int rows, int columns)
        {
            this.NodesArray = new T[rows, columns];
            this.NumberOfRows = rows;
            this.NumberOfColumns = columns;
        }

        public void AddNext(T node)
        {
            var columns = this.NodesArray.GetLength(1);

            var column = elementsAdded % columns;
            var row = elementsAdded / columns;

            this.NodesArray[row, column] = node;

            elementsAdded++;
        }

        public void Fill(Func<int, int, T> objectFactory)
        {
            for (var y = 0; y < this.NumberOfRows; y++)
            {
                for (var x = 0; x < this.NumberOfColumns; x++)
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
                aWithCoordinates.Coordinates.X = bX;
                aWithCoordinates.Coordinates.Y = bY;
            }

            if (b is IHaveCoordinates bWithCoordinates)
            {
                bWithCoordinates.Coordinates.X = aX;
                bWithCoordinates.Coordinates.Y = aY;
            }
        }

        public override List<T> Nodes
        {
            get
            {
                var nodes = new List<T>();
                for (var y = 0; y < this.NodesArray.GetLength(0); y++)
                {
                    for (var x = 0; x < this.NodesArray.GetLength(1); x++)
                    {
                        nodes.Add(this.NodesArray[y, x]);
                    }
                }

                return nodes;
            }
        }

        public override T Set(int y, int x, T node)
        {
            this.NodesArray[y, x] = node;
            if (node is IHaveCoordinates nodeWithCoordinates)
            {
                nodeWithCoordinates.Coordinates.Y = y;
                nodeWithCoordinates.Coordinates.X = x;
            }

            return node;
        }

        public override T Get(int y, int x)
        {
            if (y < 0 || x < 0 || y > this.NumberOfRows - 1 || x > this.NumberOfColumns - 1)
            {
                return null;
            }

            return this.NodesArray[y, x];
        }

        public override (int Y, int X) GetIndices(T node)
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
