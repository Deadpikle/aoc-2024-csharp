var fileLines = File.ReadLines("input.txt").ToList();
// var fileLines = File.ReadLines("example.txt").ToList();
// var fileLines = File.ReadLines("exampleSimple.txt").ToList();

bool IsValidCoord(int x, int y, List<string> fileLines)
{
    return x >= 0 && y >= 0 && x < fileLines[0].Length && y < fileLines.Count;
}

(Dictionary<char, List<Node>>, Dictionary<string, List<Node>>) GetAntinodes(List<string> fileLines, bool incorporateResonant)
{
    var nodes = new Dictionary<char, List<Node>>();
    var antinodes = new Dictionary<string, List<Node>>();
    for (var y = 0; y < fileLines.Count; y++)
    {
        var symbols = fileLines[y].ToCharArray();
        for (var x = 0; x < symbols.Length; x++)
        {
            var symbol = symbols[x];
            if (symbol != '.')
            {
                var node = new Node(symbol, x, y);
                if (!nodes.ContainsKey(symbol))
                {
                    nodes[symbol] = new List<Node>();
                    // Console.WriteLine("Symbol: {0}", symbol);
                }
                // Console.WriteLine("Adding node at {0}", node.GetKey());
                foreach (var existingNode in nodes[symbol])
                {
                    var nextNode = node;
                    // Console.WriteLine("-Checking existing node {0}", existingNode.GetKey());
                    var antinode = existingNode.GetAntinode(nextNode);
                    // Console.WriteLine("--Comparing {0} to {1} to get {2}", existingNode.GetKey(), nextNode.GetKey(), antinode.GetKey());
                    while (IsValidCoord(antinode.X, antinode.Y, fileLines))
                    {
                        var key = antinode.GetKey();
                        if (!antinodes.ContainsKey(key))
                        {
                            antinodes.Add(key, new List<Node>());
                        }
                        antinodes[key].Add(antinode);
                        if (!incorporateResonant)
                        {
                            break;
                        }
                        var tmp = antinode;
                        antinode = nextNode.GetAntinode(antinode);
                        nextNode = tmp;
                    }
                    // Console.WriteLine("--swapping direction");
                    nextNode = existingNode;
                    antinode = node.GetAntinode(existingNode);
                    // Console.WriteLine("--Comparing {0} to {1} to get {2}", node.GetKey(), existingNode.GetKey(), antinode.GetKey());
                    while (IsValidCoord(antinode.X, antinode.Y, fileLines))
                    {
                        var key = antinode.GetKey();
                        if (!antinodes.ContainsKey(key))
                        {
                            antinodes.Add(key, new List<Node>());
                        }
                        antinodes[key].Add(antinode);
                        if (!incorporateResonant)
                        {
                            break;
                        }
                        var tmp = antinode;
                        antinode = nextNode.GetAntinode(antinode);
                        nextNode = tmp;
                    }
                }
                nodes[symbol].Add(node);
            }
        }
    }
    // add in main nodes themselves
    if (incorporateResonant)
    {
        foreach (var nodeList in nodes)
        {
            if (nodeList.Value.Count > 1)
            {
                foreach (var nodeItem in nodeList.Value)
                {
                    var key = nodeItem.GetKey();
                    if (!antinodes.ContainsKey(key))
                    {
                        antinodes[key] = new List<Node>();
                    }
                    antinodes[key].Add(nodeItem);
                }
            }
        }
    }
    return (nodes, antinodes);
}
var antinodes = GetAntinodes(fileLines, false);
Console.WriteLine("Part 1 antinodes: {0}", antinodes.Item2.Count);
antinodes = GetAntinodes(fileLines, true);
Console.WriteLine("Part 2 antinodes: {0}", antinodes.Item2.Count);

// var allAntinodes = new List<Node>();
// create and print full map
// foreach (var antinodeList in antinodes.Item2)
// {
//     // Console.WriteLine("Antinode at {0}", antinodeList.Key);
//     allAntinodes.AddRange(antinodeList.Value);
// }
// List<List<char>> MakeMap(List<string> fileLines)
// {
//    var fullMap = new List<List<char>>();
//    for (var y = 0; y < fileLines.Count; y++)
//    {
//        var symbols = fileLines[y].ToCharArray();
//        var fullMapLine = new List<char>();
//        for (var x = 0; x < symbols.Length; x++)
//        {
//            var symbol = symbols[x];
//            fullMapLine.Add(symbol);
//        }
//        fullMap.Add(fullMapLine);
//    }
//    return fullMap;
// }
// var fullMap = MakeMap(fileLines);
// foreach (var antinode in allAntinodes)
// {
//     if (antinodes.Item1[antinode.Symbol].Where(x => x.GetKey() == antinode.GetKey()).Count() == 0)
//     {
//         fullMap[antinode.Y][antinode.X] = '#';
//     }
// }
// for (var i = 0; i < fullMap.Count; i++)
// {
//     Console.Write("{0,3} ", (i));
//     for (var j = 0; j < fullMap[i].Count; j++)
//     {
//         Console.Write(fullMap[i][j] + "");
//     }
//     Console.WriteLine();
// }
