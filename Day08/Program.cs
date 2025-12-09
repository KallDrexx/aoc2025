// const string inputFile = "test-input.txt";
const string inputFile = "real-input.txt";

var potentialLinks = new List<Link>();
var points = new List<Point>();
var circuits = new List<Circuit>();
var pointCircuitMap = new Dictionary<Point, Circuit>();

await foreach (var line in File.ReadLinesAsync(inputFile))
{
    var parts = line.Split(',');
    if (parts.Length != 3)
    {
        throw new InvalidOperationException($"Invalid line: '{line}'");
    }

    var x = int.Parse(parts[0]);
    var y = int.Parse(parts[1]);
    var z = int.Parse(parts[2]);
    var point = new Point(x, y, z);
    var circuit = new Circuit([point]);

    points.Add(point);
    circuits.Add(circuit);
    pointCircuitMap.Add(point, circuit);
}

for (var x = 0; x < points.Count - 1; x++)
for (var y = x + 1; y < points.Count; y++)
{
    var first = points[x];
    var second = points[y];
    var distance = Math.Sqrt(
        Math.Pow(second.X - first.X, 2) +
        Math.Pow(second.Y - first.Y, 2) +
        Math.Pow(second.Z - first.Z, 2));

    potentialLinks.Add(new Link(first, second, distance));
}

potentialLinks.Sort(new LinkComparer());
Link? finalLink = null;
while (true)
{
    var link = potentialLinks[0];
    potentialLinks.RemoveAt(0);

    var firstCircuit = pointCircuitMap[link.First];
    var secondCircuit = pointCircuitMap[link.Second];

    if (firstCircuit != secondCircuit)
    {
        foreach (var point in secondCircuit!.Points)
        {
            firstCircuit!.Points.Add(point);
            pointCircuitMap[point] = firstCircuit;
        }

        circuits.Remove(secondCircuit);

        if (circuits.Count == 1)
        {
            finalLink = link;
            break;
        }
    }
}

Console.WriteLine(finalLink.Value.First.X * finalLink.Value.Second.X);


return;

readonly record struct Point(int X, int Y, int Z);
readonly record struct Link(Point First, Point Second, double Distance);

record Circuit(HashSet<Point> Points);

class LinkComparer : IComparer<Link>
{
    public int Compare(Link x, Link y)
    {
        return x.Distance.CompareTo(y.Distance);
    }
}

class CircuitComparer : IComparer<Circuit>
{
    public int Compare(Circuit? x, Circuit? y)
    {
        return x!.Points.Count.CompareTo(y!.Points.Count);
    }
}