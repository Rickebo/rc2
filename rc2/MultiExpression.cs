namespace rc2;

public class MultiExpression
{
    private readonly IEnumerable<Expression> _expressions;

    public static MultiExpression Zero { get; } = Parse("0"); 
    
    public MultiExpression(IEnumerable<Expression> expressions)
    {
        _expressions = expressions;
    }

    public static MultiExpression Parse(string text) => 
        new(text
            .Split(new[] { ';', '&' }, StringSplitOptions.TrimEntries)
            .Select(Expression.Parse)
            .ToArray()
        );

    public double Process(double value, double defaultValue = 0)
    {
        foreach (var expression in _expressions)
        {
            var result = expression.Process(value);

            if (result != null)
                return result.Value;
        }

        return defaultValue;
    }
}