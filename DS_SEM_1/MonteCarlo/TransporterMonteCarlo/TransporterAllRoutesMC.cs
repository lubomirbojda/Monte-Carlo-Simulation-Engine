namespace DS_SEM_1.MonteCarlo.TransporterMonteCarlo;

public class TransporterAllRoutesMC
{
    private int _replCount;
    private int _seed;

    public TransporterAllRoutesMC(int replicationCount, int? customSeed = null)
    {
        _replCount = replicationCount;
        if (customSeed.HasValue)
            _seed = customSeed.Value;
        else
            _seed = new Random().Next();
    }

    public void RunAllRoutes()
    {
        //01------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Divinka -> Rajecke Teplice -> Strecno -> Zilina");
        RunRoute((transMC) => [
            transMC.Map.ZilinaToDivinka,
            transMC.Map.DivinkaToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToStrecno,
            transMC.Map.StrecnoToZilina
        ]);
        //02------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Divinka -> Strecno -> Rajecke Teplice -> Zilina");
        RunRoute((transMC) => [
            transMC.Map.ZilinaToDivinka,
            transMC.Map.DivinkaToStrecno,
            transMC.Map.StrecnoToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToZilina
        ]);
        //03------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Rajecke Teplice -> Divinka -> Strecno -> Zilina");
        RunRoute((transMC) => [
            transMC.Map.ZilinaToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToDivinka,
            transMC.Map.DivinkaToStrecno,
            transMC.Map.StrecnoToZilina
        ]);
        //04------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Rajecke Teplice -> Strecno -> Divinka -> Zilina");
        RunRoute((transMC) => [
            transMC.Map.ZilinaToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToStrecno,
            transMC.Map.StrecnoToDivinka,
            transMC.Map.DivinkaToZilina
        ]);
        //05------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Strecno -> Rajecke Teplice -> Divinka -> Zilina");
        RunRoute((transMC) => [
            transMC.Map.ZilinaToStrecno,
            transMC.Map.StrecnoToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToDivinka,
            transMC.Map.DivinkaToZilina
        ]);
        //06------------------------------------------------------------------------------
        Console.WriteLine("Route: Zilina -> Strecno -> Divinka -> Rajecke Teplice -> Zilina");
        RunRoute((transMC) => [
            transMC.Map.ZilinaToStrecno,
            transMC.Map.StrecnoToDivinka,
            transMC.Map.DivinkaToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToZilina
        ]);
    }

    private void RunRoute(Func<TransporterMC, List<Func<double>>> routeCreator)
    {
        TransporterMC transMC = new(_seed);
        List<Func<double>> route = routeCreator(transMC);
        transMC.SetNewRoute(route);
        transMC.RunSimulation(_replCount);
    }
}
