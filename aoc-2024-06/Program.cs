var fileLines = File.ReadAllLines("input.txt");
// var fileLines = File.ReadAllLines("example-01.txt");
var grid = new List<char[]>();
foreach (var line in fileLines)
{
    grid.Add(line.ToCharArray());
}
// find guard
var initialX = 0;
var initialY = 0;
for (var i = 0; i < grid.Count; i++)
{
    var didFind = false;
    var row = grid[i];
    for (var j = 0; j < row.Length; j++)
    {
        var spot = row[j];
        if (spot == '^') // TODO: will guard always face up initially? probably safe to assume fo rnow
        {
            initialX = j;
            initialY = i;
            didFind = true;
            break;
        }
    }
    if (didFind)
    {
        break;
    }
}
Console.WriteLine("Guard is initially at {0}, {1}", initialX, initialY);

char GetGridSpot(int x, int y, List<char[]> grid)
{
    if (x >= 0 && x < grid[0].Length && y >= 0 && y < grid.Count)
    {
        return grid[y][x];
    }
    return ' ';
}

Dictionary<string, List<string>>? GetGridVisitSpotCount(int initialX, int initialY, List<char[]> grid)
{
    var currentX = initialX;
    var currentY = initialY;
    var deltaX = 0; // facing up
    var deltaY = -1; // facing up
    var visitedSpots = new Dictionary<string, List<string>>();
    visitedSpots[string.Format("{0}|{1}", currentX, currentY)] = new List<string>() { string.Format("{0}|{1}", deltaX, deltaY) };

    while (GetGridSpot(currentX, currentY, grid) != ' ')
    {
        // first rotate guard if needed
        while (GetGridSpot(currentX + deltaX, currentY + deltaY, grid) == '#') // while => in case more than 1 obstruction, e.g. a corner
        {
            if (deltaX == 0 && deltaY == -1)
            {
                // up to right
                deltaX = 1;
                deltaY = 0;
            }
            else if (deltaX == 0 && deltaY == 1)
            {
                // down to left
                deltaX = -1;
                deltaY = 0;
            }
            else if (deltaX == -1 && deltaY == 0)
            {
                // left to up
                deltaX = 0;
                deltaY = -1;
            }
            else if (deltaX == 1 && deltaY == 0)
            {
                // right to down
                deltaX = 0;
                deltaY = 1;
            }
        }
        // then move guard
        currentX += deltaX;
        currentY += deltaY;
        if (GetGridSpot(currentX, currentY, grid) != ' ')
        {
            var key = string.Format("{0}|{1}", currentX, currentY);
            // Console.WriteLine("Key is {0}", key);
            /*if (visitedSpots.ContainsKey(key))
            {
                visitedSpots[key]++;
                // Console.WriteLine("{0}, {1} is {2}", currentX, currentY, visitedSpots[key]);
                if (visitedSpots[key] >= 25)
                {
                    return -2; // cycle found...1000 is not a good check for a cycle, but MEH
                    // it worked, and also 25 also works (but is probably low)
                    // THE REAL SOLUTION (DUH) would have been to see if they had been in that position AND orientation before, 
                    // then there is a loop. Not # of times visited. Oh well.
                }
            }
            else
            {
                visitedSpots[key] = 1;
            }*/
            // ^ old solution that worked technically
            // better solution that doesn't use a hardcoded number
            var deltaKey = string.Format("{0}|{1}", deltaX, deltaY);
            if (visitedSpots.ContainsKey(key))
            {
                if (visitedSpots[key].Contains(deltaKey))
                {
                    // loop! 
                    return null;
                }
            }
            else
            {
                visitedSpots[key] = new List<string>() { deltaKey };
            }
        }
    }
    return visitedSpots;
}

var p1Spots = GetGridVisitSpotCount(initialX, initialY, grid);
Console.WriteLine("Part 1: moved to a total of {0} spaces", p1Spots!.Count);
// part 2
var obstructionLocations = 0;
// optimization after the fact: only place obstructions in places where the guard walks
foreach (var a in p1Spots!)
{
    var details = a.Key.Split("|");
    var x = int.Parse(details[0]);
    var y = int.Parse(details[1]);
//for (var y = 0; y < grid.Count; y++) // original loop trying everywhere; above optimization done after submission (only try places guard will walk)
//{
//    for (var x = 0; x < grid.Count; x++)
//    {
        var spot = GetGridSpot(x, y, grid);
        if (spot != '^' && spot != '#')
        {
            // Console.WriteLine("Currently trying obstruction at {0}, {1}", x, y);
            //var gridCopy = new List<char[]>();
            // copy grid. yuck. why isn't it simpler to not use //references??
            //for (var i = 0; i < grid.Count; i++)
            //{
            //    var row = new char[grid[i].Length];
            //    for (var j = 0; j < grid[i].Length; j++)
            //    {
            //        row[j] = grid[i][j];
            //    }
            //    gridCopy.Add(row);
            //}
            //gridCopy[y][x] = '#';
            grid[y][x] = '#'; // didn't do this in initial run but not copying the grid will of course save some time
            // print grid for debug
            //for (var i = 0; i < grid.Count; i++)
            //{
            //    for (var j = 0; j < grid[i].Length; j++)
            //    {
            //        Console.Write(gridCopy[j][i]);
            //    }
            //    Console.WriteLine();
            //}
            if (GetGridVisitSpotCount(initialX, initialY, grid) == null)
            {
                obstructionLocations++;
            }
            grid[y][x] = '.';
        }
    //}
}
Console.WriteLine("Part 2: {0} obstructions", obstructionLocations);