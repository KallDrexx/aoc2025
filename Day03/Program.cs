// var inputFileName = "test-input.txt";
var inputFileName = "real-input.txt";

await using var stream = File.OpenRead(inputFileName);
using var reader = new StreamReader(stream);

var digitCount = 12;
ulong sum = 0;
while (true)
{
    var line = await reader.ReadLineAsync();
    if (line == null)
    {
        break;
    }
    
    var lineDigits = line.Select(x => int.Parse(x.ToString())).ToArray();
    var resultDigit = new int[digitCount];
    var searchStartIndex = 0;
    for (var x = 0; x < resultDigit.Length; x++)
    {
        var currentMax = 0;
        var searchLastIndex = lineDigits.Length - 1 - (digitCount - 1 - x);
        for (var y = searchStartIndex; y <= searchLastIndex; y++)
        {
            if (lineDigits[y] > currentMax)
            {
                currentMax = lineDigits[y];
                searchStartIndex = y + 1;
            }
        }

        resultDigit[x] = currentMax;
    }
    
    ulong finalNumber = 0;
    for (var x = 0; x < digitCount; x++)
    {
        finalNumber = finalNumber * 10 + (ulong)resultDigit[x];
    }
    
    Console.WriteLine(finalNumber);
    sum += finalNumber;
}

Console.WriteLine(sum);