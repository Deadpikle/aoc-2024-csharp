using System.Numerics;

class Machine
{
    public Button A;
    public Button B;
    public BigInteger PrizeX;
    public BigInteger PrizeY;

    public const int CostA = 3;
    public const int CostB = 1;

    private bool IsPart2 = false;

    public Machine(Button a, Button b, BigInteger x, BigInteger y, bool isPart2 = false)
    {
        A = a;
        B = b;
        PrizeX = x + (isPart2 ? 10000000000000 : 0);
        PrizeY = y + (isPart2 ? 10000000000000 : 0);
        SolutionCache = (-1, -1);
        dict = new Dictionary<(BigInteger, BigInteger), bool>();
        IsPart2 = isPart2;
    }

    private (BigInteger, BigInteger) SolutionCache;

    private Dictionary<(BigInteger, BigInteger), bool> dict;

    private (BigInteger, BigInteger) CheckSolution(BigInteger currATimes, BigInteger currBTimes)
    {
        // Use dict for cache as we will hit same cases multiple times
        if (dict.ContainsKey((currATimes, currBTimes)))
        {
            return (-1, -1);
        }
        dict.Add((currATimes, currBTimes), true);
        if (currATimes == -1 || (!IsPart2 && (currATimes > 100 || currBTimes > 100)))
        {
            return (-1, -1);
        }
        var currX = A.XDelta * currATimes + B.XDelta * currBTimes;
        var currY = A.YDelta * currATimes + B.YDelta * currBTimes;
        //Console.WriteLine("      currX = {0}, currY = {1}, PrizeX = {2}, PrizeY = {3}", currX, currY, PrizeX, PrizeY);
        //Console.WriteLine("      A.XDelta = {0}, A.YDelta = {1}", A.XDelta, A.YDelta);
        //Console.WriteLine("      B.XDelta = {0}, B.YDelta = {1}", B.XDelta, B.YDelta);
        if (currX > PrizeX || currY > PrizeY)
        {
            return (-1, -1);
        }
        if (currX == PrizeX && currY == PrizeY)
        {
            // Console.WriteLine("SOLVE");
            return (currATimes, currBTimes);
        }
        var increaseASol = CheckSolution(currATimes + 1, currBTimes);
        var increaseBSol = CheckSolution(currATimes, currBTimes + 1);
        if (increaseASol.Item1 == -1 && increaseBSol.Item2 == -1)
        {
            // Console.WriteLine("both sols were invalid");
            return (-1, -1);
        }
        // else one of them is a solution
        if (increaseASol.Item1 != -1 && increaseBSol.Item1 != -1)
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
        else if (increaseASol.Item1 != -1)
        {
            return increaseASol;
        }
        else// if (increaseBSol.Item1 != -1)
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
        return SolutionCache.Item1 != -1;
    }

    public (BigInteger, BigInteger) GetLeastCostSolution()
    {
        return SolutionCache;
    }

    public BigInteger GetLeastCost()
    {
        return SolutionCache.Item1 * CostA + SolutionCache.Item2 * CostB;
    }
}