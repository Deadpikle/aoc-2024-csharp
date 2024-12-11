var fileLines = File.ReadAllLines("input.txt");
// fileLines = File.ReadAllLines("example-01.txt");
// fileLines = File.ReadAllLines("example-01test.txt");

var rules = new List<(int, int)>();
var pageOrders = new List<string>();
var loadingRules = true;
foreach (var line in fileLines)
{
    if (loadingRules && line != "")
    {
        var items = line.Split("|");
        rules.Add((int.Parse(items[0]), int.Parse(items[1])));
    }
    else if (line != "")
    {
        pageOrders.Add(line);
    }
    if (line == "")
    {
        loadingRules = false;
    }
}
var middleNums = new List<int>();
var incorrectPageOrders = new List<string>();

bool isValidOrdering(List<(int, int)> rules, List<int> pages)
{ 
    var isValid = true;
    for (var i = 0; i < pages.Count; i++)
    {
        var pageNum = pages[i];
        for (var j = i + 1; j < pages.Count; j++)
        {
            var followingPageNum = pages[j];
            foreach (var rule in rules)
            {
                if (rule.Item2 == pageNum && rule.Item1 == followingPageNum)
                {
                    isValid = false;
                    break;
                }
            }
            if (!isValid)
            {
                break;
            }
        }
    }
    return isValid;
}

foreach (var pageOrder in pageOrders)
{
    Console.WriteLine("Processing: {0}", pageOrder);
    var pages = pageOrder.Split(",").Select(int.Parse).ToList();
    var isValid = isValidOrdering(rules, pages);
    if (isValid)
    {
        middleNums.Add(pages[pages.Count / 2]);
    }
    else
    {
        incorrectPageOrders.Add(pageOrder);
    }
}
Console.WriteLine("Sum of part 1: {0}", middleNums.Sum());
Console.WriteLine("------");
// now process incorrect page orderings

bool isItemAtIndexCorrect(List<(int, int)> rules, List<int> pages, int index)
{
    var isValid = true;
    var pageNum = pages[index];
    for (var j = index + 1; j < pages.Count; j++)
    {
        var followingPageNum = pages[j];
        foreach (var rule in rules)
        {
            if (rule.Item2 == pageNum && rule.Item1 == followingPageNum)
            {
                isValid = false;
                break;
            }
        }
        if (!isValid)
        {
            break;
        }
    }
    return isValid;
}

middleNums = new List<int>();
foreach (var invalidOrder in incorrectPageOrders)
{
    Console.WriteLine("Processing incorrect order: {0}", invalidOrder);
    var pages = invalidOrder.Split(",").Select(int.Parse).ToList();
    var fixedOrder = new List<int>();
    for (var i = 0; i < pages.Count; i++) // i = index I want to figure out
    {
        for (var j = i; j < pages.Count; j++)
        {
            var pagesCopy = new List<int>(pages);
            var tmp = pagesCopy[j];
            pagesCopy.RemoveAt(j);
            pagesCopy.Insert(i, tmp);
            // Console.WriteLine("Page copy: {0}", string.Join(",", pagesCopy));
            if (isItemAtIndexCorrect(rules, pagesCopy, i))
            {
                fixedOrder.Add(pagesCopy[i]);
                pages = new List<int>(pagesCopy);
                // Console.WriteLine("~~~index {0} is good; fixedOrder is currently {1}", i, string.Join(",", fixedOrder));
                break;
            }
        }
    }
    if (isValidOrdering(rules, fixedOrder))
    {
        Console.WriteLine("Fixed! {0}", string.Join(",", fixedOrder));
        middleNums.Add(fixedOrder[fixedOrder.Count / 2]);
    }
    else
    {
        throw new Exception("Error, invalid ordering");
    }
}
Console.WriteLine("Sum of part 2: {0}", middleNums.Sum());