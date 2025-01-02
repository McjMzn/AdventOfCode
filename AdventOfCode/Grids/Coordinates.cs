using System;

namespace AdventOfCode.Grids
{
    public record Coordinates
    {
        public Coordinates(int y, int x)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }

        public int Y { get; set; }

        public override string ToString()
        {
            return $"({this.Y}, {this.X})";
        }

        public double Distance(Coordinates other)
        {
            return Math.Sqrt(Math.Pow(X - other.X, 2) + Math.Pow(Y - other.Y, 2));
        }

        public Vector2d VectorTo (Coordinates other)
        {
            return (other.Y - Y, other.X - X);
        }

        public Coordinates Translated(Vector2d vector)
        {
            return new Coordinates(Y + vector.Y, X + vector.X);
        }

        public static implicit operator Coordinates((int Y, int X) tuple)
        {
            return new Coordinates(tuple.Y, tuple.X);
        }
    }

    public record Vector2d(int Y, int X)
    {
        public static Vector2d Up => (-1, 0);

        public static Vector2d Down => (1, 0);

        public static Vector2d Right => (0, 1);

        public static Vector2d Left => (0, -1);

        public Vector2d TurnedRight() => (X, -Y);

        public Vector2d TurnedLeft() => (-X, Y);

        public override string ToString()
        {
            return $"({this.Y}, {this.X})";
        }

        public static Vector2d operator +(Vector2d left, Vector2d right)
        {
            return (left.Y + right.Y, left.X + right.X);
        }

        public static Vector2d operator -(Vector2d left, Vector2d right)
        {
            return (left.Y - right.Y, left.X - right.X);
        }

        public static implicit operator Vector2d((int Y, int X) tuple)
        {
            return new Vector2d(tuple.Y, tuple.X);
        }
    }
}
