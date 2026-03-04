using System.Numerics;

namespace DS_SEM_1.Generators.Empiric;

public abstract class BaseInterval<T> where T : INumber<T>
{
    protected T Min { get; }
    protected double Probability { get; }
    public double UpperBound { get; }

    protected readonly Random _generator;
    protected readonly T _generationRange;

    protected BaseInterval(T min, T max, double probability, double upperBound, int seedForGenerator)
    {
        if (min > max)
            throw new Exception("Minimum has to be higher or equal maximnum.");

        Min = min;
        Probability = probability;
        UpperBound = upperBound;

        _generator = new Random(seedForGenerator);
        _generationRange = max - Min;
    }

    public abstract T GenerateNext();
}
