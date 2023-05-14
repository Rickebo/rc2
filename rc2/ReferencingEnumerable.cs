using System.Collections;

namespace rc2;

public class ReferencingEnumerable<T> : IEnumerable<T>
{
    private readonly Func<IEnumerable<T>> _reader;
    
    public ReferencingEnumerable(Func<IEnumerable<T>> reader)
    {
        _reader = reader;
    }

    public IEnumerator<T> GetEnumerator() => _reader().GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}