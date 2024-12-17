﻿var fileLines = File.ReadAllLines("input.txt");

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

void PrintMapWithVisited(Dictionary<(int, int), bool> dict)
{
    for (var y = 0; y < map.Count; y++)
    {
        for (var x = 0; x < map[0].Length; x++)
        {
            Console.Write("{0}", dict.ContainsKey((x, y)) && map[y][x] != '#' ? 'O' : map[y][x]);
        }
        Console.WriteLine();
    }
}

char GetNext(int x, int y, char dir)
{
    if (dir == '>')
    {
        return map[y][x + 1];
    }
    if (dir == '<')
    {
        return map[y][x - 1];
    }
    if (dir == 'v')
    {
        return map[y + 1][x];
    }
    if (dir == '^')
    {
        return map[y - 1][x];
    }
    return ' ';
}

var currentBestScore = long.MaxValue;
var bestPaths = new List<Dictionary<(int, int), bool>>();
long GetLengthToEnd(int currX, int currY, int endX, int endY, char direction, int score, int steps, int turns, Dictionary<(int, int), bool> visitedThisPath, Dictionary<(int, int, char), long> locationScores)
{
    if (currentBestScore != long.MaxValue && score > currentBestScore)
    {
        return long.MaxValue;
    }
    if (currX == endX && currY == endY)
    {
        visitedThisPath.Add((currX, currY), true); // add end tile for length calculations
        if (score < currentBestScore)
        {
            Console.WriteLine("   New Score is {0}; steps is {1}, turns is {2}", score, steps, turns);
            currentBestScore = score;
            bestPaths.Clear();
            bestPaths.Add(new Dictionary<(int, int), bool>(visitedThisPath));
        }
        else if (score == currentBestScore)
        {
            Console.WriteLine("   Matching Score is {0}; steps is {1}, turns is {2}", score, steps, turns);
            bestPaths.Add(new Dictionary<(int, int), bool>(visitedThisPath));
        }
        return score; // we got to the end!
    }
    if (visitedThisPath.ContainsKey((currX, currY))) // only visit each tile 1x
    {
        return long.MaxValue;
    }
    visitedThisPath.Add((currX, currY), true);
    if (locationScores.ContainsKey((currX, currY, direction)) && score > locationScores[(currX, currY, direction)])
    {
        return long.MaxValue;
    }
    var currSpot = map[currY][currX];
    if (currSpot == '#')
    {
        return long.MaxValue; // we walked onto a wall. good job.
    }
    if (!locationScores.ContainsKey((currX, currY, direction)))
    {
        locationScores.Add((currX, currY, direction), score);
    }
    else
    {
        locationScores[(currX, currY, direction)] = score;
    }
    // Console.WriteLine("At: {0}, {1}: {2}; Steps: {3}, Turns: {4}", currX, currY, currSpot, steps, turns);
    // try moving all four directions and recurse
    return new long[] {
        // left
        CanMoveDirection(direction, '<') && GetNext(currX, currY, '<') != '#' && !visitedThisPath.ContainsKey((currX - 1, currY))
            ? GetLengthToEnd(currX - 1, currY, endX, endY, '<', score + 1 + 1000 * GetRotations(direction, '<'), steps + 1, turns + GetRotations(direction, '<'), new Dictionary<(int, int), bool>(visitedThisPath), locationScores) 
            : long.MaxValue,
        // right
        CanMoveDirection(direction, '>') && GetNext(currX, currY, '>') != '#'  && !visitedThisPath.ContainsKey((currX + 1, currY))
            ? GetLengthToEnd(currX + 1, currY, endX, endY, '>', score + 1 + 1000 * GetRotations(direction, '>'), steps + 1, turns + GetRotations(direction, '>'), new Dictionary<(int, int), bool>(visitedThisPath), locationScores)
            : long.MaxValue,
        // up
        CanMoveDirection(direction, '^')  && GetNext(currX, currY, '^') != '#' && !visitedThisPath.ContainsKey((currX, currY - 1))
            ? GetLengthToEnd(currX, currY - 1, endX, endY, '^', score + 1 + 1000 * GetRotations(direction, '^'), steps + 1, turns + GetRotations(direction, '^'), new Dictionary<(int, int), bool>(visitedThisPath), locationScores)
            : long.MaxValue,
        // down
        CanMoveDirection(direction, 'v') && GetNext(currX, currY, 'v') != '#' && !visitedThisPath.ContainsKey((currX, currY + 1))
            ? GetLengthToEnd(currX, currY + 1, endX, endY, 'v', score + 1 + 1000 * GetRotations(direction, 'v'), steps + 1, turns + GetRotations(direction, 'v'), new Dictionary<(int, int), bool>(visitedThisPath), locationScores)
            : long.MaxValue,
    }.Min();
}
var watch = System.Diagnostics.Stopwatch.StartNew();
var visited = new Dictionary<(int, int), bool>();
var score = GetLengthToEnd(startX, startY, exitX, exitY, '>', 0, 0, 0, visited, new Dictionary<(int, int, char), long>());
Console.WriteLine("Score part 1: {0}", score);

var allBestPathSpots = new Dictionary<(int, int), bool>();
foreach (var spots in bestPaths)
{
    foreach (KeyValuePair<(int, int), bool> entry in spots)
    {
        if (!allBestPathSpots.ContainsKey((entry.Key.Item1, entry.Key.Item2)))
        {
            allBestPathSpots.Add((entry.Key.Item1, entry.Key.Item2), true);
        }
    }
}
watch.Stop();
Console.WriteLine("Best score: {0}", currentBestScore);
Console.WriteLine("Most tiles: {0}", allBestPathSpots.Count);
Console.WriteLine("Time: {0}ms", watch.ElapsedMilliseconds);
// for (var y = 0; y < map.Count; y++)
// {
//     for (var x = 0; x < map[0].Length; x++)
//     {
//         Console.Write("{0}", allBestPathSpots.ContainsKey((x, y)) && map[y][x] != '#' ? 'O' : map[y][x]);
//     }
//     Console.WriteLine();
// }