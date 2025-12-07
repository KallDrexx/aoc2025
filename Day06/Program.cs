// const string inputFile = "test-input.txt";
const string inputFile = "real-input.txt";

var (grid, operators) = await GetGrid(inputFile);
ulong overallTotal = 0;

for (var col = 0; col < grid[0].Length; col++)
{
    ulong colTotal = 0;
    var op = operators[col];

    for (var row = 0; row < grid.Length; row++)
    {
        var number = grid[row][col];
        if (colTotal == 0)
        {
            colTotal = number;
        }
        else
        {
            colTotal = op == Operator.Add ? colTotal + number : colTotal * number;
        }
    }

    overallTotal += colTotal;
}

Console.WriteLine(overallTotal);

return;

async Task<(ulong[][] Values, Operator[])> GetGrid(string filename)
{
    await using var stream = File.OpenRead(filename);
    using var reader = new StreamReader(stream);

    var grid = new List<ulong[]>();
    var operators = new List<Operator>();
    while (true)
    {
        var line = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(line))
        {
            break;
        }

        var entry = new List<ulong>();
        var span = line.AsSpan();
        bool? isOperator = null;
        foreach (var part in span.Split(' '))
        {
            var spanPart = span[part].Trim();
            if (spanPart.IsEmpty || spanPart.IsWhiteSpace())
            {
                continue;
            }

            if (isOperator == null)
            {
                if (operators.Count > 0)
                {
                    throw new InvalidOperationException("Unexpected second line of operators");
                }

                if (spanPart[0] == '*' || spanPart[0] == '+')
                {
                    isOperator = true;
                }
                else if (char.IsDigit(spanPart[0]))
                {
                    isOperator = false;
                }
                else
                {
                    throw new InvalidOperationException("Not an operator or digit");
                }
            }

            if (isOperator == true)
            {
                var op = spanPart[0] == '*' ? Operator.Multiply : Operator.Add;
                operators.Add(op);
            }
            else
            {
                entry.Add(ulong.Parse(span[part]));
            }
        }

        if (entry.Count > 0)
        {
            if (grid.Count > 0 && grid[0].Length != entry.Count)
            {
                throw new InvalidOperationException("Mismatch in grid size");
            }

            grid.Add(entry.ToArray());
        }
    }

    if (operators.Count != grid[0].Length)
    {
        throw new InvalidOperationException("Grid / operator count mismatch");
    }

    return (grid.ToArray(), operators.ToArray());
}


enum Operator { Add, Multiply }