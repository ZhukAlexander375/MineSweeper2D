
[System.Serializable]
public class TimerManagerData 
{
    public long TimerStartTime;    
    public bool IsTimerRunning;
    public bool IsTimerOver = true;

    public TimerManagerData(TimeModeTimerManager timeManager = null)
    {
        if (timeManager != null)
        {
            TimerStartTime = timeManager.TimerStartTime;
            IsTimerRunning = timeManager.IsTimerRunning;
            IsTimerOver= timeManager.IsTimerOver;
        }        
    }
}
