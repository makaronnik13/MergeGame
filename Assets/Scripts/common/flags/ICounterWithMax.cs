namespace com.armatur.common.flags
{
    public interface ICounterWithMax
    {
        Counter Current { get; }
        Counter Max { get; }
        Counter Missed { get; }

        long CurrentValue  { get; }
        long MaxValue  { get; }
        long MissedValue  { get; }
    }
}