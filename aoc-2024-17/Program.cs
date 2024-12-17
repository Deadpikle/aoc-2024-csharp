using System.Text.RegularExpressions;

List<int> GetNums(string str)
{
    // https://stackoverflow.com/a/75628602/3938401
    // https://stackoverflow.com/a/15814655/3938401
    Regex regex = new Regex(@"-?[\d]+");
    var matchCollection = regex.Matches(str);
    return matchCollection.Select(x => int.Parse(x.Value)).ToList();
}

var lines = File.ReadAllLines("example2.txt");

var originalRegA = GetNums(lines[0])[0];
var originalRegB = GetNums(lines[1])[0];
var originalRegC = GetNums(lines[2])[0];
long regA = originalRegA;
long regB = originalRegB;
long regC = originalRegC;
var instructions = lines.Last().Split("Program: ")[1].Split(",").Select(int.Parse).ToList();
var instructionPtr = 0;


long GetComboOperator(int operand)
{
    switch (operand)
    {
        case 4:
        return regA;
        case 5:
        return regB;
        case 6:
        return regB;
        case 7:
        return 0;
        default:
        return operand;
    }
}

bool CheckMatch(List<int> original, List<long> toCheck)
{
    if (original.Count != toCheck.Count)
    {
        return false;
    }
    for (var i = 0; i < original.Count; i++)
    {
        if (original[i] != toCheck[i])
        {
            return false;
        }
    }
    return true;
}

var done = false;
var output = new List<long>();

bool isPartTwo = true;
long regAStartVal = isPartTwo ? 0 : regA;

while (true)
{
    // run program
    regA = isPartTwo ? regAStartVal : regA;
    regB = originalRegB;
    regC = originalRegC;
    if (regA % 1000000 == 0)
    {
        Console.WriteLine("Reg A start val = {0}; B = {1}, C = {2}", regA, regB, regC);
    }
    while (!done)
    {
        if (instructionPtr >= instructions.Count)
        {
            break;
        }
        var opcode = instructions[instructionPtr];
        var operand = instructionPtr < instructions.Count - 1 ? instructions[instructionPtr + 1] : 0; // hmmm

        long comboOperand = operand;
        switch (operand)
        {
            case 4:
            comboOperand = regA;
            break;
            case 5:
            comboOperand = regB;
            break;
            case 6:
            comboOperand = regB;
            break;
            case 7:
            comboOperand = 0;
            break;
        }
        var isPartTwoFailure = false;
        switch (opcode)
        {
            case 0: // adv - division, numerator = A.val, denominator = 2^operand -> truncate to integer and write to A
            // combo operand
                var advResult = Math.Floor(regA / Math.Pow(2, comboOperand));
                regA = (int)advResult;
                instructionPtr += 2;
            break;
            case 1: // bxl - bitwise XOR of B and literal operand; result -> B
                var bxlResult = regB ^ operand;
                regB = bxlResult;
                instructionPtr += 2;
            break;
            case 2: // bst - combo operand modulo 8 (only keeps lowest 3 bits), result -> B
                var bstResult = comboOperand % 8;
                regB = bstResult;
                instructionPtr += 2;
            break;
            case 3: // jnz - if A == 0, nothing. A not zero, jumps to literal operand number (and doesn't add 2 after)
                if (regA != 0)
                {
                    instructionPtr = operand;
                }
                else 
                {
                    instructionPtr += 2;
                }
            break;
            case 4: // bxc - bitwise XOR of B and C, result -> B. Reads operand but IGNORES it.
                var bxcResult = regB ^ regC;
                regB = bxcResult;
                instructionPtr += 2;
            break;
            case 5: // out - val of combo operand modulo 8, outputs value (sep. by comma)
                var nextVal = comboOperand % 8;
                if (isPartTwo && instructions[output.Count] != nextVal)
                {
                    isPartTwoFailure = true;
                    break; // we failed part 2
                }
                instructionPtr += 2;
                output.Add(nextVal);
            break;
            case 6: // bdv - exactly like case 0 but result into B
                var bdvResult = Math.Floor(regA / Math.Pow(2, comboOperand));
                regB = (int)bdvResult;
                instructionPtr += 2;
            break;
            case 7: // cdv - exactly like case 0 but result into C
                var cdvResult = Math.Floor(regA / Math.Pow(2, comboOperand));
                regC = (int)cdvResult;
                instructionPtr += 2;
            break;
            default:
            break;
        }
        if (isPartTwo && isPartTwoFailure)
        {
            break;
        }
    }
    if (!isPartTwo)
    {
        break;
    }
    // Console.WriteLine("{0} vs {1} ({2})", 
    //     string.Join(",", instructions), 
    //     string.Join(",", output.Select(int.Parse).ToList()), 
    //     output.Count);
    if (CheckMatch(instructions, output))
    {
        // got it!
        Console.WriteLine("Found match at {0}", regAStartVal);
        break;
    }
    else
    {
        regAStartVal++;
        instructionPtr = 0;
        output.Clear();
    }
}

Console.WriteLine("{0}", string.Join(",", output));