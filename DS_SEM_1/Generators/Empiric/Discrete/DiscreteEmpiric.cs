namespace DS_SEM_1.Generators.Empiric.Discrete;

public class DiscreteEmpiric(Random seedGenerator, List<(int min, int max, double probabiltiy)> intervals)
    : BaseEmpiric<int, IntervalDiscrete>(seedGenerator, intervals)
{
    protected override IntervalDiscrete CreateInterval(int min, int max, double probabiltiy, double upperBound, int seedForGenerator)
        => new(min, max, probabiltiy, upperBound, seedForGenerator);
}
