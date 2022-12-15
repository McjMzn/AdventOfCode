using AdventOfCode;
using AdventOfCode.Grids;
using System.Diagnostics;
using System.Text;

internal enum MaterialType
{
    Air,
    Rock,
    Sand,
    Void,
    Spawner,
    ThickAir
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
            MaterialType.Spawner => "+",
            MaterialType.ThickAir => "@",
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

                    var length = Math.Abs(to.X - from.X) + 1;
                    if (length >= 3)
                    {
                        for (var subLevel = 1; subLevel <= length - 2; subLevel++)
                        {
                            var subFrom = Math.Min(from.X, to.X) + subLevel;
                            var subCount = length - (subLevel * 2);
                            for (var subX = 0; subX < subCount; subX++)
                            {
                                objects.Add(new Material(MaterialType.ThickAir) { Coordinates = new(from.Y + subLevel, subFrom + subX) });
                            }
                        }
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
        var objects = LoadObjects("input.txt");

        // Set cave bounds
        var minX = objects.Where(o => !o.Is(MaterialType.ThickAir)).Select(o => o.Coordinates.X).Min();
        var maxX = objects.Where(o => !o.Is(MaterialType.ThickAir)).Select(o => o.Coordinates.X).Max();
        var maxY = objects.Where(o => !o.Is(MaterialType.ThickAir)).Select(o => o.Coordinates.Y).Max();

        var requiredWidth = maxX - minX + 1;
        var requiredHeight = maxY + 1;

        var additionalRows = 1;
        var additionalColumns = 2;

        var missingFromLeft = 0;
        var missingFromRight = 0;

        /* Part 2 */
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
        /* End of Part 2 */

        // Initialize the Cave


        var cave = new ArrayGrid<Material>(requiredHeight + additionalRows, requiredWidth + additionalColumns + missingFromLeft + missingFromRight);
        
        //  Fill it with air
        cave.Initialize((y, x) => new Material(MaterialType.Air) { Coordinates = new(y, x) });

        // Offset the rocks
        var xOffset = -minX + additionalColumns / 2 + missingFromLeft;
        var yOffset = 0;

        objects.ForEach(o =>
        {
            o.Coordinates.X += xOffset;
            o.Coordinates.Y += yOffset;
        });

        // Place the rocks
        objects.Where(o => !o.Is(MaterialType.ThickAir)).ToList().ForEach(o => cave.Set(o.Coordinates.Y, o.Coordinates.X, o));
        //objects.Where(o => o.Is(MaterialType.ThickAir)).ToList().ForEach(o =>
        //{
        //    if (o.Coordinates.Y > maxY + 2)
        //    {
        //        return;
        //    }

        //    var material = cave.Get(o.Coordinates.Y, o.Coordinates.X);
        //    if (material.Is(MaterialType.Air))
        //    {
        //        cave.Set(o.Coordinates.Y, o.Coordinates.X, o);
        //    }
        //});


        

        // Create the void
        //cave.Nodes.Where(n => n.Coordinates.Y >= maxY && n.Type == MaterialType.Air).ToList().ForEach(n => cave.Set(n.Coordinates.Y, n.Coordinates.X, new Material(MaterialType.Void)));

        // Create the infinite bedrock
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

        var c = cave.Nodes.Count(c => c.Is(MaterialType.Spawner)); 
        // Spawner
        var spawner = new Coordinates(0, 500 + xOffset);

        // Console.WriteLine(cave);
        var stopwatch = Stopwatch.StartNew();
        var sandLost = false;
        Sand.FellIntoTheVoid += () => sandLost = true;
        var path = new Coordinates[] { null, null, null, null };
        
        while (!sandLost)
        {
            var spawnPointMaterial = cave.Get(spawner.Y, spawner.X);
            if (spawnPointMaterial.Is(MaterialType.Sand))
            {
                // part 2 result
                var res = cave.Nodes.OfType<Sand>().Count(s => s.IsStable);
                stopwatch.Stop();
                Console.WriteLine($"Measured {res} sand drops in {stopwatch.ElapsedMilliseconds} ms.");
                var xx = 3;
            }

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

            // Console.WriteLine($"{Environment.NewLine}{cave}");

            // var sand = cave.Nodes.OfType<Sand>().Single(s => !s.IsStable);
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

                // Console.WriteLine($"{Environment.NewLine}{cave}");
            }

            var tempRes = cave.Nodes.OfType<Sand>().Count(s => s.IsStable);
        }

        // Console.WriteLine($"Part 1: {cave.Nodes.OfType<Sand>().Count(s => s.IsStable)}"); // 24 for demo
        var x = 3;
    }
}