// var data = File.ReadAllText("input.txt");
// var data = File.ReadAllText("example.txt");
var data = File.ReadAllText("input-from-reddit.txt");

void PrintFiles(List<DiskFile> blocks)
{
    foreach (var block in blocks)
    {
        Console.Write(block.ID);
        for (var i = 0; i < block.Size - 1; i++)
        {
            Console.Write(block.ID);
        }
        for (var i = 0; i < block.FreeSpace; i++)
        {
            Console.Write('.');
        }
    }
    Console.WriteLine();
}

void PrintBlockData(List<BlockData> bd)
{
    foreach (var block in bd)
    {
        if (block.Data == null)
        {
            Console.Write('.');
        }
        else
        {
            Console.Write(block.Data.ID);
        }
    }
    Console.WriteLine();
    for (var i = 0; i < bd.Count; i++)
    {
        Console.Write(i % 10);
    }
    Console.WriteLine();
}
var watch = System.Diagnostics.Stopwatch.StartNew();
var id = 0;
double fileSize = 0;
DiskFile? prev = null;
var blocks = new List<DiskFile>();
for (var i = 0; i < data.Length; i++)
{
    var isReadingFileSize = i == 0 || i % 2 == 0;
    fileSize = isReadingFileSize ? char.GetNumericValue(data[i]) : fileSize;
    if (!isReadingFileSize || i == data.Length - 1)
    {
        var freeSpace = i == data.Length - 1 ? 0 : (int)char.GetNumericValue(data[i]); // might not always be great but OK for now
        var diskFile = new DiskFile()
        {
            ID = id,
            Size = (int)fileSize,
            FreeSpace = freeSpace,
            Prev = prev,
            Next = null,
        };
        if (prev != null)
        {
            prev.Next = diskFile;
        }
        prev = diskFile;
        id++;
        blocks.Add(diskFile);
    }
}
// form block data
List<BlockData> GetBlockData(List<DiskFile> df)
{
    var blockData = new List<BlockData>();
    foreach (var block in df)
    {
        blockData.Add(new BlockData() { Data = block });
        for (var i = 0; i < block.Size - 1; i++)
        {
            blockData.Add(new BlockData() { Data = block });
        }
        for (var i = 0; i < block.FreeSpace; i++)
        {
            blockData.Add(new BlockData() { Data = null });
        }
    }
    return blockData;
}
var blockData = GetBlockData(blocks);
// move blocks
var firstIdx = 0;
var lastIdx = blockData.Count - 1;
// PrintBlockData(blockData);
while (true)
{
    if (firstIdx >= lastIdx)
    {
        break;
    }
    if (blockData[firstIdx].Data != null)
    {
        firstIdx++; // no empty space to place
    }
    else
    {
        // assuming no empty space at end for now
        blockData[firstIdx].Data = blockData[lastIdx].Data; // move end to empty space
        blockData[lastIdx].Data = null; // nothing at end now
        firstIdx++;
        lastIdx--;
        while (blockData[lastIdx].Data == null)
        {
            if (firstIdx >= lastIdx)
            {
                break;
            }
            lastIdx--;
        }
        // PrintBlockData(blockData);
    }
}
// print
// PrintFiles(blocks);
// PrintBlockData(blockData);
// calculate checksum
long GetChecksum(List<BlockData> bd)
{
    long total = 0;
    for (var i = 0; i < bd.Count; i++)
    {
        if (bd[i].Data != null)
        {
            total += i * bd[i].Data?.ID ?? 0;
        }
    }
    return total;
}
watch.Stop();
Console.WriteLine("Part 1: {0} in {1}ms", GetChecksum(blockData), watch.ElapsedMilliseconds);
// now do part 2
watch = System.Diagnostics.Stopwatch.StartNew();
// re-calculate block data for a fresh, clean slate
blockData = GetBlockData(blocks);
// PrintBlockData(blockData);

// get start and size of last block we are about to move
(int, int) MoveBackToNextBlock(int index, List<BlockData> blockData)
{
    if (index < 0)
    {
        return (-1, 0);
    }
    var blockToMoveSize = 0;
    var initialBlockID = blockData[index].Data?.ID ?? 999;
    // Console.WriteLine("initial block id is {0}", initialBlockID);
    while (index >= 0 && blockData[index].Data != null && blockData[index].Data!.ID == initialBlockID)
    {
        index--;
        blockToMoveSize++;
    }
    return (index + 1 /* we want start of the block */, blockToMoveSize);
}

(int, int) MoveToNextFreeIndex(int startIndex, List<BlockData> blockData)
{
    var freeSpaceIdx = startIndex;
    // move freeSpaceIdx to next empty space
    while (freeSpaceIdx < blockData.Count && blockData[freeSpaceIdx].Data != null)
    {
        freeSpaceIdx++;
    }
    // get first empty block size
    var freeBlockSize = 0;
    if (freeSpaceIdx < blockData.Count)
    {
        for (var i = freeSpaceIdx; i < blockData.Count; i++)
        {
            if (blockData[i].Data != null)
            {
                break;
            }
            freeBlockSize++;
        }
    }
    return (freeSpaceIdx, freeBlockSize);
}

(lastIdx, var blockToMoveSize) = MoveBackToNextBlock(blockData.Count - 1, blockData);
(var freeSpaceIdx, var freeBlockSize) = MoveToNextFreeIndex(0, blockData);
// Console.WriteLine("First block size is {0} starting at {1}", freeBlockSize, freeSpaceIdx);
// Console.WriteLine("Last block size is {0} starting at {1}", blockToMoveSize, lastIdx);
// see if we can move the current last block ANYWHERE
// one optimization would be to not retry to move blocks we've already moved
while (lastIdx >= 0)
{
    // attempt to move current block
    // Console.WriteLine("Going to try to move block with ID {0}", blockData[lastIdx].ID2Str());
    var didMove = false;
    while (freeSpaceIdx < blockData.Count && freeSpaceIdx < lastIdx)
    {
        if (freeBlockSize >= blockToMoveSize)
        {
            // move da block
            // Console.WriteLine("----before----");
            // PrintBlockData(blockData);
            for (var j = lastIdx; j < lastIdx + blockToMoveSize; j++)
            {
                blockData[freeSpaceIdx++].Data = blockData[j].Data;
                blockData[j].Data = null;
                // PrintBlockData(blockData);
            }
            // Console.WriteLine("-----------------done");
            didMove = true;
            break; // we moved the block, restart at beginning with next block to move
        }
        else
        {
            // try next empty spot
            while (freeSpaceIdx < blockData.Count && blockData[freeSpaceIdx].Data == null)
            {
                freeSpaceIdx++; // move to next non-empty spot (since we are on empty now)
            }
            (freeSpaceIdx, freeBlockSize) = MoveToNextFreeIndex(freeSpaceIdx, blockData); // find next free space
        }
    }
    // ok, we either moved it or we gave up. move lastIndex to previous block.
    // move lastIndex to next block we're going to try to move
    // Console.WriteLine("moving lastIndex backwards from {0}; did we move? {1}", lastIdx, didMove);
    // PrintBlockData(blockData);
    freeSpaceIdx = 0; // reset free space index
    (freeSpaceIdx, freeBlockSize) = MoveToNextFreeIndex(freeSpaceIdx, blockData); // find next free space
    if (!didMove)
    {
        // need to move back to previous known block and give up on this one
        var currID = blockData[lastIdx--].Data!.ID;
        // Console.WriteLine("Viewing {0} at {1} with currID = {2}", blockData[lastIdx].ID2Str(), lastIdx, currID);
        //  && blockData[lastIdx].Data != null && blockData[lastIdx].Data!.ID != currID
        while (lastIdx >= 0)
        {
            if (blockData[lastIdx].Data != null && blockData[lastIdx].Data!.ID != currID)
            {
                break;
            }
            lastIdx--;
        }
    }
    else
    {
        while (lastIdx >= 0 && blockData[lastIdx].Data == null)
        {
            lastIdx--;
        }
    }
    // Console.WriteLine("lastIndex is now {0}", lastIdx);
    (lastIdx, blockToMoveSize) = MoveBackToNextBlock(lastIdx, blockData);
    // Console.WriteLine("lastIndex is now now {0}; {1}", lastIdx ,blockData[lastIdx].Data!.ID);
}
// PrintBlockData(blockData);
watch.Stop();
Console.WriteLine("Part 2: {0} in {1}ms", GetChecksum(blockData), watch.ElapsedMilliseconds);