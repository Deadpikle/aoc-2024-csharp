var fileLines = File.ReadAllLines("input.txt");
// var fileLines = File.ReadAllLines("example-01.txt");
List<List<char>> fullGrid = new List<List<char>>();
foreach (var line in fileLines)
{
    var charArr = line.ToCharArray();
    fullGrid.Add(charArr.ToList());
}
var total = 0;
var height = fullGrid.Count;
for (var i = 0; i < fullGrid.Count; i++)
{
    var width = fullGrid[i].Count;
    for (var j = 0; j < width; j++)
    {
        if (fullGrid[i][j] == 'X')
        {
            // check forward
            if (j + 3 < width && fullGrid[i][j + 1] == 'M' && fullGrid[i][j + 2] == 'A' && fullGrid[i][j + 3] == 'S')
            {
                total++;
            }
            // check backward
            if (j - 3 >= 0 && fullGrid[i][j - 1] == 'M' && fullGrid[i][j - 2] == 'A' && fullGrid[i][j - 3] == 'S')
            {
                total++;
            }
            // check up
            if (i + 3 < height && fullGrid[i + 1][j] == 'M' && fullGrid[i + 2][j] == 'A' && fullGrid[i + 3][j] == 'S')
            {
                total++;
            }
            // check down
            if (i - 3 >= 0 && fullGrid[i - 1][j] == 'M' && fullGrid[i - 2][j] == 'A' && fullGrid[i - 3][j] == 'S')
            {
                total++;
            }
            // check diagonal right up
            if (j + 3 < width && i - 3 >= 0 && fullGrid[i - 1][j + 1] == 'M' && fullGrid[i - 2][j + 2] == 'A' && fullGrid[i - 3][j + 3] == 'S')
            {
                total++;
            }
            // check diagonal right down
            if (j + 3 < width && i + 3 < height && fullGrid[i + 1][j + 1] == 'M' && fullGrid[i + 2][j + 2] == 'A' && fullGrid[i + 3][j + 3] == 'S')
            {
                total++;
            }
            // check diagonal left up
            if (j - 3 >= 0 && i - 3 >= 0 && fullGrid[i - 1][j - 1] == 'M' && fullGrid[i - 2][j - 2] == 'A' && fullGrid[i - 3][j - 3] == 'S')
            {
                total++;
            }
            // check diagonal left down
            if (j - 3 >= 0 && i + 3 < height && fullGrid[i + 1][j - 1] == 'M' && fullGrid[i + 2][j - 2] == 'A' && fullGrid[i + 3][j - 3] == 'S')
            {
                total++;
            }
        }
    }
}
Console.WriteLine("Total for part 1 = {0}", total);
/////
///
total = 0;
for (var i = 0; i < fullGrid.Count; i++)
{
    var width = fullGrid[i].Count;
    for (var j = 0; j < width; j++)
    {
        if (fullGrid[i][j] == 'A')
        {
            if (i - 1 >= 0 && i + 1 < height && j - 1 >= 0 && j + 1 < width)
            {
                if (((fullGrid[i - 1][j - 1] == 'M' && fullGrid[i + 1][j + 1] == 'S') ||
                     (fullGrid[i - 1][j - 1] == 'S' && fullGrid[i + 1][j + 1] == 'M')) &&
                   ((fullGrid[i - 1][j + 1] == 'M' && fullGrid[i + 1][j - 1] == 'S') ||
                    (fullGrid[i - 1][j + 1] == 'S' && fullGrid[i + 1][j - 1] == 'M')))
                {
                    total++;
                }
            }
        }
    }
}
Console.WriteLine("Total for part 2 = {0}", total);