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

    private TimeSpan elapsedTime = new(0, 0, 0);
    private TimeSpan limitK = new(0, 25, 0);

    public Map(Random seedGenerator)
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
    }

    private double CalculateTime<T>(int length, T generatedSpeed, bool leavingFromK) where T : INumber<T>
    {
        double speed = double.CreateChecked(generatedSpeed);
        if (leavingFromK && elapsedTime > limitK)
            speed *= GenerateSlowdown();

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

    public double ZilinaToDivinka()
    {
        var blackPathTime = GenerateTimeForBlackPath(2, false);
        var greenPathTime = GenerateTimeForGreenPath(4, false);
        var redPathTime = GenerateTimeForRedPath(4, false);

        var time = Math.Min(blackPathTime, Math.Min(redPathTime, greenPathTime));
        elapsedTime.Add(TimeSpan.FromHours(time));

        if (time == blackPathTime)
        {
            var nextRedTime = GenerateTimeForRedPath(2, true);
            time += nextRedTime;
            elapsedTime.Add(TimeSpan.FromHours(nextRedTime));
        }

        return time;
    }

    public double ZilinaToStrecno()
    {
        var blackPathTime = GenerateTimeForBlackPath(2, false);
        var greenPathTime = GenerateTimeForGreenPath(4, false);
        var redPathTime = GenerateTimeForRedPath(3, false);

        var time = Math.Min(blackPathTime, Math.Min(redPathTime, greenPathTime));
        elapsedTime.Add(TimeSpan.FromHours(time));

        if (time == blackPathTime)
        {
            var bluePathTime = GenerateTimeForBluePath(4, true);
            time += bluePathTime;
            elapsedTime.Add(TimeSpan.FromHours(bluePathTime));
        }
        else if (time == redPathTime)
        {
            var nextRedTime = GenerateTimeForRedPath(4, false);
            time += nextRedTime;
            elapsedTime.Add(TimeSpan.FromHours(nextRedTime));
        }
        else
        {
            var nextBlackTime = GenerateTimeForBlackPath(3, false);
            time += nextBlackTime;
            elapsedTime.Add(TimeSpan.FromHours(nextBlackTime));
        }

        return time;
    }

    public double ZilinaToRajeckeTeplice()
    {
        var blackPathTime = GenerateTimeForBlackPath(2, false);
        var time = blackPathTime;
        elapsedTime.Add(TimeSpan.FromHours(time));
        var greenPathTime = GenerateTimeForGreenPath(2, true);
        time += greenPathTime;
        elapsedTime.Add(TimeSpan.FromHours(greenPathTime));

        return time;
    }

    public double DivinkaToZilina()
    {
        var redPathTime1 = GenerateTimeForRedPath(2, false);
        var greenPathTime = GenerateTimeForGreenPath(4, false);
        var redPathTime2 = GenerateTimeForRedPath(4, false);

        var time = Math.Min(redPathTime1, Math.Min(redPathTime2, greenPathTime));
        elapsedTime.Add(TimeSpan.FromHours(time));

        if (time == redPathTime2)
        {
            var nextBlackTime = GenerateTimeForBlackPath(2, true);
            time += nextBlackTime;
            elapsedTime.Add(TimeSpan.FromHours(nextBlackTime));
        }

        return time;
    }

    public double DivinkaToRajeckeTeplice()
    {
        var blackPathTime = GenerateTimeForBlackPath(1, false);
        var redPathTime1 = GenerateTimeForRedPath(2, false);

        var time = Math.Min(blackPathTime, redPathTime1);
        elapsedTime.Add(TimeSpan.FromHours(time));
        if (time == redPathTime1)
        {
            var greenPathTime = GenerateTimeForGreenPath(2, true);
            time += greenPathTime;
            elapsedTime.Add(TimeSpan.FromHours(greenPathTime));
            return time;
        }

        var redPathTime2 = GenerateTimeForRedPath(3, false);
        var bluePathTime = GenerateTimeForBluePath(1, false);

        var minTime = Math.Min(redPathTime2, bluePathTime);
        time += minTime;
        elapsedTime.Add(TimeSpan.FromHours(minTime));
        if (minTime == bluePathTime)
        {
            var redPathTime3 = GenerateTimeForRedPath(2, false);
            time += redPathTime3;
            elapsedTime.Add(TimeSpan.FromHours(redPathTime3));
        }

        var bluePathTime2 = GenerateTimeForBluePath(1, false);
        time += bluePathTime2;
        elapsedTime.Add(TimeSpan.FromHours(bluePathTime2));

        return time;
    }

    public double DivinkaToStrecno()
    {
        var redPathTime = GenerateTimeForRedPath(2, false);
        var time = redPathTime;
        elapsedTime.Add(TimeSpan.FromHours(time));
        var bluePathTime = GenerateTimeForBluePath(4, true);
        time += bluePathTime;
        elapsedTime.Add(TimeSpan.FromHours(bluePathTime));

        return time;
    }

    public double StrecnoToZilina()
    {
        var blackPathTime = GenerateTimeForBlackPath(3, false);
        var bluePathTime = GenerateTimeForGreenPath(4, false);
        var redPathTime = GenerateTimeForRedPath(4, false);

        var time = Math.Min(blackPathTime, Math.Min(redPathTime, bluePathTime));
        elapsedTime.Add(TimeSpan.FromHours(time));

        if (time == blackPathTime)
        {
            var nextGreenTime = GenerateTimeForGreenPath(4, false);
            time += nextGreenTime;
            elapsedTime.Add(TimeSpan.FromHours(nextGreenTime));
        }
        else if (time == redPathTime)
        {
            var nextRedTime = GenerateTimeForRedPath(3, false);
            time += nextRedTime;
            elapsedTime.Add(TimeSpan.FromHours(nextRedTime));
        }
        else
        {
            var nextBlackTime = GenerateTimeForBlackPath(2, true);
            time += nextBlackTime;
            elapsedTime.Add(TimeSpan.FromHours(nextBlackTime));
        }

        return time;
    }

    public double StrecnoToDivinka()
    {
        var bluePathTime = GenerateTimeForBluePath(4, false);
        var time = bluePathTime;
        elapsedTime.Add(TimeSpan.FromHours(time));
        var redPathTime = GenerateTimeForRedPath(2, true);
        time += redPathTime;
        elapsedTime.Add(TimeSpan.FromHours(redPathTime));

        return time;
    }

    public double StrecnoToRajeckeTeplice()
    {
        var bluePathTime1 = GenerateTimeForBluePath(4, false);
        var bluePathTime2 = GenerateTimeForBluePath(5, false);
        var blackPathTime = GenerateTimeForBlackPath(4, false);

        var time = Math.Min(bluePathTime1, Math.Min(bluePathTime2, blackPathTime));
        elapsedTime.Add(TimeSpan.FromHours(time));

        if (time == bluePathTime1)
        {
            var nextGreenTime = GenerateTimeForGreenPath(2, true);
            time += nextGreenTime;
            elapsedTime.Add(TimeSpan.FromHours(nextGreenTime));
        }
        else
        { 
            var nextBlueTime = GenerateTimeForBluePath(8, true);
            time += nextBlueTime;
            elapsedTime.Add(TimeSpan.FromHours(nextBlueTime));
        }

        return time;
    }

    public double RajeckeTepliceToDivinka()
    {
        var greenPathTime = GenerateTimeForGreenPath(2, false);
        var bluePathTime = GenerateTimeForBluePath(1, false);
        var time = Math.Min(greenPathTime, bluePathTime);
        elapsedTime.Add(TimeSpan.FromHours(time));

        if (time == greenPathTime)
        {
            var nextRedTime = GenerateTimeForRedPath(2, true);
            time += nextRedTime;
            elapsedTime.Add(TimeSpan.FromHours(nextRedTime));
            return time;
        }

        var redPathTime1 = GenerateTimeForRedPath(2, false);
        var redPathTime2 = GenerateTimeForRedPath(3, false);
        var minTime = Math.Min(redPathTime1, redPathTime2);
        time += minTime;
        elapsedTime.Add(TimeSpan.FromHours(minTime));
        if (minTime == redPathTime1)
        {
            var nextBlueTime = GenerateTimeForBluePath(1, false);
            time += nextBlueTime;
            elapsedTime.Add(TimeSpan.FromHours(nextBlueTime));
        }

        var nextBlackTime = GenerateTimeForBlackPath(1, false);
        time += nextBlackTime;
        elapsedTime.Add(TimeSpan.FromHours(nextBlackTime));

        return time;
    }

    public double RajeckeTepliceToStrecno()
    {
        var greenPathTime = GenerateTimeForGreenPath(2, false);
        var bluePathTime = GenerateTimeForBluePath(8, false);

        var time = Math.Min(greenPathTime, bluePathTime);
        elapsedTime.Add(TimeSpan.FromHours(time));

        if (time == greenPathTime)
        {
            var nextBlueTime = GenerateTimeForBluePath(4, true);
            time += nextBlueTime;
            elapsedTime.Add(TimeSpan.FromHours(nextBlueTime));
            return time;
        }

        var nextBlueTime1 = GenerateTimeForBluePath(5, true);
        var nextBlackTime = GenerateTimeForBlackPath(5, true);
        var minTime = Math.Min(nextBlueTime1, nextBlackTime);
        time += minTime;
        elapsedTime.Add(TimeSpan.FromHours(minTime));

        return time;
    }

    public double RajeckeTepliceToZilina()
    {
        var greenPathTime = GenerateTimeForGreenPath(2, false);
        var time = greenPathTime;
        elapsedTime.Add(TimeSpan.FromHours(time));
        var blackPathTime = GenerateTimeForBlackPath(2, true);
        time += blackPathTime;
        elapsedTime.Add(TimeSpan.FromHours(blackPathTime));

        return time;
    }
}
