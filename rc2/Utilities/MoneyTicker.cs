namespace rc2.Utilities
{
    public class MoneyTicker : Ticker<double>
    {
        public double Interval { get; }

        public MoneyTicker(double interval = 1000000 / 4d)
        {
            Interval = interval;
        }

        public override string ToString(double money) => 
                money > 1000000
                        ? $"{money / 1000000:0.00}M"
                        : money > 1000
                                ? $"{money / 1000:0.00}K"
                                : $"{money:0.00}";

        public override bool IsShown(double current, double previous, bool isFirst)
        {
            return Math.Abs(
                    Math.Floor(current / Interval) -
                    Math.Floor(previous / Interval)
            ) > 0.001;
        }
    }
}
