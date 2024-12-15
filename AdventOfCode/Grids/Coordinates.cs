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
        public override string ToString()
        {
            return $"({this.Y}, {this.X})";
        }

        public static implicit operator Vector2d((int Y, int X) tuple)
        {
            return new Vector2d(tuple.Y, tuple.X);
        }
    }
}
