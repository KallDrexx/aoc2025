namespace Day01;

public static class TurnReader
{
    public static async IAsyncEnumerable<int> GetTurns(string inputFile)
    {
        using var file = File.OpenRead(inputFile);
        using var reader = new StreamReader(file);

        string? line;
        var lineNumber = -1;
        while ((line = await reader.ReadLineAsync()) != null)
        {
            lineNumber++;
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            yield return ParseLine(line, lineNumber);
        }
    }

    private static int ParseLine(string line, int lineNumber)
    {
        var modifier = 0;
        if (line.StartsWith('R'))
        {
            modifier = 1;
        }
        else if (line.StartsWith('L'))
        {
            modifier = -1;
        }
        else
        {
            var message = $"Unexpected content line #{lineNumber}: '{line}'";
            throw new InvalidOperationException(message);
        }

        if (!int.TryParse(line.AsSpan(1), out var number))
        {
            var message = $"Unexpected content line #{lineNumber}: '{line}'";
            throw new InvalidOperationException(message);
        }

        return number * modifier;
    }
}