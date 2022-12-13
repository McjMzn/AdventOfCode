using AdventOfCode;
using AdventOfCode.Grids;
using System.Text;

internal enum MaterialType
{
    Air,
    Rock,
    Sand,
    Void,
    Spawner
}

internal class Material : IHaveCoordinates
{
    public Material(MaterialType type)
    {
        this.Type = type;
    }

    public int X { get; set; }
    public int Y { get; set; }
    public MaterialType Type { get; }

    public bool Is(MaterialType type)
    {
        return this.Type == type;
    }

    public override string ToString()
    {
        return this.Type switch
        {
            MaterialType.Air => ".",
            MaterialType.Rock => "#",
            MaterialType.Sand => "o",
            MaterialType.Void => "~",
            MaterialType.Spawner => "+",
        };
    }
}

internal class Sand : Material
{
    public static event Action FellIntoTheVoid;

    public Sand() : base(MaterialType.Sand)
    {
    }

    public bool IsStable { get; set; } = false;

    public void Update(ArrayGrid<Material> grid)
    {
        var under = grid.GetDownFrom(this);
        if (under.Is(MaterialType.Air))
        {
            grid.Swap(this, under);
            return;
        }

        var lowerLeft = grid.GetLowerLeft(this);
        if (lowerLeft.Is(MaterialType.Air))
        {
            grid.Swap(this, lowerLeft);
            return;
        }

        var lowerRight = grid.GetLowerRight(this);
        if (lowerRight.Is(MaterialType.Air))
        {
            grid.Swap(this, lowerRight);
            return;
        }

        if (under.Is(MaterialType.Void) || lowerRight.Is(MaterialType.Void) || lowerLeft.Is(MaterialType.Void))
        {
            FellIntoTheVoid?.Invoke();
        }
        else
        {
            this.IsStable = true;
        }
    }
}

internal class Program
{
    private static void Main(string[] args)
    {

        var objects = new List<Material>();

        Input.ProcessDemo(line =>
        {
            var points = line.Split("->").Select(p => p.Trim()).Select(p => p.Split(",")).Select(p => new { Y = int.Parse(p[1]), X = int.Parse(p[0]) }).ToList();

            for (var i = 0; i < points.Count - 1; i++)
            {
                var from = points[i];
                var to = points[i + 1];

                if (from.X != to.X)
                {
                    var y = from.Y;
                    for (var x = Math.Min(from.X, to.X); x <= Math.Max(from.X, to.X); x++)
                    {
                        objects.Add(new Material(MaterialType.Rock) { Y = y, X = x });
                    }
                }
                else
                {
                    var x = from.X;
                    for (var y = Math.Min(from.Y, to.Y); y <= Math.Max(from.Y, to.Y); y++)
                    {
                        objects.Add(new Material(MaterialType.Rock) { Y = y, X = x });
                    }
                }
            }
        });

        var minX = objects.Select(o => o.X).Min() - 1;
        var maxX = objects.Select(o => o.X).Max() + 1;
        var maxY = objects.Select(o => o.Y).Max();

        var maxWidth = 0;
        var level = 1;
        for (var i = 0; i < maxY; i++)
        {
            maxWidth += level;
            level += 2;
        }

        maxWidth = maxWidth % 2 == 0 ? maxWidth : maxWidth + 1;

        // <----0.5*max--- 500 ---0.5*max ---->
        // offset = 

        var additionalWidth = maxWidth;
        var xOffset = -minX;// (maxWidth / 2) - minX;



        var grid = new ArrayGrid<Material>(maxY + 1, maxX - minX + 1);
        grid.Initialize((y, x) => new Material(MaterialType.Air) { X = x, Y = y });
        // grid.Set(0, 500 + xOffset, new Material(MaterialType.Spawner) { Y = 0, X = 500 + xOffset });
        objects.ForEach(o => o.X = o.X + xOffset);
        objects.ForEach(o => grid.Set(o.Y, o.X, o));
        for (var i = 0; i < grid.NumberOfColumns; i++)
        {
            var material = grid.Get(maxY, i);
            if (material.Type is not MaterialType.Rock)
            {
                grid.Set(maxY, i, new Material(MaterialType.Void));
            }
        }

        var spawners = new List<Material> { new Material(MaterialType.Spawner) { Y = 0, X = 500 + xOffset } };// grid.Nodes.Where(n => n.Is(MaterialType.Spawner)).ToList();
        var sandLost = false;
        Sand.FellIntoTheVoid += () => sandLost = true;
        Console.WriteLine($"{grid}");

        var last = new (int Y, int X)?[4] { null, null, null, null };

        while (!sandLost)
        {
            foreach (var spawner in spawners)
            {
                // Console.WriteLine($"{Environment.NewLine}{grid}");
                var spawnPointMaterial = grid.Get(spawner.Y, spawner.X);
                //if (spawnPointMaterial.Is(MaterialType.Sand))
                //{
                //    var r = grid.Nodes.OfType<Sand>().Count(s => s.IsStable);
                //}

                if (last[1].HasValue)
                {
                    grid.Set(last[1].Value.Y, last[1].Value.X, new Sand());
                    last = new[] { null, null, null, last[1] };
                }
                else
                {
                    grid.Set(spawner.Y, spawner.X, new Sand());
                }
                
                Console.WriteLine($"{Environment.NewLine}{grid}");
                continue;
            }

            var sand = grid.Nodes.OfType<Sand>().Single(s => !s.IsStable);
            while (!sand.IsStable && !sandLost)
            {
                sand.Update(grid);
                if (!sand.IsStable)
                {
                    last[0] = last[1];
                    last[1] = last[2];
                    last[2] = last[3];
                    last[3] = (sand.Y, sand.X);
                }

                Console.WriteLine($"{Environment.NewLine}{grid}");
            }
        }

        // Console.WriteLine($"{Environment.NewLine}{grid}");
        Console.WriteLine($"Part 1: {grid.Nodes.OfType<Sand>().Count(s => s.IsStable)}"); // 24 for demo
        var x = 3;
    }
}