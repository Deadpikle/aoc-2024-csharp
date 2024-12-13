class Machine
{
    public Button A;
    public Button B;
    public int PrizeX;
    public int PrizeY;

    public const int CostA = 3;
    public const int CostB = 1;

    public Machine(Button a, Button b, int x, int y)
    {
        A = a;
        B = b;
        PrizeX = x;
        PrizeY = y;
        SolutionCache = (-1, -1);
        dict = new Dictionary<(int, int), bool>();
    }

    private (int, int) SolutionCache;

    private Dictionary<(int, int), bool> dict;

    private (int, int) CheckSolution(int currATimes, int currBTimes)
    {
        // I think something is wrong with my base cases here; I shouldn't need this dict
        if (dict.ContainsKey((currATimes, currBTimes)))
        {
            return (int.MaxValue, int.MaxValue);
        }
        dict.Add((currATimes, currBTimes), true);
        if (currATimes == int.MaxValue || currATimes > 100 || currBTimes > 100)
        {
            return (int.MaxValue, int.MaxValue);
        }
        var currX = A.XDelta * currATimes + B.XDelta * currBTimes;
        var currY = A.YDelta * currATimes + B.YDelta * currBTimes;
        //Console.WriteLine("      currX = {0}, currY = {1}, PrizeX = {2}, PrizeY = {3}", currX, currY, PrizeX, PrizeY);
        //Console.WriteLine("      A.XDelta = {0}, A.YDelta = {1}", A.XDelta, A.YDelta);
        //Console.WriteLine("      B.XDelta = {0}, B.YDelta = {1}", B.XDelta, B.YDelta);
        if (currX > PrizeX || currY > PrizeY)
        {
            // Console.WriteLine("Exceed");
            return (int.MaxValue, int.MaxValue);
        }
        if (currX == PrizeX && currY == PrizeY)
        {
            // Console.WriteLine("SOLVE");
            return (currATimes, currBTimes);
        }
        if (currX < PrizeX && currY < PrizeY)
        {

        }
        var increaseASol = CheckSolution(currATimes + 1, currBTimes);
        var increaseBSol = CheckSolution(currATimes, currBTimes + 1);
        if (increaseASol.Item1 == int.MaxValue && increaseBSol.Item2 == int.MaxValue)
        {
            // Console.WriteLine("both sols were invalid");
            return (int.MaxValue, int.MaxValue);
        }
        // else one of them is a solution
        if (increaseASol.Item1 != int.MaxValue && increaseBSol.Item1 != int.MaxValue)
        {
            // they are both solutions
            var costA = increaseASol.Item1 * CostA + increaseASol.Item2 * CostB;
            var costB = increaseBSol.Item1 * CostA + increaseBSol.Item2 * CostB;
            if (costA < costB)
            {
                return increaseASol;
            }
            return increaseBSol;
        }
        else if (increaseASol.Item1 != int.MaxValue)
        {
            return increaseASol;
        }
        else// if (increaseBSol.Item1 != int.MaxValue)
        {
            return increaseBSol;
        }
    }

    public bool IsSolvable()
    {
        if (SolutionCache.Item1 == -1)
        {
            SolutionCache = CheckSolution(0, 0);
        }
        return SolutionCache.Item1 != int.MaxValue;
    }

    public (int, int) GetLeastCostSolution()
    {
        return SolutionCache;
    }

    public int GetLeastCost()
    {
        return SolutionCache.Item1 * CostA + SolutionCache.Item2 * CostB;
    }
}