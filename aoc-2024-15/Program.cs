
bool runExample = false;
var fileLines = File.ReadAllLines(runExample ? "example.txt" : "input.txt");

var map = new List<char[]>();
var isGettingMap = true;
var directions = "";
foreach (var line in fileLines)
{
    if (line == "")
    {
        isGettingMap = false;
    }
    else
    {
        if (isGettingMap)
        {
            map.Add(line.ToCharArray());
        }
        else
        {
            directions += line.Trim();
        }
    }
}
Console.WriteLine(directions);
// map everything
var robotX = -1;
var robotY = -1;
for (var y = 0; y < map.Count; y++)
{
    for (var x = 0; x < map[0].Length; x++)
    {
        var ch = map[y][x];
        if (ch == '@')
        {
            robotX = x;
            robotY = y;
            break;
        }
    }
    if (robotX != -1)
    {
        break;
    }
}

string GetMapStr(List<char[]> map)
{
    var output = "";
    for (var y = 0; y < map.Count; y++)
    {
        for (var x = 0; x < map[0].Length; x++)
        {
            output += map[y][x];
        }
        output += Environment.NewLine;
    }
    return output;
}

// ok, start moving!
Console.WriteLine("Before start:");
Console.WriteLine(GetMapStr(map));
foreach (var dir in directions)
{
    if (dir == '\n' || dir == '\r')
    {
        continue; // should be fixed by Trim() call above but just to be safe...
    }
    Console.WriteLine("Moving {0}", dir);
    var deltaX = 0;
    var deltaY = 0;
    if (dir == '<')
    {
        deltaX = -1;
    }
    else if (dir == '^')
    {
        deltaY = -1;
    }
    else if (dir == '>')
    {
        deltaX = 1;
    }
    else if (dir == 'v')
    {
        deltaY = 1;
    }
    var nextCh = map[robotY + deltaY][robotX + deltaX];
    if (nextCh == '#')
    {
        // nothing to do here
    }
    else if (nextCh == '.')
    {
        // move robot
        map[robotY][robotX] = '.';
        robotX = robotX + deltaX;
        robotY = robotY + deltaY;
        map[robotY][robotX] = '@';
    }
    else if (nextCh == 'O')
    {
        // move things if possible
        var canMove = false;
        var emptySpaceX = -1;
        var emptySpaceY = -1;
        var nextSpace = map[robotY + deltaY][robotX + deltaX];
        var checkX = robotX + deltaX;
        var checkY = robotY + deltaY;
        while (nextSpace != '#')
        {
            if (map[checkY][checkX] == '.')
            {
                canMove = true;
                emptySpaceX = checkX;
                emptySpaceY = checkY;
                break;
            }
            checkX = checkX + deltaX;
            checkY = checkY + deltaY;
            nextSpace = map[checkY][checkX];
        }
        Console.WriteLine("Can we move? {0}; emptyspace = {1},{2}", canMove, emptySpaceX, emptySpaceY);
        Console.WriteLine("   Robot = {0},{1}", robotX, robotY);
        if (canMove)
        {
            // move!
            while (true)
            {
                Console.WriteLine("Map {0},{1} = {2},{3}", emptySpaceX, emptySpaceY, emptySpaceX - deltaX, emptySpaceY - deltaY);
                map[emptySpaceY][emptySpaceX] = map[emptySpaceY - deltaY][emptySpaceX - deltaX];
                emptySpaceY -= deltaY;
                emptySpaceX -= deltaX;
                if (emptySpaceX == robotX && emptySpaceY == robotY)
                {
                    break;
                }
            }
            // for (var y = emptySpaceY; y >= 0 && y < map.Count && y != robotY; y -= deltaY)
            // {
            //     for (var x = emptySpaceX; x >= 0 && x < map[0].Length && x != robotX; x -= deltaX)
            //     {
            //         Console.WriteLine("Map {0},{1} = {2},{3}", y, x, y - deltaY, x - deltaX);
            //         map[y][x] = map[y - deltaY][x - deltaX];
            //     }
            // }
            map[robotY][robotX] = '.';
            robotX = robotX + deltaX;
            robotY = robotY + deltaY;
            map[robotY][robotX] = '@';
        }
    }
    // Console.WriteLine("After move:");
    // Console.WriteLine(GetMapStr(map));
}
Console.WriteLine(GetMapStr(map));
// now get total
long total = 0;
for (var y = 0; y < map.Count; y++)
{
    for (var x = 0; x < map[0].Length; x++)
    {
        var ch = map[y][x];
        if (ch == 'O')
        {
            // Console.WriteLine("{0},{1} has GPS total: {2}", x, y, y * 100 + x);
            total += y * 100 + x;
        }
    }
}
Console.WriteLine("GPS total: {0}", total);