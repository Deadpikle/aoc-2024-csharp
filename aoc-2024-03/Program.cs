var fileLines = File.ReadAllLines("input.txt");
// var fileLines = File.ReadAllLines("example-01.txt");
// var fileLines = File.ReadAllLines("test-01.txt");
// var fileLines = File.ReadAllLines("example-02.txt");
var allText = string.Join("", fileLines);

var parts = allText.Split("mul(");
var total = 0;

bool isAllDigits(string str)
{
    var chars = str.ToCharArray();
    foreach (var ch in chars)
    {
        if (!char.IsDigit(ch))
        {
            return false;
        }
    }
    return true;
}
int checkMulCommand(string part)
{
    var total = 0;
    if (part.Length >= 4)
    {
        var nums = part.Split(",");
        if (nums.Length >= 2)
        {
            // get first number
            var firstNum = nums[0];
            var firstNumInt = -1;
            var hasFirstNum = false;
            var hasSecondNum = false;
            if (firstNum.Length <= 3 && firstNum.Length >= 1 && isAllDigits(firstNum))
            {
                firstNumInt = int.Parse(firstNum);
                hasFirstNum = true;
            }
            // get second number
            var secondNumChars = nums[1];
            var secondNumStr = "";
            var hasEndParen = false;
            for (var i = 0; i < 4; i++)
            {
                if (i >= secondNumChars.Length)
                {
                    break;
                }
                if (char.IsDigit(secondNumChars[i]))
                {
                    secondNumStr += secondNumChars[i];
                }
                else if (secondNumChars[i] == ')')
                {
                    hasEndParen = true;
                    break;
                }
            }
            if (hasEndParen && secondNumStr.Length > 0)
            {
                hasSecondNum = true;
            }
            if (hasFirstNum && hasSecondNum)
            {
                total += firstNumInt * int.Parse(secondNumStr);
                // Console.WriteLine("         Added in {0} from {1} * {2}", firstNumInt * int.Parse(secondNumStr), firstNumInt, int.Parse(secondNumStr));
            }
        }
    }
    return total;
}
foreach (var part in parts)
{
    // Console.WriteLine("Full line is mul({0} ---- part is {1}", part, part);
    // length must be between X,Y) [4] and XXX,YYY) [8]
    total += checkMulCommand(part);
}
Console.WriteLine("Part 1 total: {0}", total);
////
var adjustedString = "";
var doNotLoc = allText.IndexOf("don't()");
var doTextLoc = -1;
while (doNotLoc != -1)
{
    adjustedString += allText.Substring(0, doNotLoc);
    // Console.WriteLine("Adding in {0}",  allText.Substring(0, doNotLoc));
    allText = allText.Substring(doNotLoc + "don't()".Length);
    doTextLoc = allText.IndexOf("do()");
    if (doTextLoc >= 0)
    {
        // Console.WriteLine("doTextLoc = {0}", doTextLoc);
        allText = allText.Substring(doTextLoc + "do()".Length);
        // Console.WriteLine("AllText is now {0}",  allText);
        doNotLoc = allText.IndexOf("don't()");
    }
    else
    {
        doNotLoc = -1; // no more valid text in the string
    }
}
if (doTextLoc >= 0)
{
    adjustedString += allText;
}
// Console.WriteLine("Parse out : " + adjustedString);
parts = adjustedString.Split("mul(");
total = 0;
foreach (var part in parts)
{
    // Console.WriteLine("Full line is mul({0} ---- part is {1}", part, part);
    // length must be between X,Y) [4] and XXX,YYY) [8]
    total += checkMulCommand(part);
}
Console.WriteLine("Part 2 total: {0}", total);