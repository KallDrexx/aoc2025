const string inputFile = "test-input.txt";
// const string inputFile = "real-input.txt";

var points = new List<Point>();
await foreach (var line in File.ReadLinesAsync(inputFile))
{
    var parts = line.Split(',');
    if (parts.Length != 2)
    {
        throw new InvalidOperationException("Unexpected line");
    }

    var x = int.Parse(parts[0]);
    var y = int.Parse(parts[1]);

    var newPoint = new Point(x, y);
    points.Add(newPoint);
}

var lines = new List<Line>();
for (var x = 0; x < points.Count; x++)
{
    var first = points[x];
    var second = x < points.Count - 1 ? points[x + 1] : points[0];
    var line = new Line(first, second);
    lines.Add(line);
}

ulong maxArea = 0;
for (var x = 0; x < points.Count - 1; x++)
{
    var point1 = points[x];
    for (var y = x + 1; y < points.Count; y++)
    {
        var point2 = points[y];

        var point3 = new Point(point1.X, point2.Y);
        var point4 = new Point(point2.X, point1.Y);
        var line1 = new Line(point1, point3);
        var line2 = new Line(point3, point2);
        var line3 = new Line(point2, point4);
        var line4 = new Line(point4, point1);

        var lineIntersectionFound = false;
        foreach (var checkLine in lines)
        {
            if (Intersects(line1, checkLine) ||
                Intersects(line2, checkLine) ||
                Intersects(line3, checkLine) ||
                Intersects(line4, checkLine))
            {
                lineIntersectionFound = true;
                break;
            }
        }

        if (lineIntersectionFound)
        {
            continue;
        }

        ulong xDiff = (ulong)Math.Abs(point1.X - point2.X) + 1;
        ulong yDiff = (ulong)Math.Abs(point1.Y - point2.Y) + 1;
        ulong area = xDiff * yDiff;

        if (area > maxArea)
        {
            maxArea = area;
        }
    }
}

Console.WriteLine(maxArea);

return;

bool Intersects(Line line1, Line line2)
{
    var line1IsHorizontal = line1.Point1.X == line1.Point2.X;
    var line2IsHorizontal = line2.Point1.X == line2.Point2.X;

    if (line1IsHorizontal == line2IsHorizontal)
    {
        // Parallel and don't intersect
        return false;
    }

    if (line1IsHorizontal)
    {
        var l1MinX = Math.Min(line1.Point1.X, line1.Point2.X);
        var l1MaxX = Math.Max(line1.Point1.X, line1.Point2.X);
        var l2MinY = Math.Min(line2.Point1.Y, line2.Point2.Y);
        var l2MaxY = Math.Max(line2.Point1.Y, line2.Point2.Y);

        return line1.Point1.Y >= l2MinY &&
               line1.Point1.Y <= l2MaxY &&
               l1MinX < line2.Point1.X &&
               l1MaxX > line2.Point1.X;
    }
    else
    {
        var l2MinX = Math.Min(line2.Point1.X, line2.Point2.X);
        var l2MaxX = Math.Max(line2.Point1.X, line2.Point2.X);
        var l1MinY = Math.Min(line1.Point1.Y, line1.Point2.Y);
        var l1MaxY = Math.Max(line1.Point1.Y, line1.Point2.Y);

        return line2.Point1.Y >= l1MinY &&
               line2.Point1.Y <= l1MaxY &&
               l2MinX < line1.Point1.X &&
               l2MaxX > line1.Point1.X;
    }
}

readonly record struct Point(int X, int Y);

readonly record struct Line(Point Point1, Point Point2);