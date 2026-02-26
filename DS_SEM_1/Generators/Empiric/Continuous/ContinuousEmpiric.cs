namespace DS_SEM_1.Generators.Empiric.Continuous;

public class ContinuousEmpiric(Random seedGenerator, List<(double min, double max, double probabiltiy)> intervals)
    : BaseEmpiric<double, IntervalContinuous>(seedGenerator, intervals)
{
    protected override IntervalContinuous CreateInterval(double min, double max, double probabiltiy, double upperBound, int seedForGenerator)
        => new(min, max, probabiltiy, upperBound, seedForGenerator);
}
