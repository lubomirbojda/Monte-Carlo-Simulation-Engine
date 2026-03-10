using OpenTK.Graphics.ES11;
using ScottPlot.WinForms;

namespace DS_SEM_1.MonteCarlo.TransporterMonteCarlo;

public class TransporterAllRoutesMC
{
    private int _replCount;
    private int _seed;
    private FormsPlot[] _plots;
    private Label[] _labels;
    private bool _isRunning = true;
    private TransporterMC? _task;

    private TimeSpan _startTime;
    private TimeSpan _kSlowdownTime;

    public TransporterAllRoutesMC(int replicationCount, FormsPlot[] plots, Label[] targetLabels,
        TimeSpan startTime, TimeSpan kSlowdownTime, int? customSeed = null)
    {
        _replCount = replicationCount;
        _plots = plots;
        if (customSeed.HasValue)
            _seed = customSeed.Value;
        else
            _seed = new Random().Next();

        _startTime = startTime;
        _kSlowdownTime = kSlowdownTime;
        _labels = targetLabels;
    }

    public void StopSimulation()
    {
        _isRunning = false;
        _task?.StopSimulation();
    }

    public void RunTask1()
    {
        //01------------------------------------------------------------------------------
        RunRoute((transMC) => [
            transMC.Map.ZilinaToDivinka,
            transMC.Map.DivinkaToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToStrecno,
            transMC.Map.StrecnoToZilina
        ],
        _plots[0],
        "Route: ZA -> D -> RT -> S -> ZA",
        _labels[0],
        _startTime,
        _kSlowdownTime);
        //02------------------------------------------------------------------------------
        RunRoute((transMC) => [
            transMC.Map.ZilinaToDivinka,
            transMC.Map.DivinkaToStrecno,
            transMC.Map.StrecnoToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToZilina
        ],
        _plots[1],
        "Route: ZA -> D -> S -> RT -> ZA",
        _labels[1],
        _startTime,
        _kSlowdownTime);
        //03------------------------------------------------------------------------------
        RunRoute((transMC) => [
            transMC.Map.ZilinaToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToDivinka,
            transMC.Map.DivinkaToStrecno,
            transMC.Map.StrecnoToZilina
        ],
        _plots[2],
        "Route: ZA -> RT -> D -> S -> ZA",
        _labels[2],
        _startTime,
        _kSlowdownTime);
        //04------------------------------------------------------------------------------
        RunRoute((transMC) => [
            transMC.Map.ZilinaToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToStrecno,
            transMC.Map.StrecnoToDivinka,
            transMC.Map.DivinkaToZilina
        ],
        _plots[3],
        "Route: ZA -> RT -> S -> D -> ZA",
        _labels[3],
        _startTime,
        _kSlowdownTime);
        //05------------------------------------------------------------------------------
        RunRoute((transMC) => [
            transMC.Map.ZilinaToStrecno,
            transMC.Map.StrecnoToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToDivinka,
            transMC.Map.DivinkaToZilina
        ],
        _plots[4],
        "Route: ZA -> S -> RT -> D -> ZA",
        _labels[4],
        _startTime,
        _kSlowdownTime);
        //06------------------------------------------------------------------------------
        RunRoute((transMC) => [
            transMC.Map.ZilinaToStrecno,
            transMC.Map.StrecnoToDivinka,
            transMC.Map.DivinkaToRajeckeTeplice,
            transMC.Map.RajeckeTepliceToZilina
        ],
        _plots[5],
        "Route: ZA -> S -> D -> RT -> ZA",
        _labels[5],
        _startTime,
        _kSlowdownTime);
    }

    private void RunRoute(Func<TransporterMC, List<Action>> routeCreator, FormsPlot plot, string plotName,
        Label targetLabel, TimeSpan startTime, TimeSpan kSlowdowndTime)
    {
        TransporterMC transMC = new(_seed, startTime, kSlowdowndTime);
        _task = transMC;
        List<Action> route = routeCreator(_task);
        _task.SetNewRoute(route);
        if (!_isRunning)
            _task.StopSimulation();
        plot.Plot.Title(plotName);
        _task.TargetLabelResult = targetLabel;
        _task.TargetPlot = plot;

        _task.RunSimulation(_replCount);
    }

    public void RunTask2(FormsPlot plot, Label targetLabel)
    {
        TransporteMCTask2 transMCT2 = new(_seed, new(7, 0, 0), new(6, 30, 0));
        _task = transMCT2;
        List<Action> route = [
            transMCT2.Map.ZilinaToDivinka,
            transMCT2.Map.DivinkaToRajeckeTeplice,
            transMCT2.Map.RajeckeTepliceToStrecno,
            transMCT2.Map.StrecnoToZilina
            ];
        _task.SetNewRoute(route);
        if (!_isRunning)
            _task.StopSimulation();
        plot.Plot.Title("Route: ZA -> D -> RT -> S -> ZA");
        _task.TargetLabelResult = targetLabel;
        _task.TargetPlot = plot;
        _task.RunSimulation(_replCount);
    }
}
