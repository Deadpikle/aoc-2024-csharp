var fileLines = File.ReadAllLines("input.txt");
// var fileLines = File.ReadAllLines("example.txt");

const int ADD = 0;
const int MULT = 1;
const int COMBINE = 2;

long DoMath(List<long> nums, int[] operators, long maxSize)
{
    var tmpNums = new List<long>(nums);
    for (var i = 0; i < operators.Length; i++)
    {
        var result = 
            operators[i] == ADD ? tmpNums[0] + tmpNums[1] : 
            (operators[i] == MULT ? tmpNums[0] * tmpNums[1] : long.Parse(tmpNums[0].ToString() + tmpNums[1].ToString()));
        // Console.WriteLine("    {0} {1} {2} = {3}", tmpNums[0], 
        //     operators[i] == ADD ? '+' : (operators[i] == MULT ? '*' : '|'), tmpNums[1], result);
        if (result > maxSize)
        {
            return -1; // short circuit further work; this set of operators will not work
        }
        tmpNums[0] = result;
        tmpNums.RemoveAt(1);
    }
    return tmpNums[0];
}

// + + + -> + + *
// + + * -> + * +
// + * + -> + * *
// + * * -> * + +
// etc.
// it's number base-X counting but I'm a lazy bum and don't want to build a general number counter
int[] ShiftOperators(int[] operators, bool allowConcat)
{
    // Console.WriteLine("Ops before: {0}", string.Join(",", operators));
    for (var i = operators.Length - 1; i >= 0; i--)
    {
        var op = operators[i];
        if (op == ADD)
        {
            operators[i] = MULT;
            break;
        }
        else if (op == MULT)
        {
            if (allowConcat)
            {
                operators[i] = COMBINE;
                break;
            }
            else if (i != 0)
            {
                operators[i] = ADD;
                for (var j = i - 1; j >= 0; j--)
                {
                    if (operators[j] == MULT && j != 0) // don't overflow
                    {
                        operators[j] = ADD;
                    }
                    else if (operators[j] == ADD)
                    {
                        operators[j] = MULT;
                        break;
                    }
                }
                break;
            }
        }
        else if (op == COMBINE && allowConcat)
        {
            if (i != 0)
            {
                operators[i] = ADD;
                for (var j = i - 1; j >= 0; j--)
                {
                    if (operators[j] == COMBINE && j != 0) // don't overflow
                    {
                        operators[j] = ADD;
                    }
                    else if (operators[j] == MULT)
                    {
                        operators[j] = COMBINE;
                        break;
                    }
                    else if (operators[j] == ADD)
                    {
                        operators[j] = MULT;
                        break;
                    }
                }
                break;
            }
        }
    }
    // Console.WriteLine("Ops after: {0}", string.Join(",", operators));
    // Console.WriteLine("---");
    return operators;



    // // ...there's likely an easier way...but meh
    // var strRep = "";
    // foreach (var op in operators)
    // {
    //     strRep += op == ADD ? "0" : (op == MULT ? "1" : "2");
    // }
    // int num = Convert.ToInt32(strRep, 3);
    // num++;
    // var bitStringChars = Convert.ToString(num, 2).PadLeft(bitString.Length, '0').ToCharArray();
    // var changedOps = new bool[bitStringChars.Length]; // wow such allocate, many memory
    // for (var i = 0; i < bitStringChars.Length; i++)
    // {
    //     changedOps[i] = bitStringChars[i] == '0' ? ADD : MULT;
    // }
    // return changedOps;
}

long GetTotal(string[] fileLines, bool allowConcat)
{
    long total = 0;
    foreach (var line in fileLines)
    {
        // Console.WriteLine("Processing {0}", line);
        var parts = line.Split(":");
        var sum = long.Parse(parts[0]);
        var nums = parts[1].Split(" ").Where(x => !string.IsNullOrWhiteSpace(x)).Select(long.Parse).ToList();
        var totalPossibilities = Math.Pow(allowConcat ? 3 : 2, nums.Count - 1);
        // Console.WriteLine("Possibilities: {0} out of {1} nums", totalPossibilities, nums.Count);
        var operators = new int[nums.Count - 1]; // false = add, true = mult
        // fill with +
        for (var i = 0; i < nums.Count - 2; i++)
        {
            operators[i] = ADD;
        }
        // ok, start going
        for (var i = 0; i < totalPossibilities; i++)
        {
            // Console.WriteLine("Doing math: {0} and {1} = {2}?", string.Join(",", nums), string.Join(",", operators), sum);
            var solution = DoMath(nums, operators, sum);
            // Console.WriteLine("Solution is {0}", solution);
            if (solution == sum)
            {
                total += sum;
                // Console.WriteLine("   it's good!");
                break; // it works, don't check again
            }
            operators = ShiftOperators(operators, allowConcat);
        }
        // Console.WriteLine("-----");
    }
    return total;
}

// Console.WriteLine(string.Join(",",ShiftOperators([0,1,1],false)));
// Console.WriteLine(string.Join(",",ShiftOperators([0,0,0],true)));
// Console.WriteLine(string.Join(",",ShiftOperators([0,0,1],true)));
// Console.WriteLine(string.Join(",",ShiftOperators([0,0,2],true)));
// Console.WriteLine(string.Join(",",ShiftOperators([0,1,0],true)));
// Console.WriteLine(string.Join(",",ShiftOperators([0,1,1],true)));
// Console.WriteLine(string.Join(",",ShiftOperators([0,1,2],true)));
// Console.WriteLine(string.Join(",",ShiftOperators([0,2,2],true)));
// Console.WriteLine(string.Join(",",ShiftOperators([2,2,2],true)));

Console.WriteLine("Part 1 total: {0}", GetTotal(fileLines, false));
Console.WriteLine("Part 2 total: {0}", GetTotal(fileLines, true));
