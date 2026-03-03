namespace DS_SEM_1.MonteCarlo;

public abstract class BaseMonteCarlo
{
    public Random _seedGenerator {  get; private set; }
    private bool _isRunning = true;
    protected int _currentReplication;

    protected BaseMonteCarlo(int seed)
    {
        _seedGenerator = new(seed);
    }

    public void StopSimulation()
    {
        _isRunning = false;
    }

    protected abstract void BeforeSimulation();
    protected abstract void BeforeReplication();
    protected abstract void DoReplication();
    protected abstract void AfterReplication();
    protected abstract void AfterSimulation();

    public void RunSimulation(int replicationsCount)
    {
        BeforeSimulation();

        _currentReplication = 0;
        while (_currentReplication < replicationsCount && _isRunning)
        {
            BeforeReplication();

            DoReplication();

            _currentReplication++;
            AfterReplication();
        }

        AfterSimulation();
    }

}
