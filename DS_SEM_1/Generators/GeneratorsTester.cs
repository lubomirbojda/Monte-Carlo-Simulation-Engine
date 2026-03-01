using DS_SEM_1.Generators.Empiric.Continuous;
using DS_SEM_1.Generators.Empiric.Discrete;
using DS_SEM_1.Generators.Uniform;
using System.Numerics;

namespace DS_SEM_1.Generators;

public class GeneratorsTester
{
    private const int GENS_COUNT = 10000;
    public static void DiscreteEmpiricTest()
    {
        Random rand = new();
        var generator = new DiscreteEmpiric(rand, [
            (10, 20, 0.1),
            (20, 32, 0.5),
            (32, 45, 0.2),
            (45, 75, 0.15),
            (75, 85, 0.05)
        ]);

        TestGenerator("DiscreteEmpiricTest", generator.GenerateNext);
    }

    public static void ContinuousEmpiricTest()
    {
        Random rand = new();
        var generator = new ContinuousEmpiric(rand, [
            (15, 29, 0.2),
            (29, 45, 0.4),
            (45, 65, 0.4)
        ]);
        TestGenerator("ContinuousEmpiricTest", generator.GenerateNext);
    }

    public static void DiscreteUniformTest()
    {
        Random rand = new();
        var generator = new DiscreteUniform(rand, 55, 75);
        TestGenerator("DiscreteUniformTest", generator.GenerateNext);
    }

    public static void ContinuousUniformTest()
    {
        Random rand = new();
        var generator = new ContinuousUniform(rand, 50, 80);
        TestGenerator("ContinuousUniformTest", generator.GenerateNext);
    }

    private static void TestGenerator<T> (string fileName, Func<T> generate) where T : INumber<T>
    {
        string currentDir = Environment.CurrentDirectory;
        string fullPath = Path.Combine(currentDir, fileName);

        StreamWriter writer = new(fullPath);
        for (int i = 0; i < GENS_COUNT; i++)
        {
            writer.WriteLine(generate());
        }
    }
}
