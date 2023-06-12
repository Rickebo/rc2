using ApexCharts;

namespace rc2.Data;

public record SimulationResult(
    IEnumerable<DataPoint> DataPoints,
    IEnumerable<YearDataPoint> YearDataPoints,
    Dictionary<int, IEnumerable<InterestDataPoint>> InterestDataPoints,
    IEnumerable<InterestDataPoint> InterestProportionDataPoints,
    IEnumerable<BalanceDataPoint> BalanceDataPoints
)
{
    public static SimulationResult Empty { get; } = new (
        DataPoints: Enumerable.Empty<DataPoint>(),
        YearDataPoints: Enumerable.Empty<YearDataPoint>(),
        InterestDataPoints: new Dictionary<int, IEnumerable<InterestDataPoint>>(),
        InterestProportionDataPoints: Enumerable.Empty<InterestDataPoint>(),
        BalanceDataPoints: Enumerable.Empty<BalanceDataPoint>()
    );
}