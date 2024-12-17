using System.Text.RegularExpressions;

List<int> GetNums(string str)
{
    // https://stackoverflow.com/a/75628602/3938401
    // https://stackoverflow.com/a/15814655/3938401
    Regex regex = new Regex(@"-?[\d]+");
    var matchCollection = regex.Matches(str);
    return matchCollection.Select(x => int.Parse(x.Value)).ToList();
}

var lines = File.ReadAllLines("input.txt");

var originalRegA = GetNums(lines[0])[0];
var originalRegB = GetNums(lines[1])[0];
var originalRegC = GetNums(lines[2])[0];
long regA = originalRegA;
long regB = originalRegB;
long regC = originalRegC;
var instructions = lines.Last().Split("Program: ")[1].Split(",").Select(int.Parse).ToList();

var done = false;
var output = new LinkedList<long>();

bool isPartTwo = true;
long regAStartVal = isPartTwo ? 0 : regA;
while (true)
{
    // run program
    regA = isPartTwo ? regAStartVal : regA;
    regB = originalRegB;
    regC = originalRegC;
    var instructionPtr = 0;
    if (regA % 100000000 == 0)
    {
        Console.WriteLine("Reg A start val = {0:n0}; B = {1}, C = {2}", regA, regB, regC);
    }
    var isPartTwoFailure = false;
    while (!done)
    {
        if (instructionPtr >= instructions.Count)
        {
            break;
        }
        var opcode = instructions[instructionPtr];
        // var operand = instructionPtr < instructions.Count - 1 ? instructions[instructionPtr + 1] : 0; // hmmm
        var operand = instructions[instructionPtr + 1];

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
            comboOperand = regC;
            break;
            case 7:
            comboOperand = 0;
            break;
        }
        switch (opcode)
        {
            case 0: // adv - division, numerator = A.val, denominator = 2^operand -> truncate to integer and write to A
            // combo operand
            case 6: // bdv - exactly like case 0 but result into B
            case 7: // cdv - exactly like case 0 but result into C
                //var pow = 1 << (int)comboOperand;
                //long result = 0;
                //if (regA < pow)
                //{
                //    result = 0;
                //}
                //else
                //{
                //    // var advResult = (long)(regA / pow);
                //    result = regA >> (int)comboOperand;
                //}
                var result = regA >> (int)comboOperand;
                switch (opcode)
                {
                    case 0:
                    regA = result;
                    break;
                    case 6:
                    regB = result;
                    break;
                    case 7:
                    regC = result;
                    break;
                }
                instructionPtr += 2;
            break;
            case 1: // bxl - bitwise XOR of B and literal operand; result -> B
                regB = regB ^ operand;
                instructionPtr += 2;
            break;
            case 2: // bst - combo operand modulo 8 (only keeps lowest 3 bits), result -> B
                regB = comboOperand & 0x7;
                instructionPtr += 2;
            break;
            case 3: // jnz - if A == 0, nothing. A not zero, jumps to literal operand number (and doesn't add 2 after)
                instructionPtr = regA != 0 ? operand : instructionPtr + 2;
            break;
            case 4: // bxc - bitwise XOR of B and C, result -> B. Reads operand but IGNORES it.
                regB = regB ^ regC;
                instructionPtr += 2;
            break;
            case 5: // out - val of combo operand modulo 8, outputs value (sep. by comma)
                var nextVal = comboOperand & 0x7;
                if (isPartTwo && instructions[output.Count] != nextVal)
                {
                    isPartTwoFailure = true;
                    break; // we failed part 2
                }
                instructionPtr += 2;
                output.AddLast(nextVal);
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
    //Console.WriteLine("{0} vs {1} ({2}) - failed? {3}", 
    //    string.Join(",", instructions), 
    //    string.Join(",", output), 
    //    output.Count, 
    //    isPartTwoFailure);
    if (!isPartTwoFailure && instructions.Count == output.Count)
    {
        // got it!
        Console.WriteLine("Found match at {0}", regAStartVal);
        break;
    }
    else
    {
        regAStartVal++;
        output.Clear();
    }
}

Console.WriteLine("{0}", string.Join(",", output));