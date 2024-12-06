using System;
using System.Collections.Generic;
using System.Linq;

namespace AdventOfCode.Grids
{
    [Flags]
    public enum AllowedDirections
    {
        None = 0,

        Up = 1,
        Down = 2,
        Left = 4,
        Right = 8,
        UpperLeft = 16,
        UpperRight = 32,
        LowerLeft = 64,
        LowerRight = 128,
        
        All = Up | Down | Left | Right | UpperLeft | UpperRight | LowerLeft | LowerRight,
    }

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

        public T GetUpperLeft(T node)
        {
            (var y, var x) = GetIndices(node);
            return Get(y - 1, x - 1);
        }

        public T GetUpperRight(T node)
        {
            (var y, var x) = GetIndices(node);
            return Get(y - 1, x + 1);
        }

        public List<T> GetDirectNeighbours(T node)
        {
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

        public List<T> GetAllNeighbours(T node)
        {
            return
                new List<T>
                {
                    GetLeftFrom(node),
                    GetRightFrom(node),
                    GetUpFrom(node),
                    GetDownFrom(node),

                    GetUpperLeft(node),
                    GetUpperRight(node),
                    GetLowerLeft(node),
                    GetLowerRight(node),
                }
                .Where(n => n is not null)
                .ToList();
        }

        public List<T> GetNeighbours(int y, int x, AllowedDirections allowedDirections)
        {
            return GetNeighboursIndices(y, x, allowedDirections).Select(n => Get(n.Y, n.X)).ToList();
        }

        public List<(int Y, int X, AllowedDirections Direction)> GetNeighboursIndices(int y, int x, AllowedDirections allowedDirections = AllowedDirections.All)
        {
            var neighbours = new List<(int Y, int X, AllowedDirections direction)>();
            if (allowedDirections.HasFlag(AllowedDirections.Left))
            {
                neighbours.Add((y, x - 1, AllowedDirections.Left));
            }

            if (allowedDirections.HasFlag(AllowedDirections.Right))
            {
                neighbours.Add((y, x + 1, AllowedDirections.Right));
            }

            if (allowedDirections.HasFlag(AllowedDirections.Up))
            {
                neighbours.Add((y - 1, x, AllowedDirections.Up));
            }

            if (allowedDirections.HasFlag(AllowedDirections.Down))
            {
                neighbours.Add((y + 1, x, AllowedDirections.Down));
            }

            if (allowedDirections.HasFlag(AllowedDirections.UpperLeft))
            {
                neighbours.Add((y - 1, x - 1, AllowedDirections.UpperLeft));
            }

            if (allowedDirections.HasFlag(AllowedDirections.UpperRight))
            {
                neighbours.Add((y - 1, x + 1, AllowedDirections.UpperRight));
            }

            if (allowedDirections.HasFlag(AllowedDirections.LowerLeft))
            {
                neighbours.Add((y + 1, x - 1, AllowedDirections.LowerLeft));
            }

            if (allowedDirections.HasFlag(AllowedDirections.LowerRight))
            {
                neighbours.Add((y + 1, x + 1, AllowedDirections.LowerRight));
            }

            return neighbours;
        }

        public abstract (int Y, int X) GetIndices(T node);
    }
}
