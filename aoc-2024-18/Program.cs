var runExample = false;
var height = runExample ? 7 : 71;
var width = runExample ? 7 : 71;
var linesToRead = runExample ? 12 : 1024;
var map = new bool[height,width];

var lines = File.ReadAllLines(runExample ? "example.txt" : "input.txt");
var linesRead = 0;
foreach (var line in lines)
{
    var parts = line.Trim().Split(",");
    var x = int.Parse(parts[0]);
    var y = int.Parse(parts[1]);
    Console.WriteLine("{0},{1}", x, y);
    map[y,x] = true;
    linesRead++;
    if (linesRead >= linesToRead) {
        break;
    }
}

void PrintMap(bool[,] toPrint) {
    Console.WriteLine();
    for (var i = 0; i < toPrint.GetLength(0); i++) {
        for (var j = 0; j < toPrint.GetLength(1); j++) {
            Console.Write(toPrint[i,j] ? '#' : '.');
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

PrintMap(map);

int FindShortestPath(bool[,] map, (int, int) pos, (int, int) dest, Dictionary<(int, int), bool> visited, Dictionary<(int, int), int> leastCosts, int length)
{
    if (pos.Item1 == dest.Item1 && pos.Item2 == dest.Item2)
    {
        // Found path to end!
        return length;
    }
    if (pos.Item1 < 0 || pos.Item1 >= map.GetLength(0) || pos.Item2 < 0 || pos.Item2 >= map.GetLength(1))
    {
        // Console.WriteLine("Outside bounds {0}", pos);
        return int.MaxValue; // failure, outside bounds
    }
    if (map[pos.Item2, pos.Item1] == true) // true designates inaccessible location
    {
        // Console.WriteLine("Inaccessible {0}", pos);
        return int.MaxValue; // failure
    }
    if (visited.ContainsKey((pos.Item1, pos.Item2)))
    {
        // Console.WriteLine("Visited here already {0}", pos);
        return int.MaxValue; // failure
    }
    if (!leastCosts.TryGetValue(pos, out var leastCostVal))
    {
        leastCosts.Add(pos, length);
    }
    else if (leastCostVal > length)
    {
        leastCosts[pos] = length; // we found a better route to this position
    }
    else if (leastCostVal <= length)
    {
        return int.MaxValue; // no use going this way, there is a better route available to this position
    }
    // return min of 4 directions
    visited.Add(pos, true);
    return new int[] {
        pos.Item1 - 1 >= 0 && map[pos.Item2, pos.Item1 - 1] != true && !visited.ContainsKey((pos.Item1 - 1, pos.Item2))
            ? FindShortestPath(map, (pos.Item1 - 1, pos.Item2), dest, new Dictionary<(int, int), bool>(visited), leastCosts, length + 1)
            : int.MaxValue,
        
        pos.Item1 + 1 < map.GetLength(0) && map[pos.Item2, pos.Item1 + 1] != true && !visited.ContainsKey((pos.Item1 + 1, pos.Item2))
            ? FindShortestPath(map, (pos.Item1 + 1, pos.Item2), dest, new Dictionary<(int, int), bool>(visited), leastCosts, length + 1)
            : int.MaxValue,
        
        pos.Item2 + 1 < map.GetLength(0) && map[pos.Item2 + 1, pos.Item1] != true && !visited.ContainsKey((pos.Item1, pos.Item2 + 1))
            ? FindShortestPath(map, (pos.Item1, pos.Item2 + 1), dest, new Dictionary<(int, int), bool>(visited), leastCosts, length + 1)
            : int.MaxValue,

        pos.Item2 - 1 >= 0 && map[pos.Item2 - 1, pos.Item1] != true && !visited.ContainsKey((pos.Item1, pos.Item2 - 1))
            ? FindShortestPath(map, (pos.Item1, pos.Item2 - 1), dest, new Dictionary<(int, int), bool>(visited), leastCosts, length + 1)
            : int.MaxValue,
    }.Min();
}

Console.WriteLine("Shortest path after 1024 bytes: " + FindShortestPath(map, (0, 0), (height - 1, width - 1), [], [], 0));
linesRead = 0;
// part 2: see when things will fail

bool DoesPathExist(bool[,] map, (int, int) pos, (int, int) dest)
{
    var visited = new Dictionary<(int, int), bool>();
    var stack = new Stack<(int, int)>();
    stack.Push(pos);
    visited.Add(pos, true);
    while (stack.Count > 0)
    {
        var item = stack.Pop();
        // Console.WriteLine("exploring {0}, stack count is {1}", item, stack.Count);
        if (item.Item1 == dest.Item1 && item.Item2 == dest.Item2)
        {
            return true;
        }
        if (item.Item1 - 1 >= 0 && map[item.Item2, item.Item1 - 1] != true && !visited.ContainsKey((item.Item1 - 1, item.Item2)))
        {
            // Console.WriteLine("   adding {0}", (item.Item1 - 1, item.Item2));
            stack.Push((item.Item1 - 1, item.Item2));
            visited.Add((item.Item1 - 1, item.Item2), true);
        }
        if (item.Item1 + 1 < map.GetLength(0) && map[item.Item2, item.Item1 + 1] != true && !visited.ContainsKey((item.Item1 + 1, item.Item2)))
        {
            // Console.WriteLine("   adding {0}", (item.Item1 + 1, item.Item2));
            stack.Push((item.Item1 + 1, item.Item2));
            visited.Add((item.Item1 + 1, item.Item2), true);
        }
        // Console.WriteLine("   Considering {0}",(item.Item1, item.Item2 + 1));
        if (item.Item2 + 1 < map.GetLength(0) && map[item.Item2 + 1, item.Item1] != true && !visited.ContainsKey((item.Item1, item.Item2 + 1)))
        {
            // Console.WriteLine("   adding {0}", (item.Item1, item.Item2 + 1));
            stack.Push((item.Item1, item.Item2 + 1));
            visited.Add((item.Item1, item.Item2 + 1), true);
        }
        if (item.Item2 - 1 >= 0 && map[item.Item2 - 1, item.Item1] != true && !visited.ContainsKey((item.Item1, item.Item2 - 1)))
        {
            // Console.WriteLine("   adding {0}", (item.Item1, item.Item2 - 1));
            stack.Push((item.Item1, item.Item2 - 1));
            visited.Add((item.Item1, item.Item2 - 1), true);
        }
    }
    return false;
}

// the thing with part 2 is that we just have to get to the end - we don't care about shortest path!
// that would be a depth first search rather than breadth first like we are doing now.
foreach (var line in lines)
{
    linesRead++; // skip ahead (yes this code is bad/lazy/etc.)
    // we could also run this backward and see the first one that succeeds...
    // or run from the end instead of beginning...
    // could also return previous path and see if the previous path still works...
    // could also multithread...since each map is unique...
    if (linesRead > linesToRead) {
        var parts = line.Trim().Split(",");
        var x = int.Parse(parts[0]);
        var y = int.Parse(parts[1]);
        Console.WriteLine("Byte at {0},{1} fell", x, y);
        map[y,x] = true;
        Console.WriteLine("Exploring path after {0} bytes", linesRead);
        var hasPath = DoesPathExist(map, (0, 0), (height - 1, width - 1));
        if (!hasPath)
        {
            Console.WriteLine("No solution for {0},{1}", x, y);
            break;
        }
        
        //var shortestLength = FindShortestPath(map, (0, 0), (height - 1, width - 1), [], [], 0);
        //if (shortestLength >= 99999)
        //{
        //    Console.WriteLine("No solution for {0},{1}", x, y);
        //    break;
        //}
    }
}