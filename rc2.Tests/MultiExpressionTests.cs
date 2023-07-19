namespace rc2.Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var exp = MultiExpression.Parse("<=100000=0.3;>=100000=0.21");
        var expected = (100000 * 0.3 + 100000 * 0.21) / 200000;
        var result = exp.Process(200000, 0, MultiExpression.ProcessMode.Aggregate);
        
        Assert.That(result, Is.EqualTo(expected).Within(0.00001));
    }
}