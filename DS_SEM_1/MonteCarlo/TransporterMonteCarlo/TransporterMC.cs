namespace DS_SEM_1.MonteCarlo.TransporterMonteCarlo;

public class TransporterMC : BaseMonteCarlo
{
    private Map _map;
    private double _totalTime;
    private List<Func<double>> _route;

    public TransporterMC(Map map, List<Func<double>> route) : base()
    {
        _map = map;
        _route = route;
    }

    public TransporterMC(int customSeed, Map map, List<Func<double>> route) : base(customSeed)
    {
        _map = map;
        _route = route;
    }

    protected override void BeforeSimulation()
    {
        _totalTime = 0;
    }

    protected override void BeforeReplication()
    {
        _map.ResetElapsedTime();
    }

    protected override void DoReplication()
    {
        foreach (var move in _route)
        {
            move();
        }
        _totalTime += _map._elapsedTime.TotalMinutes;
    }

    protected override void AfterReplication()
    {
        if (_currentReplication % 1000 == 0)
        {
            double average = _totalTime / _currentReplication;
            //Console.WriteLine("Rep: " + _currentReplication + " ->  avg:  " + average);
        }
    }

    protected override void AfterSimulation()
    {
        Console.WriteLine("Sim done");
        double average = _totalTime / _currentReplication;
        Console.WriteLine("Absolute avg: " + average);
    }
}
