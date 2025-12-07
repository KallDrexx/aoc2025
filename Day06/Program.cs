// const string inputFile = "test-input.txt";
const string inputFile = "real-input.txt";

var input = await GetInput(inputFile);
ulong overallTotal = 0;

foreach (var column in input.Columns)
{
    ulong columnTotal = 0;

    for (var innerCol = 0; innerCol < column.LongestLength; innerCol++)
    {
        ulong number = 0;
        foreach (var line in input.Lines)
        {
            var slice = column.Length != null
                ? line.AsSpan().Slice(column.StartIndex, column.Length.Value)
                : line.AsSpan()[column.StartIndex..];

            if (innerCol < slice.Length)
            {
                if (ulong.TryParse(slice[innerCol].ToString(), out var digit))
                {
                    number = number * 10 + digit;
                }
            }
        }

        if (number == 0)
        {
            // No values, ignore)
            continue;
        }

        if (column.Operator == Operator.Add)
        {
            Console.Write("+ ");
            columnTotal += number;
        }
        else
        {
            Console.Write("* ");
            columnTotal = columnTotal == 0 ? number : columnTotal * number;
        }

        Console.Write($"{number} ");
    }

    Console.WriteLine($" = {columnTotal}");
    overallTotal += columnTotal;
}

Console.WriteLine(overallTotal);

return;

async Task<Input> GetInput(string filename)
{
    await using var stream = File.OpenRead(filename);
    using var reader = new StreamReader(stream);

    var lines = new List<string>();
    var longestLineLength = 0;
    while (true)
    {
        var line = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(line))
        {
            break;
        }

        lines.Add(line);
        if (line.Length > longestLineLength)
        {
            longestLineLength = line.Length;
        }
    }

    var lastLine = lines[^1];
    lines.RemoveAt(lines.Count - 1);

    // Assume operator always designates first column
    var data = lastLine
        .Select((character, index) => new { Character = character, Index = index })
        .Where(x => !char.IsWhiteSpace(x.Character))
        .ToArray();

    var columns = new List<Column>();
    for (var x = 0; x < data.Length; x++)
    {
        var op = data[x].Character switch
        {
            '+' => Operator.Add,
            '*' => Operator.Multiply,
            _ => throw new InvalidOperationException("Invalid operator"),
        };

        int? length;
        int longestColumnLength;
        if (x < data.Length - 1)
        {
            length = data[x + 1].Index - data[x].Index;
            longestColumnLength = length.Value;
        }
        else
        {
            length = null;
            longestColumnLength = longestLineLength - data[x].Index;
        }

        columns.Add(new Column(op, data[x].Index, length, longestColumnLength));
    }

    return new Input(lines, columns);
}

public record Column(Operator Operator, int StartIndex, int? Length, int LongestLength);

public record Input(IReadOnlyList<string> Lines, IReadOnlyList<Column> Columns);

public enum Operator { Add, Multiply }
