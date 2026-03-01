namespace DS_SEM_1.Generators.Uniform;

public class DiscreteUniform : BaseUniform<int>
{
    public DiscreteUniform(Random seedGenerator, int min, int max) : base(seedGenerator, min, max)
    {
    }

    public override int GenerateNext()
    {
        return _generator.Next(_range) + _min;
    }
}
