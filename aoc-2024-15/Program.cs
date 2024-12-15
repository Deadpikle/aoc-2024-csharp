
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
(int, int) GetRobotLoc(List<char[]> map)
{
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
    return (robotX, robotY);
}

(var robotX, var robotY) = GetRobotLoc(map);

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

(int, int) GetDeltaFromChar(char dir)
{
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
    return (deltaX, deltaY);
}

(int, int) DoOneBlockMove(int robotX, int robotY, int deltaX, int deltaY, List<char[]> map)
{
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
    // Console.WriteLine("Can we move? {0}; emptyspace = {1},{2}", canMove, emptySpaceX, emptySpaceY);
    // Console.WriteLine("   Robot = {0},{1}", robotX, robotY);
    if (canMove)
    {
        // move!
        while (true)
        {
            // Console.WriteLine("Map {0},{1} = {2},{3}", emptySpaceX, emptySpaceY, emptySpaceX - deltaX, emptySpaceY - deltaY);
            map[emptySpaceY][emptySpaceX] = map[emptySpaceY - deltaY][emptySpaceX - deltaX];
            emptySpaceY -= deltaY;
            emptySpaceX -= deltaX;
            if (emptySpaceX == robotX && emptySpaceY == robotY)
            {
                break;
            }
        }
        map[robotY][robotX] = '.';
        robotX = robotX + deltaX;
        robotY = robotY + deltaY;
        map[robotY][robotX] = '@';
    }
    return (robotX, robotY);
}

// ok, start moving!
Console.WriteLine("Before start Part 1:");
Console.WriteLine(GetMapStr(map));
foreach (var dir in directions)
{
    if (dir == '\n' || dir == '\r')
    {
        continue; // should be fixed by Trim() call above but just to be safe...
    }
    (var deltaX, var deltaY) = GetDeltaFromChar(dir);
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
        (robotX, robotY) = DoOneBlockMove(robotX, robotY, deltaX, deltaY, map);
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
Console.WriteLine("GPS total Part 1: {0}", total);
// Part 2
// Recreate map with 2x size
map = new List<char[]>();
foreach (var line in fileLines)
{
    if (line == "")
    {
        break; // don't need directions again
    }
    else
    {
        var chLine = new char[line.Length * 2];
        var pos = 0;
        foreach (var ch in line)
        {
            if (ch == '#')
            {
                chLine[pos] = '#';
                chLine[pos + 1] = '#';
                pos += 2;
            }
            else if (ch == 'O')
            {
                chLine[pos] = '[';
                chLine[pos + 1] = ']';
                pos += 2;
            }
            else if (ch == '.')
            {
                chLine[pos] = '.';
                chLine[pos + 1] = '.';
                pos += 2;
            }
            else if (ch == '@')
            {
                chLine[pos] = '@';
                chLine[pos + 1] = '.';
                pos += 2;
            }
        }
        map.Add(chLine);
    }
}

Console.WriteLine("------------------");

Console.WriteLine("Before start Part 2:");
Console.WriteLine(GetMapStr(map));

(robotX, robotY) = GetRobotLoc(map);
foreach (var dir in directions)
{
    if (dir == '\n' || dir == '\r')
    {
        continue; // should be fixed by Trim() call above but just to be safe...
    }
    (var deltaX, var deltaY) = GetDeltaFromChar(dir);

    var nextCh = map[robotY + deltaY][robotX + deltaX];
    // Console.WriteLine("Moving {0}; nextCh is {1}", dir, nextCh);
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
    else if (nextCh == '[' || nextCh == ']')
    {
        // move if we can!
        if (dir == '<' || dir == '>')
        {
            // old algorithm will work as it's always moving just a line of blocks
            (robotX, robotY) = DoOneBlockMove(robotX, robotY, deltaX, deltaY, map);
        }
        else
        {
            bool CanPush2SizeBox(int leftX, int rightX, int y, int deltaY)
            {
                // Console.WriteLine("  Checking if we can push box at left {0}, right {1}, y {2}, deltaY {3}", leftX, rightX, y, deltaY);
                var aboveLeftBox = map[y + deltaY][leftX];
                // Console.WriteLine("  aboveLeftBox is {0}", aboveLeftBox);
                var canPushLeft = aboveLeftBox == '.' ? true : 
                    (aboveLeftBox == ']' || aboveLeftBox == '[' 
                        ? CanPush2SizeBox(aboveLeftBox == ']' ? leftX - 1 : leftX, 
                                          aboveLeftBox == ']' ? leftX : leftX + 1,
                                           y + deltaY, deltaY)
                        : false);
                var aboveRightBox = map[y + deltaY][rightX];
                // Console.WriteLine("  aboveRightBox is {0}", aboveRightBox);
                var canPushRight = 
                    aboveRightBox == '.' ? true 
                    : (aboveRightBox == ']' || aboveRightBox == '[' 
                        ? CanPush2SizeBox(aboveRightBox == ']' ? rightX - 1 : rightX, 
                                          aboveRightBox == ']' ? rightX : rightX + 1,
                                           y + deltaY, deltaY)
                        : false);
                return canPushLeft & canPushRight;
            }
            // get left and right of box
            var leftBoxX = nextCh == '[' ? robotX : robotX - 1;
            var rightBoxX = nextCh == '[' ? robotX + 1 : robotX;
            var otherY = robotY + deltaY;
            var canPush = CanPush2SizeBox(leftBoxX, rightBoxX, otherY, deltaY);
            // Console.WriteLine("Can we push? {0}", canPush);
            if (canPush)
            {
                // push!
                void Push2SizeBox(int x, int y, int deltaX, int deltaY)
                {
                    var ch = map[y][x];
                    if (ch != '.' && ch != '#')
                    {
                        var otherX = ch == '[' ? x + 1 : x - 1;
                        if (map[y + deltaY][x + deltaX] == '.' && map[y + deltaY][otherX + deltaX] == '.') // both sides free?
                        {
                            // move!
                            map[y + deltaY][x + deltaX] = map[y][x];
                            map[y + deltaY][otherX + deltaX] = map[y][otherX];
                            map[y][x] = '.';
                            map[y][otherX] = '.';
                        }
                        else
                        {
                            // recursion!
                            Push2SizeBox(x + deltaX, y + deltaY, deltaX, deltaY);
                            Push2SizeBox(otherX + deltaX, y + deltaY, deltaX, deltaY);
                            // now can move the immediate box
                            map[y + deltaY][x + deltaX] = map[y][x];
                            map[y + deltaY][otherX + deltaX] = map[y][otherX];
                            map[y][x] = '.';
                            map[y][otherX] = '.';
                        }
                    }
                }
                Push2SizeBox(robotX + deltaX, robotY + deltaY, deltaX, deltaY);
                // move robot
                map[robotY][robotX] = '.';
                robotX = robotX + deltaX;
                robotY = robotY + deltaY;
                map[robotY][robotX] = '@';
            }
        }
    }
    // Console.WriteLine("After move:");
    // Console.WriteLine(GetMapStr(map));
}
Console.WriteLine("After Part 2:");
Console.WriteLine(GetMapStr(map));
// now get total
total = 0;
for (var y = 0; y < map.Count; y++)
{
    for (var x = 0; x < map[0].Length; x++)
    {
        var ch = map[y][x];
        if (ch == '[')
        {
            // Console.WriteLine("{0},{1} has GPS total: {2}", x, y, y * 100 + x);
            total += y * 100 + x;
        }
    }
}
Console.WriteLine("Doubled map size is: {0}", total);