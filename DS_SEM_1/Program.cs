using DS_SEM_1.Generators;
using DS_SEM_1.MonteCarlo.TransporterMonteCarlo;

namespace DS_SEM_1;

public class Program
{
    static void Main(string[] args)
    {
        //GeneratorsTester.DiscreteUniformTest();
        //GeneratorsTester.ContinuousUniformTest();
        //GeneratorsTester.DiscreteEmpiricTest();
        //GeneratorsTester.ContinuousEmpiricTest();
        //return;

        int replCount = 10000000;
        Random seedGen = new();
        var seed = seedGen.Next();
        Map map = new(new Random(seed));

        //Zilina -> Divinka -> Rajecke Teplice -> Strecno -> Zilina
        List<Func<double>> route1 = [
            map.ZilinaToDivinka,
            map.DivinkaToRajeckeTeplice,
            map.RajeckeTepliceToStrecno,
            map.StrecnoToZilina
        ];

        Console.WriteLine("Route: Zilina -> Divinka -> Rajecke Teplice -> Strecno -> Zilina");
        TransporterMC trMC = new(map, route1);
        trMC.RunSimulation(replCount);


        //--------------------------------------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Divinka -> Strecno -> Rajecke Teplice -> Zilina");
        map = new(new Random(seed));

        //Zilina -> Divinka -> Strecno -> Rajecke Teplice -> Zilina
        List<Func<double>> route2 = [
            map.ZilinaToDivinka,
            map.DivinkaToStrecno,
            map.StrecnoToRajeckeTeplice,
            map.RajeckeTepliceToZilina
        ];

        trMC = new(map, route2);
        trMC.RunSimulation(replCount);


        //--------------------------------------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Rajecke Teplice -> Divinka -> Strecno -> Zilina");
        map = new(new Random(seed));

        //Zilina -> Rajecke Teplice -> Divinka -> Strecno -> Zilina
        List<Func<double>> route3 = [
            map.ZilinaToRajeckeTeplice,
            map.RajeckeTepliceToDivinka,
            map.DivinkaToStrecno,
            map.StrecnoToZilina
        ];

        trMC = new(map, route3);
        trMC.RunSimulation(replCount);


        //--------------------------------------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Rajecke Teplice -> Strecno -> Divinka -> Zilina");
        map = new(new Random(seed));

        //Zilina -> Rajecke Teplice -> Strecno -> Divinka -> Zilina
        List<Func<double>> route4 = [
            map.ZilinaToRajeckeTeplice,
            map.RajeckeTepliceToStrecno,
            map.StrecnoToDivinka,
            map.DivinkaToZilina
        ];

        trMC = new(map, route4);
        trMC.RunSimulation(replCount);


        //--------------------------------------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Strecno -> Rajecke Teplice -> Divinka -> Zilina");
        map = new(new Random(seed));

        //Zilina -> Strecno -> Rajecke Teplice -> Divinka -> Zilina
        List<Func<double>> route5 = [
            map.ZilinaToStrecno,
            map.StrecnoToRajeckeTeplice,
            map.RajeckeTepliceToDivinka,
            map.DivinkaToZilina
        ];

        trMC = new(map, route5);
        trMC.RunSimulation(replCount);


        //--------------------------------------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Strecno -> Divinka -> Rajecke Teplice -> Zilina");
        map = new(new Random(seed));

        //Zilina -> Strecno -> Divinka -> Rajecke Teplice -> Zilina
        List<Func<double>> route6 = [
            map.ZilinaToStrecno,
            map.StrecnoToDivinka,
            map.DivinkaToRajeckeTeplice,
            map.RajeckeTepliceToZilina
        ];

        trMC = new(map, route6);
        trMC.RunSimulation(replCount);
    }
}
