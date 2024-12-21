using AdventOfCode.Grids;

namespace Day18
{
    internal class DijkstraSolver
    {
        public static void Solve<T>(IDictionary<Coordinates, T> map)
            where T : IReachable
        {
            Solve(map, _ => true);
        }

        public static void Solve<T>(IDictionary<Coordinates, T> map, Predicate<T> isLegal)
            where T : IReachable
        {
            while (map.Any(kvp => !kvp.Value.IsVisited && isLegal(kvp.Value)))
            {
                var selected = map.Where(kvp => !kvp.Value.IsVisited).OrderBy(kvp => kvp.Value.Distance).FirstOrDefault();

                var node = selected.Value;
                var coordinates = selected.Key;
                node.IsVisited = true;

                List<Coordinates> neighbours = new Coordinates[] {
                    coordinates.Translated(Vector2d.Up),
                    coordinates.Translated(Vector2d.Down),
                    coordinates.Translated(Vector2d.Left),
                    coordinates.Translated(Vector2d.Right),
                 }
                .Where(c => map.ContainsKey(c) && isLegal(map[c]))
                .ToList();

                foreach (var neigbour in neighbours)
                {
                    if (node.Distance + 1 < map[neigbour].Distance)
                    {
                        map[neigbour].Distance = node.Distance + 1;
                    }
                }
            }
        }

        public static void SolveReachable<T>(IDictionary<Coordinates, T> map, Predicate<T> isLegal)
            where T : IReachable
        {
            var first = map.Where(kvp => kvp.Value.Distance == 0).Select(kvp => kvp.Key).First();

            var queue = new PriorityQueue<Coordinates, long>();
            queue.Enqueue(first, 0);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                if (map[current].IsVisited)
                {
                    continue;
                }

                map[current].IsVisited = true;

                List<Coordinates> neighbours = new Coordinates[] {
                    current.Translated(Vector2d.Up),
                    current.Translated(Vector2d.Down),
                    current.Translated(Vector2d.Left),
                    current.Translated(Vector2d.Right),
                 }
                .Where(c => map.ContainsKey(c) && isLegal(map[c]) && !map[c].IsVisited)
                .ToList();

                foreach (var neigbour in neighbours)
                {
                    if (map[current].Distance + 1 < map[neigbour].Distance)
                    {
                        map[neigbour].Distance = map[current].Distance + 1;
                    }

                    queue.Enqueue(neigbour, map[neigbour].Distance);
                }
            }
        }
    }
}
