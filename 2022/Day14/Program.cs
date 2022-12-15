using AdventOfCode;
using AdventOfCode.Grids;
using System.Diagnostics;

internal enum MaterialType
{
    Air,
    Rock,
    Sand,
    Void,
}

internal class Material : IHaveCoordinates
{
    public Material(MaterialType type)
    {
        this.Type = type;
    }

    public Coordinates Coordinates { get; set; } = new(-1, -1);
    public MaterialType Type { get; set; }

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
    private static List<Material> LoadObjects(string inputFilePath)
    {
        var objects = new List<Material>();

        Input.Process(line =>
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
                        objects.Add(new Material(MaterialType.Rock) { Coordinates = new(y, x) });
                    }
                }
                else
                {
                    var x = from.X;
                    for (var y = Math.Min(from.Y, to.Y); y <= Math.Max(from.Y, to.Y); y++)
                    {
                        objects.Add(new Material(MaterialType.Rock) { Coordinates = new(y, x) });
                    }
                }
            }
        }, inputFilePath);

        return objects;
    }

    private static void Main(string[] args)
    {
        // Load input data
        var part2 = true;
        var objects = LoadObjects("input.txt");

        // Set cave bounds
        var minX = objects.Select(o => o.Coordinates.X).Min();
        var maxX = objects.Select(o => o.Coordinates.X).Max();
        var maxY = objects.Select(o => o.Coordinates.Y).Max();

        var requiredWidth = maxX - minX + 1;
        var requiredHeight = maxY + 1;

        var additionalRows = 1;
        var additionalColumns = 2;

        var missingFromLeft = 0;
        var missingFromRight = 0;

        /* Part 2 */
        if (part2)
        {
            additionalRows += 1;
            var pileWidth = 0;
            var levelWidth = 1;
            for (var i = 0; i < maxY + 2; i++)
            {
                pileWidth += levelWidth;
                levelWidth += 2;
            }

            var pileWidthHalf = pileWidth / 2 + 1;
            missingFromRight = pileWidthHalf - (maxX - 500);
            missingFromLeft = pileWidthHalf - (500 - minX);
        }
        
        // Initialize the Cave
        var cave = new ArrayGrid<Material>(requiredHeight + additionalRows, requiredWidth + additionalColumns + missingFromLeft + missingFromRight);
        
        //  Fill it with air
        cave.Fill((y, x) => new Material(MaterialType.Air) { Coordinates = new(y, x) });

        // Offset the rocks
        var xOffset = -minX + additionalColumns / 2 + missingFromLeft;
        var yOffset = 0;
        objects.ForEach(o => { o.Coordinates.X += xOffset; o.Coordinates.Y += yOffset; });

        // Place the rocks in the cave
        objects.ToList().ForEach(o => cave.Set(o.Coordinates.Y, o.Coordinates.X, o));
              
        if (part2)
        {
            // Create the 'infinite' bedrock
            cave.Nodes.Where(n => n.Coordinates.Y == maxY + 2).ToList().ForEach(n => n.Type = MaterialType.Rock);

            var counter = 0;
            var startingX = 500 + xOffset;
            var rows = maxY + 2;
            var rowCount = 1;
            for (var yPos = 0; yPos < rows; yPos++)
            {
                for (var xPos = startingX; xPos < startingX + rowCount; xPos++)
                {
                    if (yPos == 0)
                    {
                        cave.Set(yPos, xPos, new Material(MaterialType.Sand));
                        counter++;
                        continue;
                    }

                    if (
                        (cave.Get(yPos - 1, xPos - 1).Is(MaterialType.Rock) || cave.Get(yPos - 1, xPos - 1).Is(MaterialType.Air)) &&
                        (cave.Get(yPos - 1, xPos).Is(MaterialType.Rock) || cave.Get(yPos - 1, xPos).Is(MaterialType.Air)) &&
                        (cave.Get(yPos - 1, xPos + 1).Is(MaterialType.Rock) || cave.Get(yPos - 1, xPos + 1).Is(MaterialType.Air))
                    )
                    {
                        continue;
                    }

                    if (cave.Get(yPos, xPos).Is(MaterialType.Rock))
                    {
                        continue;
                    }

                    cave.Set(yPos, xPos, new Material(MaterialType.Sand));
                    counter++;
                }

                startingX -= 1;
                rowCount += 2;
            }

            Console.WriteLine($"Part 2: {counter}");
            return;
        }

        // Create the void
        cave.Nodes.Where(n => n.Coordinates.Y >= maxY && n.Type == MaterialType.Air).ToList().ForEach(n => cave.Set(n.Coordinates.Y, n.Coordinates.X, new Material(MaterialType.Void)));
        
        // Spawner
        var spawner = new Coordinates(0, 500 + xOffset);
        var sandLost = false;
        Sand.FellIntoTheVoid += () => sandLost = true;
        var path = new Coordinates[] { null, null, null, null };

        if (!part2)
        {
            Console.WriteLine(cave);
        }

        while (!sandLost)
        {
            var sand = new Sand();
            if (path[2] is not null)
            {
                cave.Set(path[2].Y, path[2].X, sand);
                path = new[] { null, null, null, path[2] };
            }
            else
            {
                cave.Set(spawner.Y, spawner.X, sand);
            }

            while (!sand.IsStable && !sandLost)
            {
                sand.Update(cave);
                if (!sand.IsStable)
                {
                    path[0] = path[1];
                    path[1] = path[2];
                    path[2] = path[3];
                    path[3] = sand.Coordinates;
                }
            }
        }

        if (!part2)
        {
            Console.WriteLine(cave);
        }

        Console.WriteLine($"Part 1: {cave.Nodes.OfType<Sand>().Count(s => s.IsStable)}");
    }
}