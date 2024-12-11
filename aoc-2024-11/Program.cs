var stoneData = File.ReadAllText("input.txt");
// var stoneData = File.ReadAllText("example.txt");
// var stoneData = File.ReadAllText("example-2.txt");

var stoneNums = stoneData.Split(" ").Select(long.Parse);
var numBlinks = 75;

long GetStonesFromLinkedList(IEnumerable<long> stoneNums)
{
    var stones = new LinkedList<long>();
    foreach (var stoneNum in stoneNums)
    {
        stones.AddLast(stoneNum);
    }
    // now blink!
    for (var i = 0; i < numBlinks; i++)
    {
        Console.WriteLine("On blink {0}", i + 1);
        var curr = stones.First;
        for (LinkedListNode<long>? node = stones.First; node != null; node = node?.Next ?? null)
        {
            var stoneNum = node.Value;
            if (stoneNum == 0)
            {
                node.Value = 1;
            }
            else
            {
                var stoneNumStr = stoneNum.ToString();
                if (stoneNumStr.Length % 2 == 0)
                {
                    var firstPart = stoneNumStr.Substring(0, stoneNumStr.Length / 2);
                    var otherPart = stoneNumStr.Substring(stoneNumStr.Length / 2);
                    node.Value = long.Parse(firstPart);
                    stones.AddAfter(node, long.Parse(otherPart));
                    node = node?.Next ?? null;
                }
                else
                {
                    node.Value *= 2024;
                }
            }
        }
    }
    return stones.Count;
}

string MakeKey(int iteration, long number)
{
    return string.Format("{0}|{1}", iteration, number);
}

long GetStoneCountViaQueue(IEnumerable<long> stoneNums)
{
    // var queue = new Queue<Stone>();
    var queue = new PriorityQueue<Stone,int>(Comparer<int>.Create((a,b)=>b-a));
    long total = 0;
    foreach (var stone in stoneNums)
    {
        queue.Enqueue(new Stone(stone, 0), 0);
    }
    // queue.Enqueue(new Stone(0, 0));
    // Console.WriteLine("Queue count is initially {0}", queue.Count);
    var cache = new Dictionary<string, (int, List<Stone>)>();
    long cacheHits = 0;
    while (queue.Count > 0)
    {
        // process stone
        var stone = queue.Dequeue();
        total++;
        if (queue.Count % 10000 == 0)
        {
            Console.WriteLine("Queue size: {0:n0}; cache hits: {1:n0}; cache size: {2}; last # is {3} with iteration {4}", queue.Count, cacheHits, cache.Count, queue.Count > 0 ? queue.Peek().Number : "N/A", 
            queue.Count > 0 ? queue.Peek().CurrIteration : "N/A");
        }
        if (total % 10000 == 0)
        {
            Console.WriteLine("Queue size: {0:n0}; cache hits: {1:n0}; cache size: {2}; last # is {3} with iteration {4}; total is {5:n0}", queue.Count, cacheHits, cache.Count, queue.Count > 0 ? queue.Peek().Number : "N/A", 
            queue.Count > 0 ? queue.Peek().CurrIteration : "N/A", total);
        }
        var cacheCreator = new Dictionary<string, int>();
        var initialNum = stone.Number;
        var initialIteration = stone.CurrIteration;
        var numStonesCreated = 0;
        var stonesCreated = new List<Stone>();
        // Console.WriteLine("after deque, Count is {0} for stone {1}", queue.Count, stone.Number);
        var didUseCache = false;
        for (var i = stone.CurrIteration; i < numBlinks; i++)
        {
            var iterKey = MakeKey(i, stone.Number);
            if (cache.ContainsKey(iterKey))
            {
                total += cache[iterKey].Item1;
                // foreach (var s in cache[iterKey].Item2)
                // {
                //     queue.Enqueue(s);
                // }
                didUseCache = true;
                cacheHits++;
                // Console.WriteLine("Cache hit: {0}; {1}", ++cacheHits, iterKey);
                break;
            }
            // cache may be slightly wrong, not sure -- yes it is; it needs to account for what other stones make, too
            if (stone.Number == 0)
            {
                stone.Number = 1;
            }
            else
            {
                var stoneNumStr = stone.Number.ToString();
                if (stoneNumStr.Length % 2 == 0)
                {
                    var firstPart = stoneNumStr.Substring(0, stoneNumStr.Length / 2);
                    var otherPart = stoneNumStr.Substring(stoneNumStr.Length / 2);
                    stone.Number = long.Parse(firstPart);
                    var nextNumber = long.Parse(otherPart);
                    var nextStone = new Stone(nextNumber, i + 1);
                    queue.Enqueue(nextStone, i + 1);
                    numStonesCreated++;
                    stonesCreated.Add(nextStone);
                }
                else
                {
                    stone.Number *= 2024;
                }
            }
        }
        var key = MakeKey(initialIteration, initialNum);
        if (!didUseCache && initialIteration != numBlinks && !cache.ContainsKey(key))
        {
            // Console.WriteLine("{0} -> {1}", key, numStonesCreated);
            //cache.Add(key, (numStonesCreated, stonesCreated));
        }
        // Console.WriteLine("We created {0} stones", numStonesCreated);
        // Console.WriteLine("After processing stone, queue count is {0} and final stone # is {1}", queue.Count, stone.Number);
    }
    return total;
}

long GetStoneCountViaDoNotKnowYet(IEnumerable<long> stoneNums)
{
    long GetTotal(Stone stone)
    {
        // HOW TO CACHE RESULTSSSS
        long total = 1; // for stone passed in
        for (var i = stone.CurrIteration; i < numBlinks; i++)
        {
            if (stone.Number == 0)
            {
                stone.Number = 1;
            }
            else
            {
                var stoneNumStr = stone.Number.ToString();
                if (stoneNumStr.Length % 2 == 0)
                {
                    var firstPart = stoneNumStr.Substring(0, stoneNumStr.Length / 2);
                    var otherPart = stoneNumStr.Substring(stoneNumStr.Length / 2);
                    stone.Number = long.Parse(firstPart);
                    var nextNumber = long.Parse(otherPart);
                    var nextStone = new Stone(nextNumber, i + 1);
                    total += GetTotal(nextStone);
                }
                else
                {
                    stone.Number *= 2024;
                }
            }
        }
        return total;
    }
    long total = 0;
    foreach (var stone in stoneNums)
    {
        total += GetTotal(new Stone(stone, 0));
    }
    return total;
}

long GetStonesFromDict(IEnumerable<long> stoneNums)
{
    void UpdateCount(long num, long numStones, Dictionary<long, long> data)
    {
        if (!data.ContainsKey(num))
        {
            data[num] = 0;
        }
        // Console.WriteLine("    Adding {0} stones to {1}", numStones, num);
        data[num] += numStones;
    }
    var currentIter = new Dictionary<long, long>();
    long totalStones = 0;
    foreach (var stoneNum in stoneNums)
    {
        UpdateCount(stoneNum, 1, currentIter);
        totalStones++;
    }
    // now blink!
    for (var i = 0; i < numBlinks; i++)
    {
        // Console.WriteLine("=====On blink {0}=====", i + 1);
        var nextIter = new Dictionary<long, long>();
        foreach(KeyValuePair<long, long> entry in currentIter)
        {
            var stoneNum = entry.Key;
            if (stoneNum == 0)
            {
                // Console.WriteLine("0 -> 1");
                UpdateCount(1, entry.Value, nextIter);
            }
            else
            {
                var stoneNumStr = stoneNum.ToString();
                if (stoneNumStr.Length % 2 == 0)
                {
                    var firstPart = stoneNumStr.Substring(0, stoneNumStr.Length / 2);
                    var otherPart = stoneNumStr.Substring(stoneNumStr.Length / 2);
                    // Console.WriteLine("{0} -> {1} & {2}", stoneNumStr, firstPart, otherPart);
                    UpdateCount(long.Parse(firstPart), entry.Value, nextIter);
                    UpdateCount(long.Parse(otherPart), entry.Value, nextIter);
                    totalStones += entry.Value;
                }
                else
                {
                    // Console.WriteLine("{0} -> {1}", stoneNum, stoneNum * 2024);
                    UpdateCount(stoneNum * 2024, entry.Value, nextIter);
                }
            }
        }
        currentIter = nextIter;
    }
    return totalStones;
    //long total = 0;
    //// Console.WriteLine("----");
    //foreach(KeyValuePair<long, long> entry in currentIter)
    //{
    //    // Console.WriteLine(entry.Key + " -> " + entry.Value);
    //    total += entry.Value;
    //}
    //return total;
}


// Console.WriteLine("We have {0} stones", GetStonesFromLinkedList(stoneNums));
// Console.WriteLine("We have {0} stones", GetStoneCountViaQueue(stoneNums));
// Console.WriteLine("We have {0} stones", GetStoneCountViaDoNotKnowYet(stoneNums));
var watch = System.Diagnostics.Stopwatch.StartNew();
var amnt = GetStonesFromDict(stoneNums); // this is the one that runs the best. GetStonesFromLinkedList ran part 1.
watch.Stop();
Console.WriteLine("We have {0:n0} stones", amnt);
Console.WriteLine("{0}ms", watch.ElapsedMilliseconds);
// foreach (var stone in stones)
// {
//     Console.Write(stone + " ");
// }
// Console.WriteLine();