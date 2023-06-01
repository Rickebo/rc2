namespace rc2.Data;

public record DataPoint(
    double Time,
    double Balance,
    double Payment,
    double Amortization,
    double Interest,
    double Fees
)
{
    public static DataPoint Zero =>
        new(
            0,
            0,
            0,
            0,
            0,
            0
        );
}