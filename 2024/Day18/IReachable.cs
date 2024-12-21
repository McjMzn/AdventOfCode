namespace Day18
{
    internal interface IReachable
    {
        long Distance { get; set; }

        bool IsVisited { get; set; }
    }
}
