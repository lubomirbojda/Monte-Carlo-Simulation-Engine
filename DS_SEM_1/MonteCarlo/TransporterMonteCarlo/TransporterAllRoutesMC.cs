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
        "Trasa: ZA -> D -> RT -> S -> ZA",
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
        "Trasa: ZA -> D -> S -> RT -> ZA",
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
        "Trasa: ZA -> RT -> D -> S -> ZA",
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
        "Trasa: ZA -> RT -> S -> D -> ZA",
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
        "Trasa: ZA -> S -> RT -> D -> ZA",
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
        "Trasa: ZA -> S -> D -> RT -> ZA",
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
            transMCT2.Map.ZilinaToStrecno,
            transMCT2.Map.StrecnoToRajeckeTeplice,
            transMCT2.Map.RajeckeTepliceToDivinka,
            transMCT2.Map.DivinkaToZilina
            ];
        _task.SetNewRoute(route);
        if (!_isRunning)
            _task.StopSimulation();
        plot.Plot.Title("Trasa: ZA -> S -> RT -> D -> ZA");
        _task.TargetLabelResult = targetLabel;
        _task.TargetPlot = plot;
        _task.RunSimulation(_replCount);
    }

    public void RunTask2Percentil(TimeSpan startTime, Label targetLabel)
    {
        TransporteMCTask2Percentil transMCT2Perc = new(_seed, startTime, new(6, 30, 0));
        _task = transMCT2Perc;
        List<Action> route = [
            transMCT2Perc.Map.ZilinaToStrecno,
            transMCT2Perc.Map.StrecnoToRajeckeTeplice,
            transMCT2Perc.Map.RajeckeTepliceToDivinka,
            transMCT2Perc.Map.DivinkaToZilina
        ];
        _task.SetNewRoute(route);
        if (!_isRunning)
            _task.StopSimulation();
        _task.TargetLabelResult = targetLabel;
        _task.RunSimulation(_replCount);
    }
}
