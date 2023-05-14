using System.Collections;
using ApexCharts;

namespace rc2.Data;

public class Relation
{
    public double Value { get; }
    public string Name { get; }
    
    public Relation(IEnumerable<DataPoint> dataPoints, Func<DataPoint, double> selector, string name)
    {
        Value = dataPoints.Sum(selector);
        Name = name;
    }
}