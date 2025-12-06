// var inputFile = "test-input.txt";
var inputFile = "real-input.txt";

await using var fileStream = File.OpenRead(inputFile);
using var fileReader = new StreamReader(fileStream);

ulong count = 0;
var ranges = await ReadRanges(fileReader);
foreach (var range in ranges)
{
    count += (range.Last - range.First) + 1;
}

// await foreach (var id in ReadTestValues(fileReader))
// {
//     var containsId = RangeContainsValue(id, ranges);
//     if (containsId)
//     {
//         count++;
//     }
//     
//     Console.WriteLine($"{id} - {containsId}");
// }

Console.WriteLine(count);

return;

async Task<Range[]> ReadRanges(StreamReader reader)
{
    var ranges = new List<Range>();
    while (true)
    {
        var line = await reader.ReadLineAsync();
        if (line == null)
        {
            throw new InvalidOperationException("Unexpected end of stream");
        }

        if (line == string.Empty)
        {
            break;
        }

        var span = line.AsSpan();
        var dashIndex = span.IndexOf('-');
        if (dashIndex < 0)
        {
            throw new InvalidOperationException("No dash on line");
        }

        var first = ulong.Parse(span[..dashIndex]);
        var second = ulong.Parse(span[(dashIndex + 1)..]);
        if (first > second)
        {
            throw new InvalidOperationException("wrong order");
        }

        InsertRange(new Range(first, second), ranges);
        
        // ranges.Add(new Range(first, second));
    }

    return ranges.ToArray();
}

void InsertRange(Range toInsert, List<Range> ranges)
{
    for (var x = 0; x < ranges.Count; x++)
    {
        var current = ranges[x];

        if (toInsert.First > current.Last)
        {
            continue;
        }

        if (toInsert.Last < current.First)
        {
            // Fully before this range
            ranges.Insert(x, toInsert);
        }
        else if (current.First <= toInsert.First && current.Last >= toInsert.Last)
        {
            // The range to insert fully inside the current range, nothing to do
        }
        else if (toInsert.First < current.First && toInsert.Last >= current.Last)
        {
            // toInsert fully encloses current
            ranges.RemoveAt(x);
            InsertRange(toInsert, ranges);
        }
        else if (toInsert.First < current.First)
        {
            // Does this range need to be extended on the first side?
            ranges.RemoveAt(x);
            InsertRange(current with { First = toInsert.First }, ranges);
        }
        else if (toInsert.Last > current.Last)
        {
            // Does the range need to be extended on the last side?
            ranges.RemoveAt(x);
            InsertRange(current with { Last = toInsert.Last }, ranges);
        }

        return;
    }
    
    // If we got here, we didn't insert it, so append it
    ranges.Add(toInsert);
}

async IAsyncEnumerable<ulong> ReadTestValues(StreamReader reader)
{
    while (true)
    {
        var line = await reader.ReadLineAsync();
        if (line == null || string.IsNullOrWhiteSpace(line))
        {
            break;
        }

        yield return ulong.Parse(line);
    }
}

public record struct Range(ulong First, ulong Last);