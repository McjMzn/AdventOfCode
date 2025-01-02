namespace AdventOfCode.Pathfinding
{
    public interface IReachable
    {
        long Distance { get; set; }

        bool IsVisited { get; set; }
    }
}
