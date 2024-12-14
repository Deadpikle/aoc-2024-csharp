using System.Numerics;
using System.Runtime.InteropServices;

class Machine
{
    public Button A;
    public Button B;
    public long PrizeX;
    public long PrizeY;

    public const int CostA = 3;
    public const int CostB = 1;

    private bool IsPart2 = false;

    public Machine(Button a, Button b, long x, long y, bool isPart2 = false)
    {
        A = a;
        B = b;
        PrizeX = x + (isPart2 ? 10000000000000 : 0);
        PrizeY = y + (isPart2 ? 10000000000000 : 0);
        SolutionCache = (-1, -1);
        dict = new Dictionary<(long, long), bool>();
        IsPart2 = isPart2;
    }

    private (long, long) SolutionCache;

    private Dictionary<(long, long), bool> dict;

    private (long, long) CheckSolution(long currATimes, long currBTimes)
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
        return SolveViaMathEquations();
        //if (SolutionCache.Item1 == -1)
        //{
        //    SolutionCache = CheckSolution(0, 0);
        //}
        //return SolutionCache.Item1 != -1;
    }

    public bool SolveViaMathEquations()
    {
        if (SolutionCache.Item1 == -1)
        {
            /*
            HOW DID I NOT REALIZE THIS IS JUST A SET OF EQUATIONS!!!
            See: https://www.reddit.com/r/adventofcode/comments/1hd5b6o/2024_day_13_in_the_end_math_reigns_supreme/m1viocb/
            Button A: X+94, Y+34
            Button B: X+22, Y+67
            Prize: X=8400, Y=5400
            94x + 22y = 8400
            34x + 67y = 5400
            now solve the set of equations
            Ax + By = C
            Dx + Ey = F
            y = (C - Ax) / B
            y = (F - Dx) / E
            --two y equations equal each other--
            (C - Ax) / B = (F - Dx) / E
            BF - BDx = EC - EAx
            BF - EC - BDx = -EAx
            BF - EC = BDx - EAx
            (BF - EC) / (BD - EA) = x
            can get # for x now and solve for y:
            y = (c - a * x) / b
            */
            decimal a = A.XDelta;
            decimal b = B.XDelta;
            decimal c = PrizeX;
            decimal d = A.YDelta;
            decimal e = B.YDelta;
            decimal f = PrizeY;
            
            decimal x = (c * e - b * f) / (e * a - b * d);
            decimal y = (c - a * x) / b;
            if (x.Scale == 0 && y.Scale == 0)
            {
                SolutionCache = ((long)x, (long)y);
            }
            else
            {
                SolutionCache = (-1, -1);
            }
        }
        return SolutionCache.Item1 != -1;
    }

    public (long, long) GetLeastCostSolution()
    {
        return SolutionCache;
    }

    public long GetLeastCost()
    {
        return SolutionCache.Item1 * CostA + SolutionCache.Item2 * CostB;
    }
}