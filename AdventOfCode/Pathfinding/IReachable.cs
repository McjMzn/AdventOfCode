namespace AdventOfCode.Pathfinding
{
    internal interface IReachable
    {
        long Distance { get; set; }

        bool IsVisited { get; set; }
    }
}
