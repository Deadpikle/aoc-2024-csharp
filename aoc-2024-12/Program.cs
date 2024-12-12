var fileLines = File.ReadAllLines("input.txt");
// var fileLines = File.ReadAllLines("example.txt");
// var fileLines = File.ReadAllLines("example-2.txt");
// var fileLines = File.ReadAllLines("example-3.txt");
// var fileLines = File.ReadAllLines("example-4.txt");
// var fileLines = File.ReadAllLines("example-5.txt");

char GetPlotSpot(int x, int y, List<char[]> farm)
{
    if (x >= 0 && y >= 0 && x < farm[0].Length && y < farm.Count)
    {
        return farm[y][x];
    }
    return '.';
}

// create farm
var farm = new List<char[]>();
foreach (var line in fileLines)
{
    farm.Add(line.ToCharArray());
}
// ok, start at top left and don't hit regions more than 1x
var seenSpots = new Dictionary<string, char>();

bool CanReachSideUpDownViaX(int startX, int startY, char search, bool isNorthCheck, Dictionary<string, bool> sides, List<char []> farm)
{
    var currX = startX - 1;
    Console.WriteLine("  Searching for {0} ({3}) starting at {1},{2}", search, startX, startY, isNorthCheck ? "north" : "south");
    Console.WriteLine(" First Spot == {0}", GetPlotSpot(currX, startY, farm));
    while (GetPlotSpot(currX, startY, farm) == search)
    {
        if (GetPlotSpot(currX, isNorthCheck ? startY - 1 : startY + 1, farm) == search)
        {
            break;
        }
        var key = string.Format("{2},y,{0},{1}", currX, isNorthCheck ? startY - 1 : startY + 1, isNorthCheck ? "n" : "s");
        Console.WriteLine("     Trying y, {0}, {1}", currX, isNorthCheck ? startY - 1 : startY + 1);
        foreach (var side in sides)
        {
            if (key == side.Key)
            {
                Console.WriteLine("    found!");
                return true;
            }
        }
        currX--;
    }
    currX = startX + 1;
    while (GetPlotSpot(currX, startY, farm) == search)
    {
        if (GetPlotSpot(currX, isNorthCheck ? startY - 1 : startY + 1, farm) == search)
        {
            break;
        }
        Console.WriteLine("     Trying y, {0}, {1}", currX, isNorthCheck ? startY - 1 : startY + 1);
        var key = string.Format("{2},y,{0},{1}", currX, isNorthCheck ? startY - 1 : startY + 1, isNorthCheck ? "n" : "s");
        foreach (var side in sides)
        {
            if (key == side.Key)
            {
                Console.WriteLine("    found!");
                return true;
            }
        }
        currX++;
    }
    return false;
}

bool CanReachSideLeftRightViaY(int startX, int startY, char search, bool isWestCheck, Dictionary<string, bool> sides, List<char []> farm)
{
    var currY = startY - 1;
    Console.WriteLine("  Searching for {0} ({3}) starting at {1},{2}", search, startX, startY, isWestCheck ? "west" : "east");
    Console.WriteLine(" First Spot == {0}", GetPlotSpot(startX, currY, farm));
    while (GetPlotSpot(startX, currY, farm) == search)
    {
        if (GetPlotSpot(isWestCheck ? startX - 1 : startX + 1, currY, farm) == search)
        {
            break;
        }
        Console.WriteLine("     Trying y, {0}, {1}", isWestCheck ? startX - 1 : startX + 1, currY);
        var key = string.Format("{2},x,{0},{1}", isWestCheck ? startX - 1 : startX + 1, currY, isWestCheck ? "w" : "e");
        foreach (var side in sides)
        {
            if (key == side.Key)
            {
                Console.WriteLine("    found!");
                return true;
            }
        }
        currY--;
    }
    currY = startY + 1;
    while (GetPlotSpot(startX, currY, farm) == search)
    {
        if (GetPlotSpot(isWestCheck ? startX - 1 : startX + 1, currY, farm) == search)
        {
            break;
        }
        Console.WriteLine("     Trying y, {0}, {1}", isWestCheck ? startX - 1 : startX + 1, currY);
        var key = string.Format("{2},x,{0},{1}", isWestCheck ? startX - 1 : startX + 1, currY, isWestCheck ? "w" : "e");
        foreach (var side in sides)
        {
            if (key == side.Key)
            {
                Console.WriteLine("    found!");
                return true;
            }
        }
        currY++;
    }
    return false;
}

(int, int, int) FindPlot(int x, int y, char search, List<char[]> farm)
{
    var queue = new Queue<(int, int)>();
    queue.Enqueue((x, y));
    var totalSpots = 0;
    var perimeter = 0;
    var totalSides = 0;
    var sideCache = new Dictionary<string, bool>();
    while (queue.Count > 0)
    {
        var item = queue.Dequeue();
        var spot = GetPlotSpot(item.Item1, item.Item2, farm);
        var key = string.Format("{0},{1}", item.Item1, item.Item2);
        if (spot != '.' && !seenSpots.ContainsKey(key) && spot == search)
        {
            totalSpots++;
            seenSpots.Add(key, spot);
            var north = GetPlotSpot(item.Item1, item.Item2 - 1, farm);
            var south = GetPlotSpot(item.Item1, item.Item2 + 1, farm);
            var east = GetPlotSpot(item.Item1 + 1, item.Item2, farm);
            var west = GetPlotSpot(item.Item1 - 1, item.Item2, farm);
            var northSideKey = string.Format("n,y,{0},{1}", item.Item1, item.Item2 - 1);
            var southSideKey = string.Format("s,y,{0},{1}", item.Item1, item.Item2 + 1);
            var eastSideKey = string.Format("e,x,{0},{1}", item.Item1 + 1, item.Item2);
            var westSideKey = string.Format("w,x,{0},{1}", item.Item1 - 1, item.Item2);
            if (north != '.')
            {
                if (north == search)
                {
                    queue.Enqueue((item.Item1, item.Item2 - 1));
                }
                else
                {
                    perimeter++;
                    if (!sideCache.ContainsKey(northSideKey) && !CanReachSideUpDownViaX(item.Item1, item.Item2, spot, true, sideCache, farm))
                    {
                        // Console.WriteLine("Couldn't reach another north for {0},{1} for key {2}", item.Item1, item.Item2, northSideKey);
                        sideCache[northSideKey] = true;
                        totalSides++;
                    }
                }
            }
            else
            {
                perimeter++;
                if (!sideCache.ContainsKey(northSideKey) && !CanReachSideUpDownViaX(item.Item1, item.Item2, spot, true, sideCache, farm))
                {
                    // Console.WriteLine("Couldn't reach another north for {0},{1} for key {2}", item.Item1, item.Item2, northSideKey);
                    sideCache[northSideKey] = true;
                    // Console.WriteLine("   Updated cache: {0}", string.Join(";", sideCache.Keys));
                    totalSides++;
                }
            }
            if (south != '.')
            {
                if (south == search)
                {
                    queue.Enqueue((item.Item1, item.Item2 + 1));
                }
                else
                {
                    perimeter++;
                    if (!sideCache.ContainsKey(southSideKey) && !CanReachSideUpDownViaX(item.Item1, item.Item2, spot, false, sideCache, farm))
                    {
                        sideCache[southSideKey] = true;
                        totalSides++;
                    }
                }
            }
            else
            {
                perimeter++;
                if (!sideCache.ContainsKey(southSideKey) && !CanReachSideUpDownViaX(item.Item1, item.Item2, spot, false, sideCache, farm))
                {
                    sideCache[southSideKey] = true;
                    totalSides++;
                }
            }
            if (east != '.')
            {
                if (east == search)
                {
                    queue.Enqueue((item.Item1 + 1, item.Item2));
                }
                else
                {
                    perimeter++;
                    if (!sideCache.ContainsKey(eastSideKey) && !CanReachSideLeftRightViaY(item.Item1, item.Item2, spot, false, sideCache, farm))
                    {
                        sideCache[eastSideKey] = true;
                        totalSides++;
                    }
                }
            }
            else
            {
                perimeter++;
                if (!sideCache.ContainsKey(eastSideKey) && !CanReachSideLeftRightViaY(item.Item1, item.Item2, spot, false, sideCache, farm))
                {
                    sideCache[eastSideKey] = true;
                    totalSides++;
                }
            }
            if (west != '.')
            {
                if (west == search)
                {
                    queue.Enqueue((item.Item1 - 1, item.Item2));
                }
                else
                {
                    Console.WriteLine("Looking west: Character at {0} is NOT {1}! {3} in cache? {2}", westSideKey, search, sideCache.ContainsKey(westSideKey), westSideKey);
                    perimeter++;
                    if (!sideCache.ContainsKey(westSideKey) && !CanReachSideLeftRightViaY(item.Item1, item.Item2, spot, true, sideCache, farm))
                    {
                        sideCache[westSideKey] = true;
                        totalSides++;
                    }
                }
            }
            else
            {
                perimeter++;
                if (!sideCache.ContainsKey(westSideKey) && !CanReachSideLeftRightViaY(item.Item1, item.Item2, spot, true, sideCache, farm))
                {
                    sideCache[westSideKey] = true;
                    totalSides++;
                }
            }
        }
    }
    return (totalSpots, perimeter, totalSides);
}

long totalPrice = 0;
long totalPriceWithSides = 0;
for (var y = 0; y < farm.Count; y++)
{
    for (var x = 0; x < farm[y].Length; x++)
    {
        var key = string.Format("{0},{1}", x, y);
        if (!seenSpots.ContainsKey(key))
        {
            // process this spot
            (var totalSpots, var perimeter, var totalSides) = FindPlot(x, y, farm[y][x], farm);
            Console.WriteLine("{0} has totalSpots {1}", farm[y][x], totalSpots);
            Console.WriteLine("{0} has perimeter {1}", farm[y][x], perimeter);
            Console.WriteLine("{0} has total sides {1}", farm[y][x], totalSides);
            Console.WriteLine("----");
            totalPrice += totalSpots * perimeter;
            totalPriceWithSides += totalSpots * totalSides;
        }
    }
}
Console.WriteLine("Price of fence part 1 is {0}", totalPrice);
Console.WriteLine("Price of fence part 2 is {0}", totalPriceWithSides);