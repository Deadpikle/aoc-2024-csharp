var fileLines = File.ReadAllLines("input.txt");
var firstList = new List<int>();
var secondList = new List<int>();
foreach (var line in fileLines)
{
    var split = line.Split("   ");
    firstList.Add(Int32.Parse(split[0]));
    secondList.Add(Int32.Parse(split[1]));
}
firstList.Sort();
secondList.Sort();
var distanceSum = 0;
for (var i = 0; i < firstList.Count; i++)
{
    distanceSum += Math.Abs(firstList[i] - secondList[i]);
}
Console.WriteLine("The total distance is: " + distanceSum);
Console.WriteLine("------");
var timesOccur = new Dictionary<int, int>();
foreach (var item in secondList)
{
    timesOccur[item] = timesOccur.ContainsKey(item) ? timesOccur[item] + 1 : 1;
}
var similarityScore = 0;
foreach (var item in firstList)
{
    similarityScore += timesOccur.ContainsKey(item) ? item * timesOccur[item] : 0;
}
Console.WriteLine("The similarity score is: " + similarityScore);
Console.WriteLine("-----------AoC 2023 Day 1-----------");
////// 2023 day 1 for fun :-)
fileLines = File.ReadAllLines("2023-input-1.txt");
// fileLines = File.ReadAllLines("2023-input-1-example.txt");
var total = 0;
foreach (var line in fileLines)
{
    var chars = line.ToCharArray();
    double firstDigit = -1;
    double lastDigit = -1;
    foreach (var character in chars)
    {
        if (Char.IsDigit(character))
        {
            if (firstDigit == -1)
            {
                firstDigit = Char.GetNumericValue(character);
                lastDigit = Char.GetNumericValue(character);
            }
            else 
            {
                lastDigit = Char.GetNumericValue(character);
            }
        }
    }
    var number = string.Format("{0}{1}", firstDigit, lastDigit);
    //Console.WriteLine(number);
    total += int.Parse(number);
}
Console.WriteLine("2023 part 1 is: " + total);
// ok part 1 works, now in part 2 it can be number WORDS :(
fileLines = File.ReadAllLines("2023-input-1.txt");
// fileLines = File.ReadAllLines("2023-input-1-example2.txt");
total = 0;
List<string> numWords = ["one", "two", "three", "four", "five", "six", "seven", "eight", "nine"];
foreach (var line in fileLines)
{
    // find first number word
    var firstIndexForWord = -1;
    var firstWordValue = -1;
    for (var i = 0; i < numWords.Count; i++)
    {
        var index = line.IndexOf(numWords[i]);
        // Console.WriteLine("First index of {0} is {1}", numWords[i], index);
        if (index != -1)
        {
            if (firstIndexForWord == -1 || index < firstIndexForWord)
            {
                firstIndexForWord = index;
                firstWordValue = i + 1;
            }
        }
    }
    // find last number word
    var lastIndexForWord = -1;
    var lastWordValue = -1;
    for (var i = 0; i < numWords.Count; i++)
    {
        var index = line.LastIndexOf(numWords[i]);
        if (index != -1)
        {
            if (lastIndexForWord == -1 || index > lastIndexForWord)
            {
                lastIndexForWord = index;
                lastWordValue = i + 1;
            }
        }
    }
    // find first digit
    // find last digit
    var chars = line.ToCharArray();
    double firstDigit = -1;
    double lastDigit = -1;
    var firstDigitIndex = -1;
    var lastDigitIndex = -1;
    for (var j = 0; j < chars.Count(); j++)
    {
        var character = chars[j];
        if (Char.IsDigit(character))
        {
            if (firstDigit == -1)
            {
                firstDigit = Char.GetNumericValue(character);
                firstDigitIndex = j;
                lastDigit = Char.GetNumericValue(character);
                lastDigitIndex = j;
            }
            else 
            {
                lastDigit = Char.GetNumericValue(character);
                lastDigitIndex = j;
            }
        }
    }
    // use indices to figure out what was first thing and what was last
    var finalFirstDigit = "";
    var finalLastDigit = "";
    if (firstIndexForWord >= 0 && firstDigitIndex < 0)
    {
        finalFirstDigit = firstWordValue.ToString();
    }
    else if (firstIndexForWord < 0 && firstDigitIndex >= 0)
    {
        finalFirstDigit = firstDigit.ToString();
    }
    else if (firstIndexForWord < firstDigitIndex)
    {
        finalFirstDigit = firstWordValue.ToString();
    }
    else
    {
        finalFirstDigit = firstDigit.ToString();
    }
    //
    if (lastIndexForWord >= 0 && lastDigitIndex < 0)
    {
        finalLastDigit = lastWordValue.ToString();
    }
    else if (lastIndexForWord < 0 && lastDigitIndex >= 0)
    {
        finalLastDigit = lastDigit.ToString();
    }
    else if (lastIndexForWord > lastDigitIndex)
    {
        finalLastDigit = lastWordValue.ToString();
    }
    else
    {
        finalLastDigit = lastDigit.ToString();
    }
    var number = string.Format("{0}{1}", finalFirstDigit, finalLastDigit);
    //Console.WriteLine(number);
    total += int.Parse(number);
}
Console.WriteLine("2023 part 2 is: " + total);