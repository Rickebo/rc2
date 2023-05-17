namespace rc2.Utilities
{
    public class YearTicker : Ticker<double>
    {
        public double Interval { get; } = 1;

        public YearTicker(double interval = 1)
        {
            Interval = interval;
        }

        public override string ToString(double time) => $"{time:0.00}";

        public override bool IsShown(double current, double previous, bool isFirst)
        {
            if (isFirst)
                return true;

            return Math.Abs(
                    Math.Floor(current / Interval) -
                    Math.Floor(previous / Interval)
            ) > 0.0001;
        }
    }
}