namespace DS_SEM_1.MonteCarlo;

public abstract class BaseMonteCarlo
{
    protected Random _seedGenerator;
    private bool _isRunning = true;

    protected BaseMonteCarlo()
    {
        _seedGenerator = new();
    }

    protected BaseMonteCarlo(int customSeed)
    {
        _seedGenerator = new(customSeed);
    }

    public void StopSimulation()
    {
        _isRunning = false;
    }

    protected abstract void DoReplication();
    protected abstract void BeforeSimulation();
    protected abstract void AfterSimulation();
    protected abstract void BeforeReplication();
    protected abstract void AfterReplication();

    public void RunSimulation(int replicationsCount)
    {
        BeforeSimulation();

        int currentReplication = 0;
        while (currentReplication < replicationsCount && _isRunning)
        {
            BeforeReplication();

            DoReplication();

            currentReplication++;
            AfterReplication();
        }

        AfterSimulation();
    }

}
