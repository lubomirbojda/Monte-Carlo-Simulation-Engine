using System.Numerics;

namespace DS_SEM_1.Generators.Uniform;

public abstract class BaseUniform<T> where T : INumber<T>
{
    protected Random _generator;
    protected T _range;
    protected T _min;

    protected BaseUniform(Random seedGenerator, T min, T max)
    {
        if (min > max)
            throw new Exception("Minimum has to be higher or equal maximnum.");

        _generator = new(seedGenerator.Next());
        _range = max - min;
        _min = min;
    }

    public abstract T GenerateNext();
}
