using System.Runtime.Intrinsics.X86;

namespace DS_SEM_1.MonteCarlo.TransporterMonteCarlo;

public class TransporteMCTask2 : TransporterMC
{
    private List<double> _allResults = [];
    public TransporteMCTask2(int seed, TimeSpan startTime, TimeSpan kSlowdownTime) : 
        base(seed, startTime, kSlowdownTime)
    {
    }

    protected override void BeforeSimulation()
    {
        base.BeforeSimulation();
        _allResults = new(_replicationsCount);
    }

    protected override void AfterReplication()
    {
        base.AfterReplication();
        double average = _totalTime / _currentReplication;
        _allResults.Add(average);
    }

    protected override void AfterSimulation()
    {
        base.AfterSimulation();
        _allResults.Sort();
        var th80 = (int)(_allResults.Count * 0.8);
        var result80 = _replictaionsResults[th80];
        TimeSpan end = new(7, 35, 0);
        TimeSpan result80Time = end.Subtract(TimeSpan.FromMinutes(result80));
        TargetLabelResult.Invoke(new Action(() =>
        {
            TargetLabelResult.Text = $"Výsledok: Kuriér musí vyraziť zo ZA najneskôr v čase: " + result80Time.ToString();
            TargetLabelResult.Refresh();
        }));
    }
}
