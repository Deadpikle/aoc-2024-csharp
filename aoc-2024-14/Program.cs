using System.Text.RegularExpressions;

bool runExample = false;
var fileLines = File.ReadAllLines(runExample ? "example.txt" : "input.txt");

List<int> GetNums(string str)
{
    // https://stackoverflow.com/a/75628602/3938401
    // https://stackoverflow.com/a/15814655/3938401
    Regex regex = new Regex(@"-?[\d]+");
    var matchCollection = regex.Matches(str);
    return matchCollection.Select(x => int.Parse(x.Value)).ToList();
}

var robots = new List<Robot>();
foreach (var line in fileLines)
{
    if (line.StartsWith("#"))
    {
        continue;
    }
    var nums = GetNums(line);
    // Console.WriteLine("Nums: {0}", string.Join(",", nums));
    robots.Add(new Robot(nums[0], nums[1], nums[2], nums[3]));
}

const int numSeconds = 100;
int width = runExample ? 11 : 101;
int height = runExample ? 7 : 103;

void Draw()
{
    var spots = new Dictionary<(int, int), int>();
    foreach (var robot in robots)
    {
        if (!spots.ContainsKey((robot.X, robot.Y)))
        {
            spots.Add((robot.X, robot.Y), 0);
        }
        spots[(robot.X, robot.Y)]++;
    }
    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            if (spots.ContainsKey((x, y)))
            {
                Console.Write(spots[(x, y)]);
            }
            else
            {
                Console.Write(".");
            }
        }
        Console.WriteLine();
    }
}

Console.WriteLine("Before starting state:");
Draw();
Console.WriteLine();
Console.WriteLine();

for (var i = 0; i < numSeconds; i++)
{
    // move robots
    foreach (var robot in robots)
    {
        var nextX = robot.X + robot.DeltaX;
        var nextY = robot.Y + robot.DeltaY;
        // Console.WriteLine("Before wrapping; robot is at {1}, {2}", i + 1, nextX, nextY);
        // resolve wrapping
        if (nextX < 0)
        {
            nextX = width + nextX; // + as it's already negative
        }
        if (nextX >= width)
        {
            nextX = nextX - width;
        }
        if (nextY < 0)
        {
            nextY = height + nextY; // + as it's already negative
        }
        if (nextY >= height)
        {
            nextY = nextY - height;
        }
        robot.X = nextX;
        robot.Y = nextY;
    }

    // Console.WriteLine("After {0} seconds; robot is at {1}, {2}", i + 1, robots[0].X, robots[0].Y);
    // Draw();
    // Console.WriteLine();
    // Console.WriteLine();
}

// segment field into quadrants
var quadrantWidth = Math.Floor((double)width / 2);
var quadrantHeight = Math.Floor((double)height / 2);
Console.WriteLine("Quadrant width: {0}", quadrantWidth);
Console.WriteLine("Quadrant height: {0}", quadrantHeight);
// find robots and get answer
var topLeftQuad = 0;
var topRightQuad = 0;
var bottomLeftQuad = 0;
var bottomRightQuad = 0;
// tests for edge cases on small grid
// robots = new List<Robot>() 
// { 
//     new Robot(0,0,0,0), 
//     new Robot(4,0,0,0),
//     new Robot(5,0,0,0), // should NOT be counted
//     new Robot(6,0,0,0),
//     new Robot(10,0,0,0),
//     new Robot(0,3,0,0), // not counted
//     new Robot(0,4,0,0),
//     new Robot(0,6,0,0),
//     new Robot(10,3,0,0), // not counted
//     new Robot(10,6,0,0),
//     new Robot(5,6,0,0),
//     new Robot(6,6,0,0),
//     new Robot(6,3,0,0),
//     new Robot(6,4,0,0),
//     new Robot(5,4,0,0),
//     new Robot(10,4,0,0),
//     new Robot(0,2,0,0),
//     new Robot(4,2,0,0),
//     new Robot(5,2,0,0),
//     new Robot(5,3,0,0),
//     new Robot(6,2,0,0),
// };
foreach (var robot in robots)
{
    // 0-indexed!
    if (robot.X < quadrantWidth && robot.Y < quadrantHeight)
    {
        topLeftQuad++;
    }
    else if (robot.X < quadrantWidth && robot.Y >= height - quadrantHeight)
    {
        bottomLeftQuad++;
    }
    else if (robot.X >= width - quadrantWidth && robot.Y < quadrantHeight)
    {
        topRightQuad++;
    }
    else if (robot.X >= width - quadrantWidth && robot.Y >= height - quadrantHeight)
    {
        bottomRightQuad++;
    }
}
Console.WriteLine("After:");
Draw();
Console.WriteLine("Robots in {0}, {1}, {2}, {3}", topLeftQuad, topRightQuad, bottomLeftQuad, bottomRightQuad);
var part1Ans = topLeftQuad * bottomLeftQuad * topRightQuad * bottomRightQuad;
Console.WriteLine("Part 1: {0}", part1Ans);
foreach (var robot in robots)
{
    robot.Reset();
}
//
// make a christmas tree?!
string GetMap()
{
    var spots = new Dictionary<(int, int), int>();
    foreach (var robot in robots)
    {
        if (!spots.ContainsKey((robot.X, robot.Y)))
        {
            spots.Add((robot.X, robot.Y), 0);
        }
        spots[(robot.X, robot.Y)]++;
    }
    string output = "";
    for (var y = 0; y < height; y++)
    {
        for (var x = 0; x < width; x++)
        {
            if (spots.ContainsKey((x, y)))
            {
                output += spots[(x, y)];
            }
            else
            {
                output += ".";
            }
        }
        output += Environment.NewLine;
    }
    return output;
}
var nextPotentialTree = 99;
for (var i = 0; i < 100000; i++)
{
    // move robots
    foreach (var robot in robots)
    {
        var nextX = robot.X + robot.DeltaX;
        var nextY = robot.Y + robot.DeltaY;
        // resolve wrapping
        if (nextX < 0)
        {
            nextX = width + nextX; // + as it's already negative
        }
        if (nextX >= width)
        {
            nextX = nextX - width;
        }
        if (nextY < 0)
        {
            nextY = height + nextY; // + as it's already negative
        }
        if (nextY >= height)
        {
            nextY = nextY - height;
        }
        robot.X = nextX;
        robot.Y = nextY;
    }

    if (i + 1 == nextPotentialTree)
    {
        // Console.WriteLine("After {0} seconds:", i + 1);
        //mapStrings += string.Format("After {0} seconds:", i + 1);
        //mapStrings += Environment.NewLine;
        //mapStrings += GetMap();
        //mapStrings += Environment.NewLine;
        //mapStrings += Environment.NewLine;
        nextPotentialTree += 101; // magic number (?) for my input
        var str = GetMap();
        if (str.Contains("1111111111")) // figured this out by looking at a real tree in my console after finding manually
        {
            Console.WriteLine("Tree at second: {0}", i+1);
            Draw();
            break;
        }
        // To be clear about my process, which is not reflected in git history:
        // 1) Printed out every map for first thousand iterations or so
        // 2) Noticed that every X iterations the robots converged
        // 3) Printed out every X iterations 10,000 times or so
        // 4) Looked at the file to find the tree
        // 5) Manually find first tree
        // 6) Submit successful part 2 (after being reminded online to reset robots by others...)
        // 7) Re-code this to have code find first tree
    }
    // This will draw them easy enough for manual verification, however,
    // technically, now that we know what it looks like, we COULD find it via code
}