using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace rc2.Settings;

public class BindableOptions : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public Func<Options> Options { get; }

    public BindableOptions(Func<Options> options)
    {
        Options = options;
    }

    public void UpdateAll()
    {
        var properties = new[]
        {
            nameof(InterestPercent),
            nameof(DepositPercent),
            nameof(DepreciationPercent),
            nameof(InflationPercent),
            nameof(Balance),
            nameof(Payment),
            nameof(Fees),
            nameof(TimeStep),
            nameof(MinAmortization),
            nameof(InterestDeduction)
        };
        
        foreach (var property in properties)
            OnPropertyChanged(property);
    }

    public double InterestPercent
    {
        get => Options().Interest * 100;
        set => SetValue(
            nameof(InterestPercent),
            () => Options().Interest = value / 100
        );
    }

    public double DepositPercent
    {
        get => Options().Deposit * 100;
        set => SetValue(
            nameof(DepositPercent),
            () => Options().Deposit = value / 100
        );
    }

    public double DepreciationPercent
    {
        get => Options().Depreciation * 100;
        set => SetValue(
            nameof(DepreciationPercent),
            () => Options().Depreciation = value / 100
        );
    }

    public double InflationPercent
    {
        get => Options().Inflation * 100;
        set => SetValue(
            nameof(InflationPercent),
            () => Options().Inflation = value / 100
        );
    }

    public double Balance
    {
        get => Options().Balance;
        set => SetValue(
            nameof(Balance),
            () => Options().Balance = value
        );
    }

    public double Payment
    {
        get => Options().Payment;
        set => SetValue(
            nameof(Payment),
            () => Options().Payment = value
        );
    }

    public double Fees
    {
        get => Options().Fees;
        set => SetValue(
            nameof(Fees),
            () => Options().Fees = value
        );
    }

    public string TimeStep
    {
        get => Options().TimeStep;
        set => SetValue(
            nameof(TimeStep),
            () => Options().TimeStep = value
        );
    }

    public string MinAmortization
    {
        get => Options().MinAmortization;
        set => SetValue(
            nameof(MinAmortization),
            () => Options().MinAmortization = value
        );
    }

    public string InterestDeduction
    {
        get => Options().InterestDeduction;
        set => SetValue(
            nameof(InterestDeduction), 
            () => Options().InterestDeduction = value
            );
    }

    private void SetValue(string propertyName, Action updater)
    {
        updater();
        OnPropertyChanged(propertyName);
    }
    
    public virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}