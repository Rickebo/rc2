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
        Options? opt = null,
        Func<double, double>? overrideInterest = null
    )
    {
        opt ??= _options;

        var currentBalance = CalculateCorrectedBalance(opt);
        var payment = overridePayment ?? opt.Payment;

        var yearlyPayment = payment * 12;
        var yearlyFees = opt.Fees * 12;
        var yearlyInflation = opt.Inflation;
        var yearlyDepreciation = opt.Depreciation;

        var timeStep = opt.TimeStepFactor;

        DataPoint? dp = null;

        for (var time = 0d; time < endTime; time += timeStep)
        {
            var yearlyInterest = overrideInterest?.Invoke(time) ?? opt.Interest;

            if (currentBalance < 0 && endAtZero)
            {
                if (accumulatedResultOnly)
                    yield return dp;

                break;
            }

            // If, for example, deduction is 0.3 (30%), then 30% of interest is deducted, hence factor should be 0.7.
            var currentYearlyInterestFactor = 1 - opt.InterestDeductionExpression.Process(
                currentBalance * yearlyInterest,
                mode: MultiExpression.ProcessMode.Aggregate
            );
            var currentYearlyInterest = yearlyInterest * currentYearlyInterestFactor;

            var currentLoanProportion = currentBalance / opt.Balance;
            var requiredAmortization = currentBalance * opt.MinAmortizationExpression.Process(
                currentLoanProportion
            );
            var currentYearlyInterestSum = currentBalance * currentYearlyInterest;

            var requiredYearlyPayment = currentYearlyInterestSum + yearlyFees + requiredAmortization;
            var currentYearlyPayment = yearlyPayment;

            if (currentYearlyPayment < requiredYearlyPayment)
            {
                if (!allowInsufficientPayment)
                    yield break;

                currentYearlyPayment = requiredYearlyPayment;
            }

            var currentYearlyAmortization = currentYearlyPayment - currentYearlyInterestSum - yearlyFees;

            dp = new DataPoint(
                Balance: currentBalance,
                Time: time,
                Amortization: currentYearlyAmortization / 12,
                Interest: currentYearlyInterestSum / 12,
                Payment: currentYearlyPayment / 12,
                Fees: yearlyFees / 12
            );

            if (!accumulatedResultOnly)
                yield return dp;

            currentBalance -= currentYearlyAmortization * timeStep;

            if (yearlyDepreciation > 0)
                currentBalance -= currentBalance * yearlyDepreciation * timeStep;

            yearlyPayment += yearlyPayment * yearlyInflation * timeStep;
            yearlyFees += yearlyFees * yearlyInflation * timeStep;
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

    public Dictionary<int, IEnumerable<InterestDataPoint>> SimulateInterestDelayed(
        int[] delays,
        double minInterest = 0,
        double maxInterest = 0.2,
        double interestStep = 0.01,
        double endTime = 100,
        double? overridePayment = null,
        double? overrideInflation = null
    )
    {
        var result = new Dictionary<int, IEnumerable<InterestDataPoint>>();
        foreach (var delay in delays)
        {
            var points = SimulateInterest(
                minInterest: minInterest,
                maxInterest: maxInterest,
                interestStep: interestStep,
                endTime: endTime,
                overridePayment: overridePayment,
                overrideInflation: overrideInflation,
                timeDelay: delay
            );
            
            result.Add(delay, points.ToList());
        }

        return result;
    }

    public IEnumerable<InterestDataPoint> SimulateInterest(
        double minInterest = 0,
        double maxInterest = 0.2,
        double interestStep = 0.01,
        double endTime = 100,
        double? overridePayment = null,
        double? overrideInflation = null,
        double? timeDelay = null
    )
    {
        var opt = _options.Clone();
        var inflation = overrideInflation ?? opt.Inflation;
        var payment = overridePayment ?? opt.Payment;

        opt.Inflation = inflation;
        opt.Payment = payment;

        for (var interest = minInterest; interest < maxInterest; interest += interestStep)
        {
            var localInterest = interest;
            var dps = Simulate(
                endAtZero: true,
                endTime: endTime,
                accumulatedResultOnly: false,
                allowInsufficientPayment: true,
                opt: opt,
                overrideInterest: time =>
                    timeDelay == null || time >= timeDelay
                        ? localInterest
                        : opt.Interest
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

        var min = opt.Balance * minBalanceFactor;
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