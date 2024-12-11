var fileLines = File.ReadAllLines("input.txt");
// fileLines = File.ReadAllLines("input-01-example.txt");
bool checkLine(string line)
{
    var levels = line.Split(" ");
    var prevLevel = -1;
    var didPass = true;
    var isDirectionKnown = false;
    var overallDirection = 0;
    for (var i = 0; i < levels.Length; i++)
    {
        var currentLevel = int.Parse(levels[i]);
        if (i > 0) 
        {
            if (!isDirectionKnown)
            {
                overallDirection = currentLevel > prevLevel ? 1 : -1;
                isDirectionKnown = true;
            }
            var direction = currentLevel > prevLevel ? 1 : -1;
            var diff = Math.Abs(currentLevel - prevLevel);
            if (diff < 1 || diff > 3 || direction != overallDirection)
            {
                didPass = false;
                break;
            }
        }
        prevLevel = currentLevel;
    }
    return didPass;
}
int getSafeReports(string[] fileLines, bool useDampener = false)
{
    var safeReports = 0;
    foreach (var levelLine in fileLines)
    {
        var didPass = checkLine(levelLine);
        if (didPass)
        {
            safeReports++;
        }
        else if (useDampener)
        {
            // modify level line one item at a time to see if it's usable
            var levels = levelLine.Split(" ");
            for (var i = 0; i < levels.Length; i++)
            {
                var rebuiltLevel = "";
                var levelsToUse = new List<string>();
                for (var j = 0; j < levels.Length; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    levelsToUse.Add(levels[j]);
                }
                rebuiltLevel = string.Join(" ", levelsToUse);
                didPass = checkLine(rebuiltLevel);
                if (didPass)
                {
                    safeReports++;
                    break; // continue to next level line
                }
            }
        }
    }
    return safeReports;
}
var safeReports = getSafeReports(fileLines, false);
Console.WriteLine("Safe reports without dampener: " + safeReports);
safeReports = getSafeReports(fileLines, true);
Console.WriteLine("Safe reports with dampener: " + safeReports);