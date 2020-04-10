namespace com.armatur.common.flags
{
    public class PositiveCounterWithMax : CounterWithMax
    {
        public PositiveCounterWithMax(string name) : base(name)
        {
            RealCounter.SetMinimal(0);
            MaxCounter.SetMinimal(0);
        }
    }
}