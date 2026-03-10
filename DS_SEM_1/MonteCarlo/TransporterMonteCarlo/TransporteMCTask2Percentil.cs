namespace DS_SEM_1.MonteCarlo.TransporterMonteCarlo;

internal class TransporteMCTask2Percentil : TransporterMC
{
    private int arrivedAtTime = 0;
    private TimeSpan _start;
    private TimeSpan _end = new(7, 35, 0);
    public TransporteMCTask2Percentil(int seed, TimeSpan startTime, TimeSpan kSlowdownTime) : base(seed, startTime, kSlowdownTime)
    {
        _start = startTime;
    }

    protected override void AfterReplication()
    {
        TimeSpan arrivalTime = _start.Add(TimeSpan.FromMinutes(Map.ElapsedTime.TotalMinutes));
        if (arrivalTime <= _end)
            arrivedAtTime++;
    }

    protected override void AfterSimulation()
    {
        double arrivedPercent = (double)arrivedAtTime / _replicationsCount * 100;
        TargetLabelResult.Invoke(new Action(() =>
        {
            TargetLabelResult.Text = $"Percentuálny počet príchodov do 7:35: " + arrivedPercent + "%";
            TargetLabelResult.Refresh();
        }));
    }
}
