
using Day01;

var current = 50;
var count = 0;
var ignoreNextNegativeRotation = false;
await foreach (var number in TurnReader.GetTurns("input.txt"))
{
    Console.Write($"{current} + {number} = ");
    var innerRotations = 0;
    var fullRotations = Math.Abs(number) / 100;
    var subRotation = Math.Abs(number) % 100;
    if (number < 0)
    {
        subRotation *= -1;
    }

    current += subRotation;
    if (current < 0)
    {
        current += 100;
        if (!ignoreNextNegativeRotation)
        {
            innerRotations++;
        }
    }
    else if (current > 99)
    {
        current -= 100;
        innerRotations++;
    }
    else if (current == 0)
    {
        innerRotations++;
    }

    count += fullRotations + innerRotations;
    Console.WriteLine($"{current} ({innerRotations + fullRotations} rotations) ({count} total)");

    ignoreNextNegativeRotation = current == 0;
}

Console.WriteLine(count);