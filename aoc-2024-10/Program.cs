var fileLines = File.ReadAllLines("input.txt");
// var fileLines = File.ReadAllLines("example.txt");
// var fileLines = File.ReadAllLines("example-small-1.txt");
// var fileLines = File.ReadAllLines("example-small-2.txt");
// var fileLines = File.ReadAllLines("example-small-3.txt");

var map = new List<List<int>>();

int GetHeight(int x, int y, List<List<int>> map)
{
    if (x >= 0 && y >= 0 && y < map.Count && x < map[0].Count)
    {
        return map[y][x];
    }
    return -1;
}

foreach (var line in fileLines)
{
    map.Add(line.ToCharArray().Select(x => (int)char.GetNumericValue(x)).ToList());
}
// find all trailheads
var trailheads = new List<Point>();
for (var i = 0; i < map.Count; i++)
{
    for (var j = 0; j < map[i].Count; j++)
    {
        if (map[i][j] == 0)
        {
            trailheads.Add(new Point(j, i));
        }
    }
}
Console.WriteLine("There are {0} trailheads", trailheads.Count);
// start from every trailhead and find score
int GetScore(Point point, int desiredHeight, List<string> foundTops, List<List<int>> map)
{
    var curr = GetHeight(point.X, point.Y, map);
    if (curr != desiredHeight)
    {
        // Console.WriteLine("We want {0} but got {1} at {2},{3}", desiredHeight, curr, point.X, point.Y);
        return 0;
    }
    if (curr == 9)
    {
        var key = string.Format("{0},{1}", point.X, point.Y);
        if (!foundTops.Contains(key)) // ... what's funny is that I added this after and without it I have pt 2 solution basically
        {
            foundTops.Add(key);
            // Console.WriteLine("Got a 9 at {0}, {1}!", point.X, point.Y);
            return 1;
        }
        return 0;
    }
    return GetScore(new Point(point.X, point.Y - 1), curr + 1, foundTops, map) +
           GetScore(new Point(point.X + 1, point.Y), curr + 1, foundTops, map) +
           GetScore(new Point(point.X, point.Y + 1), curr + 1, foundTops, map) +
           GetScore(new Point(point.X - 1, point.Y), curr + 1, foundTops, map);
}

var total = 0;
foreach (var trailhead in trailheads)
{
    var score = GetScore(trailhead, 0, new List<string>(), map);
    // Console.WriteLine("Score of {0} from {1},{2}", score, trailhead.X, trailhead.Y);
    total += score;
}

Console.WriteLine("Part 1 score: {0}", total);

int GetRanking(Point point, int desiredHeight, List<string> foundTops, 
    List<string> foundPaths, List<string> currPath, List<List<int>> map)
{
    var curr = GetHeight(point.X, point.Y, map);
    var key = string.Format("{0},{1}", point.X, point.Y);
    if (curr != desiredHeight)
    {
        // Console.WriteLine("We want {0} but got {1} at {2},{3}", desiredHeight, curr, point.X, point.Y);
        return 0;
    }
    if (curr == 9)
    {
        currPath.Add(key);
        var fullPath = string.Join(";", currPath);
        foundPaths.Add(fullPath);
        if (!foundTops.Contains(key))
        {
            foundTops.Add(key);
            // Console.WriteLine("Got a 9 at {0}, {1}!", point.X, point.Y);
            return 1;
        }
        return 0;
    }
    currPath.Add(key);
    return GetRanking(new Point(point.X, point.Y - 1), curr + 1, foundTops, foundPaths, currPath.ToList(), map) +
           GetRanking(new Point(point.X + 1, point.Y), curr + 1, foundTops, foundPaths, currPath.ToList(), map) +
           GetRanking(new Point(point.X, point.Y + 1), curr + 1, foundTops, foundPaths, currPath.ToList(), map) +
           GetRanking(new Point(point.X - 1, point.Y), curr + 1, foundTops, foundPaths, currPath.ToList(), map);
}

total = 0;
foreach (var trailhead in trailheads)
{
    var paths = new List<string>();
    var score = GetRanking(trailhead, 0, new List<string>(), paths, new List<string>(), map);
    // Console.WriteLine("Ranking of {0} from {1},{2}", paths.Count, trailhead.X, trailhead.Y);
    total += paths.Count;
}

Console.WriteLine("Part 2 score: {0}", total);