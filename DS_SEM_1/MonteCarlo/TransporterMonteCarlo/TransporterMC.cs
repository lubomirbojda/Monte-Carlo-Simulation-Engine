namespace DS_SEM_1.MonteCarlo.TransporterMonteCarlo;

public class TransporterMC : BaseMonteCarlo
{
    public Map Map {  get; private set; }
    private double _totalTime;
    private List<Func<double>> _route = [];

    public TransporterMC(int seed) : base(seed)
    {
        Map = new(_seedGenerator);
    }

    public void SetNewRoute(List<Func<double>> route)
    {
        _route = route;
    }

    protected override void BeforeSimulation()
    {
        _totalTime = 0;
    }

    protected override void BeforeReplication()
    {
        Map.ResetElapsedTime();
    }

    protected override void DoReplication()
    {
        foreach (var move in _route)
        {
            move();
        }
        _totalTime += Map._elapsedTime.TotalMinutes;
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
