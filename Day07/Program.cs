// const string inputFile = "test-input.txt";
const string inputFile = "real-input.txt";

var board = await GetBoard(inputFile);
var rootSplitter = NextSplitter(board.Start, board, []);
// var splitCount = SplitterCount(rootSplitter!);
var finalCount = PathCount(rootSplitter, []);

Console.WriteLine(finalCount);

return;

async Task<Board> GetBoard(string filename)
{
    await using var stream = File.OpenRead(filename);
    using var reader = new StreamReader(stream);

    var row = 0;
    Point? start = null;
    HashSet<Point> splitters = [];

    while (true)
    {
        var line = await reader.ReadLineAsync();
        if (string.IsNullOrWhiteSpace(line))
        {
            break;
        }

        for (var col = 0; col < line.Length; col++)
        {
            switch (line[col])
            {
                case 'S':
                    if (start != null)
                    {
                        throw new InvalidOperationException("Multiple start points");
                    }

                    start = new Point(col, row);
                    break;

                case '.':
                    break;

                case '^':
                    splitters.Add(new Point(col, row));
                    break;

                default:
                    throw new NotSupportedException(line[col].ToString());
            }
        }

        row++;
    }

    if (start == null)
    {
        throw new InvalidOperationException("No start point found");
    }

    return new Board(start.Value, splitters, row);
}

Splitter? NextSplitter(Point laser, Board board, Dictionary<Point, Splitter?> knownSplitters)
{
    while (true)
    {
        laser = laser with { Y = laser.Y + 1 };
        if (laser.Y > board.RowCount)
        {
            return null;
        }

        if (board.Splitters.Contains(laser))
        {
            var left = laser with { X = laser.X - 1 };
            var right = laser with { X = laser.X + 1 };

            Splitter? leftSplitter = null;
            Splitter? rightSplitter = null;

            if (knownSplitters.TryGetValue(left, out var splitter))
            {
                leftSplitter = splitter;
            }
            else
            {
                leftSplitter = NextSplitter(left, board, knownSplitters);
                knownSplitters.Add(left, leftSplitter);
            }

            if (knownSplitters.TryGetValue(right, out splitter))
            {
                rightSplitter = splitter;
            }
            else
            {
                rightSplitter = NextSplitter(right, board, knownSplitters);
                knownSplitters.Add(right, rightSplitter);
            }

            return new Splitter(laser, leftSplitter, rightSplitter);
        }
    }
}

ulong PathCount(Splitter splitter, Dictionary<Point, ulong> counts)
{
    if (counts.TryGetValue(splitter.Point, out var count))
    {
        return count;
    }

    ulong leftCount = 1;
    ulong rightCount = 1;

    if (splitter.Left != null)
    {
        leftCount = counts.TryGetValue(splitter.Left.Point, out count)
            ? count
            : PathCount(splitter.Left, counts);
    }

    if (splitter.Right != null)
    {
        rightCount = counts.TryGetValue(splitter.Right.Point, out count)
            ? count
            : PathCount(splitter.Right, counts);
    }

    var total = leftCount + rightCount;
    counts.Add(splitter.Point, total);

    return total;
}

int SplitterCount(Splitter root)
{
    var splitters = new Queue<Splitter>([root]);
    var found = new HashSet<Point>();
    while (splitters.Count > 0)
    {
        var splitter = splitters.Dequeue();
        if (!found.Add(splitter.Point))
        {
            continue;
        }

        if (splitter.Left != null)
        {
            splitters.Enqueue(splitter.Left);
        }

        if (splitter.Right != null)
        {
            splitters.Enqueue(splitter.Right);
        }
    }

    return found.Count;
}



readonly record struct Point(int X, int Y);

record Board(Point Start, IReadOnlySet<Point> Splitters, int RowCount);

record Splitter(Point Point, Splitter? Left, Splitter? Right);
