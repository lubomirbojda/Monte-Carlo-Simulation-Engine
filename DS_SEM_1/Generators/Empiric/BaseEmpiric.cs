using System.Numerics;

namespace DS_SEM_1.Generators.Empiric;

public abstract class BaseEmpiric<T, TInterval> where TInterval : BaseInterval<T> where T : INumber<T>
{
    private Random _seedGenerator;
    private List<TInterval> _allIntervals;
    private Random _probGenerator;

    protected abstract TInterval CreateInterval(T min, T max, double probabiltiy, double upperBound, 
        int seedForGenerator);

    protected BaseEmpiric(Random seedGenerator, List<(T min, T max, double probabiltiy)> intervals)
    {
        _seedGenerator = seedGenerator;
        _probGenerator = new(_seedGenerator.Next());
        _allIntervals = [];
        double upperBound = 0;

        foreach (var (min, max, probability) in intervals)
        {
            upperBound += probability;
            _allIntervals.Add(CreateInterval(min, max, probability, upperBound, _seedGenerator.Next()));
        }

        if (Math.Abs(upperBound - 1.0) > 0.00001)
        {
            throw new Exception("Sum of probabilities from all intervals has to be 1.");
        }
    }

    //metóda vytvorená pomocou AI: Obrázok č. 1
    public T GenerateNext()
    {
        double roll = _probGenerator.NextDouble();

        var selectedInterval = _allIntervals.FirstOrDefault(i => roll < i.UpperBound);

        //in case of double rounding mistake
        if (selectedInterval == null)
        {
            selectedInterval = _allIntervals.Last();
        }

        return selectedInterval.GenerateNext();
    }
}
