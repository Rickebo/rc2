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

    public double Process(double value, double defaultValue = 0, ProcessMode mode = ProcessMode.First)
    {
        if (mode == ProcessMode.Aggregate)
            return ProcessAggregating(value, defaultValue);
        
        var total = defaultValue;

        foreach (var expression in _expressions)
        {
            var result = expression.Process(value);

            if (result != null)
                return result.Value;
        }

        return total;
    }

    private double ProcessAggregating(double value, double defaultValue = 0)
    {
        var processed = 0d;
        var steps = new List<Step>(); 
        
        foreach (var expr in _expressions)
        {
            if (processed >= value - double.Epsilon)
                break;

            if (expr.IsLessThan)
            {
                var step = value > expr.Threshold
                    ? expr.Threshold - processed - double.Epsilon
                    : value - processed;

                var result = expr.Process(processed + step);

                if (result.HasValue)
                {
                    steps.Add(new Step
                    {
                        Lower = processed,
                        Upper = processed + step,
                        Value = result.Value
                    });

                    processed += step;
                }    
            } else if (expr.IsGreaterThan && processed >= expr.Threshold)
            {
                var result = expr.Process(value);

                if (result != null)
                {
                    steps.Add(new Step()
                    {
                        Lower = processed,
                        Upper = value,
                        Value = result.Value,
                    });

                    processed = value;
                }
            }
        }

        if (!steps.Any())
            return defaultValue;
        
        return steps.Sum(step => step.Value * step.Range) / steps.Sum(step => step.Range);
    }

    private struct Step
    {
        public double Lower { get; init; }
        public double Upper { get; init; }
        public double Value { get; init; }

        public double Range => Upper - Lower;
    }

    public enum ProcessMode
    {
        First,
        Aggregate
    }
}