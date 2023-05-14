using System.Text.Json.Serialization;

namespace rc2.Settings;

public class Options
{
    private static readonly Dictionary<string, double> TimeStepOperatorFactors =
        new()
        {
            ["y"] = 1,
            ["m"] = 12,
            ["w"] = 52,
            ["d"] = 365,
            ["h"] = 365 * 24
        };

    public static Options Default { get; } = new()
    {
        Interest = 0.04,
        Payment = 14000,
        Balance = 2500000,
        Deposit = 0.15,
        Depreciation = 0,
        Inflation = 0.00,
        MinAmortization = ">=0.7=0.03;<0.7=0.02",
        InterestDeduction = "<=100000=0.3;>=100000=0.21",
        Fees = 3500,
        TimeStep = "1m"
    };
    
    public double Interest { get; set; }
    public double Payment { get; set; }
    public double Balance { get; set; }
    public double Deposit { get; set; }
    public double Depreciation { get; set; }
    public double Inflation { get; set; }

    public string MinAmortization
    {
        get => _minAmortization;
        set
        {
            _minAmortization = value;
            MinAmortizationExpression = MultiExpression.Parse(value);
        }
    }

    public string InterestDeduction
    {
        get => _interestDeduction;
        set
        {
            _interestDeduction = value;
            InterestDeductionExpression = MultiExpression.Parse(value);
        }
    }

    [JsonIgnore]
    public MultiExpression MinAmortizationExpression = MultiExpression.Zero;
   
    [JsonIgnore]
    public MultiExpression InterestDeductionExpression = MultiExpression.Zero;
    
    public double Fees { get; set; }

    [JsonIgnore]
    public double TimeStepFactor = 1 / 12d;
    public string TimeStep
    {
        get => _timeStep;
        set
        {
            _timeStep = value;
            TimeStepFactor = ParseTimeStep(value);
        }
    }

    [JsonIgnore]
    private string _timeStep = "1m";

    [JsonIgnore]
    private string _minAmortization = "0";

    [JsonIgnore]
    private string _interestDeduction = "0";
    
    

    public static double ParseTimeStep(string text)
    {
        var mode = new string(text.Where(char.IsLetter).ToArray());
        var rest = new string(text.Where(ch => !char.IsLetter(ch)).ToArray());
        
        if (!TimeStepOperatorFactors.TryGetValue(mode.ToLowerInvariant(), out var factor))
            throw new ArgumentException($"Unknown time step mode: {mode}");
        
        if (!double.TryParse(rest, out var value))
            throw new ArgumentException($"Invalid time step value: {rest}");

        return value / factor;
    }

    public Options Clone() => (Options) MemberwiseClone();
}