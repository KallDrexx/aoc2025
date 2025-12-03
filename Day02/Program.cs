// var inputFile = "test-input.txt";
var inputFile = "real-input.txt";
ulong sum = 0;
await foreach (var (first, second) in GetRanges(inputFile))
{
    for (var x = first; x <= second; x++)
    {
        var span = x.ToString().AsSpan();

        for (var patternLength = 1; patternLength <= span.Length / 2; patternLength++)
        {
            // Whole value must be a repeating number, so it must be divisible by the pattern length
            if (span.Length % patternLength != 0)
            {
                continue;
            }

            var searchFor = span[..patternLength];
            var testSpan = span[patternLength..];
            var allMatch = true;
            do
            {
                if (!testSpan.StartsWith(searchFor, StringComparison.OrdinalIgnoreCase))
                {
                    allMatch = false;
                    break;
                }

                testSpan = testSpan[patternLength..];
            } while (!testSpan.IsEmpty);

            if (allMatch)
            {
                sum += x;
                break;
            }
        }
    }
}

Console.WriteLine($"Total: {sum}");


static async IAsyncEnumerable<(ulong First, ulong Second)> GetRanges(string file)
{
    await using var stream = File.OpenRead(file);
    using var reader = new StreamReader(stream);

    // Only one line of input
    var line = await reader.ReadLineAsync() ?? string.Empty;

    var lineContents = line.AsMemory().Trim();
    while (!lineContents.IsEmpty)
    {
        var span = lineContents.Span;
        var nextDashIndex = span.IndexOf('-');
        if (nextDashIndex < 0)
        {
            break;
        }

        var nextCommaIndex = span.IndexOf(',');
        if (nextCommaIndex < 0)
        {
            // end of the line
            nextCommaIndex = span.Length;
        }

        var firstValue = span[..nextDashIndex];
        var secondValue = span.Slice(nextDashIndex + 1, nextCommaIndex - nextDashIndex - 1);

        var firstNumber = ulong.Parse(firstValue);
        var secondNumber = ulong.Parse(secondValue);

        if (nextCommaIndex == span.Length)
        {
            // end of the string
            lineContents = ReadOnlyMemory<char>.Empty;
        }
        else
        {
            lineContents = lineContents[(nextCommaIndex + 1)..];
        }

        yield return (firstNumber, secondNumber);
    }
}