var fileLines = File.ReadAllLines("example2.txt");

var startX = 0;
var startY = 0;
var exitX = 0;
var exitY = 0;

var map = new List<char[]>();
foreach (var line in fileLines)
{
    for (var x = 0; x < line.Length; x++)
    {
        // assumes one start, one exit
        if (line[x] == 'E')
        {
            exitX = x;
            exitY = map.Count;
        }
        if (line[x] == 'S')
        {
            startX = x;
            startY = map.Count;
        }
    }
    map.Add(line.ToCharArray());
}

int GetRotations(char from, char to)
{
    if (from == to)
    {
        return 0;
    }
    if (from == '<' && (to == '^' || to == 'v'))
    {
        return 1;
    }
    if (from == '<' && (to == '>'))
    {
        return 2;
    }
    if (from == '^' && (to == '<' || to == '>'))
    {
        return 1;
    }
    if (from == '^' && (to == 'v'))
    {
        return 2;
    }
    if (from == '>' && (to == '^' || to == 'v'))
    {
        return 1;
    }
    if (from == '>' && (to == '<'))
    {
        return 2;
    }
    if (from == 'v' && (to == '<' || to == '>'))
    {
        return 1;
    }
    if (from == 'v' && (to == '^'))
    {
        return 2;
    }
    return 0;
}

bool CanMoveDirection(char from, char to)
{
    if (from == '<' && to == '>')
    {
        return false;
    }
    if (from == '^' && to == 'v')
    {
        return false;
    }
    if (from == '>' && to == '<')
    {
        return false;
    }
    if (from == 'v' && to == '^')
    {
        return false;
    }
    return true;
}

long GetLengthToEnd(int currX, int currY, int endX, int endY, char direction, int score, int steps, int turns, Dictionary<(int, int), bool> visitedThisPath)
{
    if (currX == endX && currY == endY)
    {
        Console.WriteLine("   Score is {0}; steps is {1}, turns is {2}", score, steps, turns);
        return score; // we got to the end!
    }
    if (visitedThisPath.ContainsKey((currX, currY))) // only visit each tile 1x
    {
        return long.MaxValue;
    }
    visitedThisPath.Add((currX, currY), true);
    var currSpot = map[currY][currX];
    Console.WriteLine("Steps: {0}, Turns: {1}", steps, turns);
    // Console.WriteLine("At: {0}, {1}: {2}", currX, currY, currSpot);
    if (currSpot == '#')
    {
        return long.MaxValue; // we walked into a wall. good job.
    }
    // try moving all four directions and recurse
    return new long[] {
        // left
        CanMoveDirection(direction, '<') 
            ? GetLengthToEnd(currX - 1, currY, endX, endY, '<', score + 1 + 1000 * GetRotations(direction, '<'), steps + 1, turns + GetRotations(direction, '<'), new Dictionary<(int, int), bool>(visitedThisPath)) 
            : long.MaxValue,
        // right
        CanMoveDirection(direction, '>') 
            ? GetLengthToEnd(currX + 1, currY, endX, endY, '>', score + 1 + 1000 * GetRotations(direction, '>'), steps + 1, turns + GetRotations(direction, '>'), new Dictionary<(int, int), bool>(visitedThisPath))
            : long.MaxValue,
        // up
        CanMoveDirection(direction, '^') 
            ? GetLengthToEnd(currX, currY - 1, endX, endY, '^', score + 1 + 1000 * GetRotations(direction, '^'), steps + 1, turns + GetRotations(direction, '^'), new Dictionary<(int, int), bool>(visitedThisPath))
            : long.MaxValue,
        // down
        CanMoveDirection(direction, 'v') 
            ? GetLengthToEnd(currX, currY + 1, endX, endY, 'v', score + 1 + 1000 * GetRotations(direction, 'v'), steps + 1, turns + GetRotations(direction, 'v'), new Dictionary<(int, int), bool>(visitedThisPath))
            : long.MaxValue,
    }.Min();
}
var visited = new Dictionary<(int, int), bool>();
var score = GetLengthToEnd(startX, startY, exitX, exitY, '>', 0, 0, 0, visited);
Console.WriteLine("Score part 1: {0}", score);
//for (var y = 0; y < map.Count; y++)
//{
//    for (var x = 0; x < map[0].Length; x++)
//    {
//        //Console.Write("{0}", visited.ContainsKey((x, y)) && map[y][x] != '#' ? '!' : map[y][x]);
//    }
//    Console.WriteLine();
//}