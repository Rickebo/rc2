using System.Globalization;

namespace rc2;

public class Expression
{
    private readonly string _text;
    private readonly Func<double, double?> _processor;
    public double Threshold { get; }
    public string Operator { get; }
    
    public double InclusiveStart { get; }
    public double ExclusiveEnd { get; }
    
    public bool IsLessThan => Operator is "<" or "<=" or "≤";
    public bool IsGreaterThan => Operator is ">" or ">=" or "≥";
    
    private Expression(string text, Func<double, double?> processor, string op, double threshold)
    {
        _text = text;
        _processor = processor;

        Operator = op;
        Threshold = threshold;

        switch (op)
        {
            case "<":
                InclusiveStart = double.NegativeInfinity;
                ExclusiveEnd = threshold;
                break;
            
            case "≤":
            case "<=":
                InclusiveStart = double.NegativeInfinity;
                ExclusiveEnd = threshold + double.Epsilon;
                break;
            
            case ">":
                InclusiveStart = threshold + double.Epsilon;
                ExclusiveEnd = double.PositiveInfinity;
                break;
            
            case "≥":
            case ">=":
                InclusiveStart = threshold;
                ExclusiveEnd = double.PositiveInfinity;
                break;
            
            case "=":
                InclusiveStart = threshold;
                ExclusiveEnd = threshold + double.Epsilon;
                break;
            
            case "!=":
                InclusiveStart = threshold + double.Epsilon;
                ExclusiveEnd = threshold;
                break;
        }
    }

    private static bool IsDigit(char ch) => char.IsDigit(ch) || ch is ',' or '.';
    
    private static bool AreEqual(double a, double b) => Math.Abs(a - b) < 0.0001;

    public static Expression Parse(string text)
    {
        var split = text.Split(new[] { "=" }, StringSplitOptions.TrimEntries);

        if (split.Length < 2)
        {
            if (!TryParseDouble(text, out var constant))
                throw new ArgumentException($"Invalid constant: {text}");

            return new Expression(text, _ => constant, "c", constant);
        }
        
        var opPart = string.Join("=", split.Take(split.Length - 1));
        var valuePart = split.Last();
        
        var op = new string(opPart.Where(ch => !IsDigit(ch)).ToArray());
        var operatorValueString = new string(opPart.Where(IsDigit).ToArray());
        
        if (!TryParseDouble(operatorValueString, out var operatorValue))
            throw new ArgumentException($"Invalid operator value: {operatorValueString}");
        
        if (!TryParseDouble(valuePart, out var value))
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

        return new Expression(text, processor, op, operatorValue);
    }

    public double? Process(double input) => _processor(input);
    public static bool TryParseDouble(string input, out double output) => 
        double.TryParse(input, NumberStyles.Any, CultureInfo.InvariantCulture, out output);
}