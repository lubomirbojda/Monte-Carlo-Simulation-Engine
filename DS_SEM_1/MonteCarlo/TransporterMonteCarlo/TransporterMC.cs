using ScottPlot.WinForms;

namespace DS_SEM_1.MonteCarlo.TransporterMonteCarlo;

public class TransporterMC : BaseMonteCarlo
{
    public Map Map {  get; private set; }
    private double _totalTime;
    private List<Action> _route = [];
    public List<double> _replictaionsResults { get; private set; } = [];

    public FormsPlot TargetPlot { get; set; } = new();
    public Label TargetLabelResult { get; set; } = new();
    private ScottPlot.Plottables.Signal? _signalPlot = null;

    private int _warmUp;

    public TransporterMC(int seed, TimeSpan startTime, TimeSpan kSlowdownTime) : base(seed)
    {
        Map = new(_seedGenerator, startTime, kSlowdownTime);
    }

    public void SetNewRoute(List<Action> route)
    {
        _route = route;
    }

    protected override void BeforeSimulation()
    {
        _replictaionsResults = new(_replicationsCount);
        _totalTime = 0;
        _warmUp = (int)(_replicationsCount * 0.2);
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
        _totalTime += Map.ElapsedTime.TotalMinutes;
    }

    protected override void AfterReplication()
    {
        if (_currentReplication > _warmUp)
        {
            if (_currentReplication % 1000 == 0)
            {
                double average = _totalTime / _currentReplication;
                _replictaionsResults.Add(average);
                UpdateGraph();
            }
        }
    }

    protected override void AfterSimulation()
    {
        if (_currentReplication <= 0)
            return;

        double average = _totalTime / _currentReplication;
        TimeSpan avg = Map.StartTime.Add(TimeSpan.FromMinutes(average));

        TargetLabelResult.Invoke(new Action(() =>
        {
            TargetLabelResult.Text = $"Priemerný čas: " + avg.ToString();
            TargetLabelResult.Refresh();
        }));

        //_replictaionsResults.Sort();
        //var th80 = (int)(_replictaionsResults.Count * 0.8);
        //var result80 = _replictaionsResults[th80];
        //TimeSpan end = new(7, 35, 0);
        //TimeSpan result80Time = end.Subtract(TimeSpan.FromMinutes(result80));
        //Console.WriteLine("Leave time if 80 at 7:35:  " + result80Time);
    }

    private void UpdateGraph()
    {
        if (TargetPlot == null || _replictaionsResults.Count == 0) return;

        TargetPlot.Invoke(new Action(() =>
        {
            double warmUpThreshold = _replicationsCount * 0.2;
            double startX = warmUpThreshold + 1000;

            if (_signalPlot == null)
            {
                _signalPlot = TargetPlot.Plot.Add.Signal(_replictaionsResults.ToArray());
                _signalPlot.Data.XOffset = startX;
                _signalPlot.Data.Period = 1000;
            }
            else
            {
                _signalPlot.Data = new ScottPlot.DataSources.SignalSourceDouble(_replictaionsResults.ToArray(), 1000);
                _signalPlot.Data.XOffset = startX;
            }

            var yAxis = TargetPlot.Plot.Axes.Left;
            if (yAxis.TickGenerator is ScottPlot.TickGenerators.NumericAutomatic gen)
            {
                gen.TargetTickCount = 15;
            }

            yAxis.TickLabelStyle.FontSize = 10;

            double min = _replictaionsResults.Min();
            double max = _replictaionsResults.Max();

            TargetPlot.Plot.Axes.SetLimitsY(min, max);
            TargetPlot.Plot.Axes.AutoScaleX();
            TargetPlot.Refresh();
        }));
    }
}
