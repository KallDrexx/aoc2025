// const string inputFile = "test-input.txt";
const string inputFile = "real-input.txt";

var grid = await GetGrid(inputFile);
var count = 0;
while (true)
{
    var iterationCount = RemoveRolls();
    count += iterationCount;

    if (iterationCount == 0)
    {
        break;
    }
}

Console.WriteLine(count);

int RemoveRolls()
{
    var count = 0;
    var removedRolls = new List<(int, int)>();
    for (var row = 0; row < grid.Length; row++)
    for (var col = 0; col < grid[row].Length; col++)
    {
        if (!grid[row][col])
        {
            continue;
        }

        var adjacentCount = 0;
        for (var rowCheck = row - 1; rowCheck <= row + 1; rowCheck++)
        for (var colCheck = col - 1; colCheck <= col + 1; colCheck++)
        {
            if (rowCheck < 0 || colCheck < 0 || rowCheck >= grid.Length || colCheck >= grid[rowCheck].Length)
            {
                continue; // out of bounds
            }

            if (rowCheck == row && colCheck == col)
            {
                continue;
            }

            if (grid[rowCheck][colCheck])
            {
                adjacentCount++;
            }
        }

        if (adjacentCount < 4)
        {
            count++;
            removedRolls.Add((row, col));
        }
    }

    foreach ((var row, var col) in removedRolls)
    {
        grid[row][col] = false;
    }

    return count;
}


async Task<bool[][]> GetGrid(string fileName)
{
    await using var stream = File.OpenRead(fileName);
    using var reader = new StreamReader(stream);

    var grid = new List<bool[]>();
    while (true)
    {
        var line = await reader.ReadLineAsync();
        if (line == null)
        {
            break;
        }

        var span = line.AsSpan().Trim();
        var lineData = new bool[span.Length];
        for (var x = 0; x < span.Length; x++)
        {
            lineData[x] = span[x] == '@';
        }

        grid.Add(lineData);
    }

    return grid.ToArray();
}