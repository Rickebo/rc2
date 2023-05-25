using rc2.Data;
using rc2.Settings;

namespace rc2;

public class Simulator
{
    private readonly Options _options;

    public Simulator(Options options)
    {
        _options = options;
    }

    private double CalculateMinPayment() =>
        _options.TimeStepFactor * _options.Balance * (
            _options.MinAmortizationExpression.Process(_options.Balance) +
            _options.Interest
        );

    private double CalculateCorrectedBalance(Options? opt = null)
    {
        opt ??= _options;
        return opt.Balance - opt.Balance * opt.Deposit;
    }

    public IEnumerable<DataPoint?> Simulate(
        double endTime = 100,
        bool endAtZero = true,
        bool accumulatedResultOnly = false,
        double? overridePayment = null,
        bool allowInsufficientPayment = false,
        Options? opt = null
    )
    {
        opt ??= _options;

        var x = CalculateCorrectedBalance(opt);
        var payment = overridePayment ?? opt.Payment;

        var monthlyPayment = payment;
        var fees = opt.Fees;
        var timeStep = opt.TimeStepFactor;
        var monthlyTimeStep = timeStep / 12;
        var monthlyTimeFactor = (1 / timeStep) / 12;

        DataPoint? dp = null;

        for (var time = 0d; time < endTime; time += timeStep)
        {
            if (x < 0 && endAtZero)
            {
                if (accumulatedResultOnly)
                    yield return dp;

                break;
            }

            var currentFees = fees * monthlyTimeStep;
            var yearlyInterest = x * opt.Interest;
            var interestDeduction = opt.InterestDeductionExpression.Process(yearlyInterest);
            var currentInterest = x * opt.Interest * timeStep;

            currentInterest -= currentInterest * interestDeduction;

            var xPercentage = x / opt.Balance;
            var currentMinAmortization = x * timeStep * opt.MinAmortizationExpression.Process(xPercentage);
            var requiredPayment = currentInterest + currentMinAmortization + currentFees;
            var currentPayment = monthlyPayment * monthlyTimeStep;

            if (!allowInsufficientPayment && currentPayment < requiredPayment)
                yield break;

            var actualPayment = Math.Max(requiredPayment, monthlyPayment);
            var amortization = actualPayment - currentInterest - currentFees;

            dp = new DataPoint(
                Balance: x,
                Time: time,
                Amortization: amortization * monthlyTimeFactor,
                Interest: currentInterest * monthlyTimeFactor,
                Payment: actualPayment * monthlyTimeFactor,
                Fees: fees * monthlyTimeFactor
            );

            if (!accumulatedResultOnly)
                yield return dp;

            x -= amortization;
            x -= timeStep * opt.Depreciation;

            monthlyPayment += monthlyPayment * opt.Inflation * timeStep;
            fees += fees * opt.Inflation * timeStep;
        }
    }

    public IEnumerable<YearDataPoint> SimulateYears(
        double? minPayment = null,
        double? maxPayment = null,
        double paymentStep = 100,
        double endTime = 100
    )
    {
        var min = minPayment ?? CalculateMinPayment();
        var max = maxPayment ?? CalculateCorrectedBalance() / (12 * 5);

        for (var payment = min; payment < max; payment += paymentStep)
        {
            var dp = Simulate(
                endTime: endTime,
                endAtZero: true,
                accumulatedResultOnly: true,
                overridePayment: payment
            ).LastOrDefault();

            if (dp is null)
                continue;

            yield return new YearDataPoint(
                Time: dp.Time,
                Payment: payment
            );
        }
    }

    public IEnumerable<InterestDataPoint> SimulateInterest(
        double minInterest = 0,
        double maxInterest = 0.2,
        double interestStep = 0.01,
        double endTime = 100,
        double? overridePayment = null,
        double? overrideInflation = null
    )
    {
        var opt = _options.Clone();
        var inflation = overrideInflation ?? opt.Inflation;
        var payment = overridePayment ?? opt.Payment;

        opt.Inflation = inflation;
        opt.Payment = payment;

        for (var interest = minInterest; interest < maxInterest; interest += interestStep)
        {
            opt.Interest = interest;
            var dps = Simulate(
                endAtZero: true,
                endTime: endTime,
                accumulatedResultOnly: false,
                allowInsufficientPayment: true,
                opt: opt
            );

            double? maxPayment = null;
            var totalInterest = 0d;

            foreach (var dp in dps)
            {
                if (dp == null)
                    continue;

                if (maxPayment == null || dp.Payment > maxPayment)
                    maxPayment = dp.Payment;

                totalInterest += dp.Interest;
            }

            if (maxPayment == null)
                continue;

            yield return new InterestDataPoint(
                Interest: interest,
                MaxPayment: maxPayment.Value,
                InterestProportion: totalInterest / _options.Balance
            );
        }
    }

    public IEnumerable<BalanceDataPoint> SimulateBalance(
        double minBalanceFactor = 0,
        double maxBalanceFactor = 2,
        double balanceStep = 0.01,
        double endTime = 100,
        double? overridePayment = null,
        double? overrideInflation = null
    )
    {
        var opt = _options.Clone();
        var inflation = overrideInflation ?? opt.Inflation;
        var payment = overridePayment ?? opt.Payment;

        opt.Inflation = inflation;
        opt.Payment = payment;

        var min= opt.Balance * minBalanceFactor;
        var max = opt.Balance * maxBalanceFactor;
        var step = (max - min) * balanceStep;

        for (
            var balance = min;
            balance < max;
            balance += step
        )
        {
            opt.Balance = balance;
            var dps = Simulate(
                endTime: endTime,
                endAtZero: true,
                accumulatedResultOnly: false,
                allowInsufficientPayment: true,
                opt: opt
            );

            double? maxPayment = null;
            foreach (var dp in dps)
            {
                if (dp == null)
                    continue;

                if (maxPayment == null || dp.Payment > maxPayment)
                    maxPayment = dp.Payment;
            }

            if (maxPayment == null)
                continue;

            yield return new BalanceDataPoint(
                Balance: balance,
                MaxPayment: maxPayment.Value
            );
        }
    }
}