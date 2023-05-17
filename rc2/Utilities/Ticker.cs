namespace rc2.Utilities
{
    public abstract class Ticker<T>
    {
        public IEnumerable<string> Tick(IEnumerable<T> input)
        {
            var last = default(T);
            var isFirst = true;
            foreach (var tick in input)
            {
                if (IsShown(tick, last, isFirst))
                    yield return ToString(tick);
                else
                    yield return "";

                last = tick;
                isFirst = false;
            }
        }

        public abstract string ToString(T tick);

        public abstract bool IsShown(T current, T? previous, bool isFirst);
    }
}
