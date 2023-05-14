namespace rc2;

public class Expression
{
    private readonly string _text;
    private readonly Func<double, double?> _processor;
    
    private Expression(string text, Func<double, double?> processor)
    {
        _text = text;
        _processor = processor;
    }

    private static bool IsDigit(char ch) => char.IsDigit(ch) || ch is ',' or '.';
    
    private static bool AreEqual(double a, double b) => Math.Abs(a - b) < 0.0001;

    public static Expression Parse(string text)
    {
        var split = text.Split(new[] { "=" }, StringSplitOptions.TrimEntries);

        if (split.Length < 2)
        {
            if (!double.TryParse(text, out var constant))
                throw new ArgumentException($"Invalid constant: {text}");

            return new Expression(text, _ => constant);
        }
        
        var opPart = string.Join("=", split.Take(split.Length - 1));
        var valuePart = split.Last();
        
        var op = new string(opPart.Where(ch => !IsDigit(ch)).ToArray());
        var operatorValueString = new string(opPart.Where(IsDigit).ToArray());
        
        if (!double.TryParse(operatorValueString, out var operatorValue))
            throw new ArgumentException($"Invalid operator value: {operatorValueString}");
        
        if (!double.TryParse(valuePart, out var value))
            throw new ArgumentException($"Invalid value: {valuePart}");

        Func<double, double?> processor = op switch
        {
            "<" => x => x < operatorValue ? value : null,
            "≤" => x => x < operatorValue ? value : null,
            "<=" => x => x <= operatorValue ? value : null,
            ">" => x => x > operatorValue ? value : null,
            ">=" => x => x >= operatorValue ? value : null,
            "≥" => x => x >= operatorValue ? value : null,
            "=" => x => AreEqual(x, operatorValue) ? value : null,
            "!=" => x => !AreEqual(x, operatorValue) ? value : null,
            _ => throw new ArgumentException($"Unknown operator: {op}")
        };

        return new Expression(text, processor);
    }

    public double? Process(double input) => _processor(input);
}