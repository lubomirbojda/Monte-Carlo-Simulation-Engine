using DS_SEM_1.Generators.Empiric.Continuous;
using DS_SEM_1.Generators.Empiric.Discrete;
using DS_SEM_1.Generators.Uniform;
using System.Numerics;

namespace DS_SEM_1.MonteCarlo.TransporterMonteCarlo;

public class Map
{
    private DiscreteUniform _redPathGenerator;
    private ContinuousUniform _greenPathGenerator;
    private DiscreteEmpiric _bluePathGenerator;
    private ContinuousEmpiric _blackPathGenerator;

    private ContinuousUniform _kSlowdownGenerator;

    public TimeSpan ElapsedTime { get; private set; }
    private TimeSpan _limitK;

    public TimeSpan StartTime { get; private set; }

    public void ResetElapsedTime() => ElapsedTime = new(0, 0, 0);

    public Map(Random seedGenerator, TimeSpan startTime, TimeSpan kSlowdownTime)
    {
        _redPathGenerator = new(seedGenerator, 55, 75);
        _greenPathGenerator = new(seedGenerator, 50, 80);
        _blackPathGenerator = new(seedGenerator,
        [
            (10, 20, 0.1),
            (20, 32, 0.5),
            (32, 45, 0.2),
            (45, 75, 0.15),
            (75, 85, 0.05)
        ]);

        _bluePathGenerator = new(seedGenerator,
        [
            (15, 29, 0.2),
            (29, 45, 0.4),
            (45, 65, 0.4)
        ]);

        _kSlowdownGenerator = new(seedGenerator, 10, 25);
        StartTime = startTime;
        _limitK = kSlowdownTime - startTime;
    }

    private double CalculateTime<T>(int length, T generatedSpeed, bool leavingFromK) where T : INumber<T>
    {
        double speed = double.CreateChecked(generatedSpeed);
        if (leavingFromK && ElapsedTime > _limitK)
        {
            var slowdown = GenerateSlowdown();
            speed *= slowdown;
        }


        return length / speed;
    }

    private double CalculateTimeAtElapsed<T>(int length, T generatedSpeed, bool leavingFromK, TimeSpan currElapsed) where T : INumber<T>
    {
        double speed = double.CreateChecked(generatedSpeed);
        if (leavingFromK && currElapsed > _limitK)
        {
            var slowdown = GenerateSlowdown();
            speed *= slowdown;
        }


        return length / speed;
    }

    private double GenerateTimeForRedPath(int length, bool leavingFromK) => CalculateTime(length, _redPathGenerator.GenerateNext(), leavingFromK);

    private double GenerateTimeForGreenPath(int length, bool leavingFromK) => CalculateTime(length, _greenPathGenerator.GenerateNext(), leavingFromK);

    private double GenerateTimeForBluePath(int length, bool leavingFromK) => CalculateTime(length, _bluePathGenerator.GenerateNext(), leavingFromK);

    private double GenerateTimeForBlackPath(int length, bool leavingFromK) => CalculateTime(length, _blackPathGenerator.GenerateNext(), leavingFromK);

    private double GenerateSlowdown()
    {
        var slowdown = _kSlowdownGenerator.GenerateNext();
        return (double)(100 - slowdown) / 100.0;
    }

    private double GetBestOption(params double[] options)
    {
        var minTime = options.Min();
        ElapsedTime = ElapsedTime.Add(TimeSpan.FromHours(minTime));
        return minTime;
    }

    public void ZilinaToDivinka()
    {
        var option1 = GenerateTimeForGreenPath(4, false);
        var option2 = GenerateTimeForRedPath(4, false);
        var toK = GenerateTimeForBlackPath(2, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(2, _redPathGenerator.GenerateNext(), true, curEl);
        var option3 = toK + fromK;

        GetBestOption(option1, option2, option3);
    }

    public void ZilinaToStrecno()
    {
        var option1 = GenerateTimeForRedPath(3, false) + GenerateTimeForRedPath(4, false);
        var option2 = GenerateTimeForGreenPath(4, false) + GenerateTimeForBlackPath(3, false);
        //var option3 = GenerateTimeForBlackPath(2, false) + GenerateTimeForBluePath(4, true);

        var toK = GenerateTimeForBlackPath(2, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(4, _bluePathGenerator.GenerateNext(), true, curEl);
        var option3 = toK + fromK;

        GetBestOption(option1, option2, option3);
    }

    public void ZilinaToRajeckeTeplice()
    {
        //var option1 = GenerateTimeForBlackPath(2, false) + GenerateTimeForGreenPath(2, true);

        var toK = GenerateTimeForBlackPath(2, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(2, _greenPathGenerator.GenerateNext(), true, curEl);
        var option1 = toK + fromK;

        GetBestOption(option1);
    }

    public void DivinkaToZilina()
    {
        var option1 = GenerateTimeForGreenPath(4, false);
        var option2 = GenerateTimeForRedPath(4, false);
        //var option3 = GenerateTimeForRedPath(2, false) + GenerateTimeForBlackPath(2, true);

        var toK = GenerateTimeForRedPath(2, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(2, _blackPathGenerator.GenerateNext(), true, curEl);
        var option3 = toK + fromK;

        GetBestOption(option1, option2, option3);
    }

    public void DivinkaToRajeckeTeplice()
    {
        //var option1 = GenerateTimeForRedPath(2, false) + GenerateTimeForGreenPath(2, true);
        var blackPathTime = GenerateTimeForBlackPath(1, false);
        var bluePathTime = GenerateTimeForBluePath(1, false);
        var option2 = blackPathTime + GenerateTimeForRedPath(3, false) + bluePathTime;
        var option3 = blackPathTime + GenerateTimeForBluePath(1, false) + GenerateTimeForRedPath(2, false) + bluePathTime;

        var toK = GenerateTimeForRedPath(2, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(2, _greenPathGenerator.GenerateNext(), true, curEl);
        var option1 = toK + fromK;

        GetBestOption(option1, option2, option3);
    }

    public void DivinkaToStrecno()
    {
        //var option1 = GenerateTimeForRedPath(2, false) + GenerateTimeForBluePath(4, true);

        var toK = GenerateTimeForRedPath(2, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(4, _bluePathGenerator.GenerateNext(), true, curEl);
        var option1 = toK + fromK;

        GetBestOption(option1);
    }

    public void StrecnoToZilina()
    {
        var option1 = GenerateTimeForRedPath(4, false) + GenerateTimeForRedPath(3, false);
        var option2 = GenerateTimeForBlackPath(3, false) + GenerateTimeForGreenPath(4, false);
        //var option3 = GenerateTimeForBluePath(4, false) + GenerateTimeForBlackPath(2, true);

        var toK = GenerateTimeForBluePath(4, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(2, _blackPathGenerator.GenerateNext(), true, curEl);
        var option3 = toK + fromK;

        GetBestOption(option1, option2, option3);
    }

    public void StrecnoToDivinka()
    {
        //var option1 = GenerateTimeForBluePath(4, false) + GenerateTimeForRedPath(2, true);

        var toK = GenerateTimeForBluePath(4, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(2, _redPathGenerator.GenerateNext(), true, curEl);
        var option1 = toK + fromK;

        GetBestOption(option1);
    }

    public void StrecnoToRajeckeTeplice()
    {
        //var option1 = GenerateTimeForBluePath(4, false) + GenerateTimeForGreenPath(2, true);
        var bluePathTime = GenerateTimeForBluePath(8, false);
        var option2 = GenerateTimeForBluePath(5, false) + bluePathTime;
        var option3 = GenerateTimeForBlackPath(5, false) + bluePathTime;

        var toK = GenerateTimeForBluePath(4, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(2, _greenPathGenerator.GenerateNext(), true, curEl);
        var option1 = toK + fromK;

        GetBestOption(option1, option2, option3);
    }

    public void RajeckeTepliceToDivinka()
    {
        //var option1 = GenerateTimeForGreenPath(2, false) + GenerateTimeForRedPath(2, true);
        var blackPathTime = GenerateTimeForBlackPath(1, false);
        var bluePathTime = GenerateTimeForBluePath(1, false);
        var option2 = bluePathTime + GenerateTimeForRedPath(3, false) + blackPathTime;
        var option3 = bluePathTime + GenerateTimeForRedPath(2, false) + GenerateTimeForBluePath(1, false) + blackPathTime;

        var toK = GenerateTimeForGreenPath(2, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(2, _redPathGenerator.GenerateNext(), true, curEl);
        var option1 = toK + fromK;


        GetBestOption(option1, option2, option3);
    }

    public void RajeckeTepliceToStrecno()
    {
        //var option1 = GenerateTimeForGreenPath(2, false) + GenerateTimeForBluePath(4, true);
        var bluePathTime = GenerateTimeForBluePath(8, false);
        var option2 = bluePathTime + GenerateTimeForBluePath(5, false);
        var option3 = bluePathTime + GenerateTimeForBlackPath(5, false);

        var toK = GenerateTimeForGreenPath(2, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(4, _bluePathGenerator.GenerateNext(), true, curEl);
        var option1 = toK + fromK;

        GetBestOption(option1, option2, option3);
    }

    public void RajeckeTepliceToZilina()
    {
        //var option1 = GenerateTimeForGreenPath(2, false) + GenerateTimeForBlackPath(2, true);

        var toK = GenerateTimeForGreenPath(2, false);
        var curEl = ElapsedTime.Add(TimeSpan.FromHours(toK));
        var fromK = CalculateTimeAtElapsed(2, _blackPathGenerator.GenerateNext(), true, curEl);
        var option1 = toK + fromK;

        GetBestOption(option1);
    }
}
