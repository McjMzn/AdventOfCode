using AdventOfCode;
using AdventOfCode.Grids;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace Day15
{
    internal abstract class Device : IHaveCoordinates
    {
        public Coordinates Coordinates { get; set; }
    }

    internal class Beacon : Device
    {
        public override string ToString()
        {
            return $"B{this.Coordinates}";
        }
    }

    internal class Sensor : Device
    {
        private Beacon closestBeacon;
        private int beaconDeltaX;
        private int beaconDeltaY;
        private int distanceToClosestBeacon;

        public Beacon ClosestBeacon
        {
            get => this.closestBeacon;
            set
            {
                this.closestBeacon = value;
                this.beaconDeltaX = Math.Abs(this.Coordinates.X - value.Coordinates.X);
                this.beaconDeltaY = Math.Abs(this.Coordinates.Y - value.Coordinates.Y);
                this.distanceToClosestBeacon = this.beaconDeltaX + beaconDeltaY;
            }
        }


        public int DetectionRangeStartX => this.Coordinates.X - this.distanceToClosestBeacon;
        public int DetectionRangeEndX => this.Coordinates.X + this.distanceToClosestBeacon;

        public int DetectionRangeStartY => this.Coordinates.Y - this.distanceToClosestBeacon;
        public int DetectionRangeEndY => this.Coordinates.Y + this.distanceToClosestBeacon;


        public bool ExcludesBeaconAt(Coordinates coordinates)
        {
            if (coordinates == this.closestBeacon.Coordinates)
            {
                return false;
            }

            var deltaX = Math.Abs(this.Coordinates.X - coordinates.X);
            var deltaY = Math.Abs(this.Coordinates.Y - coordinates.Y);

            return (deltaX + deltaY) <= (this.beaconDeltaX + this.beaconDeltaY);
        }

        public List<Coordinates> GetOutline()
        {
            var outline = new HashSet<Coordinates>();

            var d = this.distanceToClosestBeacon + 1;
            for (var i = 0; i <= d; i++)
            {
                var increasing = i;
                var decreasing = (d - i);

                outline.Add(new Coordinates(this.Coordinates.Y + decreasing, this.Coordinates.X + increasing));
                outline.Add(new Coordinates(this.Coordinates.Y - increasing, this.Coordinates.X + decreasing));
                outline.Add(new Coordinates(this.Coordinates.Y - decreasing, this.Coordinates.X - increasing));
                outline.Add(new Coordinates(this.Coordinates.Y + increasing, this.Coordinates.X - decreasing));
            }

            return outline.ToList();
        }

        public override string ToString()
        {
            return $"S{this.Coordinates}";
        }
    }


    internal class Program
    {
        static void Main(string[] args)
        {
            var devices = new List<Device>();

            Input.Process(line =>
            {
                var match = Regex.Match(line, @"Sensor at x=(?<sensorX>-?\d+), y=(?<sensorY>-?\d+): closest beacon is at x=(?<beaconX>-?\d+), y=(?<beaconY>-?\d+)");
                var beacon = new Beacon { Coordinates = new(int.Parse(match.Groups["beaconY"].Value), int.Parse(match.Groups["beaconX"].Value)) };
                var sensor = new Sensor { Coordinates = new(int.Parse(match.Groups["sensorY"].Value), int.Parse(match.Groups["sensorX"].Value)) };
                sensor.ClosestBeacon = beacon;

                devices.Add(sensor);
                if (devices.OfType<Beacon>().All(b => b.Coordinates != beacon.Coordinates))
                {
                    devices.Add(beacon);
                }
            });

            var minY = devices.OfType<Sensor>().Select(d => d.DetectionRangeStartY).Min();
            var maxY = devices.OfType<Sensor>().Select(d => d.DetectionRangeEndY).Max();
            var minX = devices.OfType<Sensor>().Select(d => d.DetectionRangeStartX).Min();
            var maxX = devices.OfType<Sensor>().Select(d => d.DetectionRangeEndX).Max();

            ulong excluded = 0;

            for (var x = minX; x <= maxX; x++)
            {
                var y = 2000000;
                var coordinates = new Coordinates(y, x);
                foreach (var device in devices)
                {
                    if (device is Beacon beacon && beacon.Coordinates == coordinates)
                    {
                        // A beacon is already there.
                        break;
                    }

                    if (device is not Sensor sensor)
                    {
                        continue;
                    }

                    if (sensor.Coordinates == coordinates)
                    {
                        // A sensor excludes a beacon from being there.
                        excluded++;
                        break;
                    }

                    if (sensor.ExcludesBeaconAt(coordinates))
                    {
                        excluded++;
                        break;
                    }
                }
            }

            Console.WriteLine($"Part 1: {excluded}");

            ulong distress = 0;
            var sensors = devices.OfType<Sensor>().ToList();
            var coordinatesLimit = 4000000;

            var outlines =
                devices
                .OfType<Sensor>()
                .SelectMany(s => s.GetOutline())
                .Where(c =>
                    c.X >= 0 &&
                    c.X <= coordinatesLimit &&
                    c.Y >= 0 &&
                    c.Y <= coordinatesLimit
                ).ToList();

            var totalCoordinates = outlines.Count;

            Parallel.ForEach(outlines, coordinates =>
            {
                if (devices.All(d => d.Coordinates != coordinates) && sensors.All(s => !s.ExcludesBeaconAt(coordinates)))
                {
                    distress = (ulong)4000000 * (ulong)coordinates.X + (ulong)coordinates.Y;
                    Console.WriteLine($"Part 2: {distress}");
                    return;
                }
            });
        }
    }
}