using System.Text.RegularExpressions;

var fileLines = File.ReadAllLines("input.txt");
// var fileLines = File.ReadAllLines("example.txt");

var machines = new List<Machine>();
Button? tmpButtonA = null;
Button? tmpButtonB = null;

(int, int) GetNums(string str)
{
    // https://stackoverflow.com/a/75628602/3938401
    Regex regex = new Regex(@"[\d]+");
    var matchCollection = regex.Matches(str);
    int firstNumber = int.Parse(matchCollection[0].Value);
    int secondNumber = int.Parse(matchCollection[1].Value);
    return (firstNumber, secondNumber);
}

foreach (var line in fileLines)
{
    if (line != "")
    {
        (int x, int y) = GetNums(line);
        if (line.StartsWith("Button A:"))
        {
            tmpButtonA = new Button(x, y);
        }
        else if (line.StartsWith("Button B"))
        {
            tmpButtonB = new Button(x, y);
        }
        else if (line.StartsWith("Prize:"))
        {
            var machine = new Machine(tmpButtonA!, tmpButtonB!, x, y);
            if (machine.IsSolvable())
            {
                machines.Add(machine);
            }
        }
    }
}
Console.WriteLine("We have {0} machines", machines.Count);
var total = 0;
foreach (var machine in machines)
{
    total += machine.GetLeastCost();
}
Console.WriteLine("Least cost: {0}", total);
