namespace DS_SEM_1.Generators.Uniform;

public class ContinuousUniform : BaseUniform<double>
{
    public ContinuousUniform(Random seedGenerator, double min, double max) : base(seedGenerator, min, max)
    {
    }

    public override double GenerateNext()
    {
        return (_generator.NextDouble() * _range) + _min;
    }
}
