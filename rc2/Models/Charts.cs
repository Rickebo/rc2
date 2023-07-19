using ApexCharts;
using rc2.Data;

namespace rc2.Models;

public class Charts
{
    public const string MoneyFormat =
        "function (value, time, opts) {" +
        "  return value > 1000000" +
        "    ? (value / 1000000).toFixed(2) + 'M'" +
        "    : value > 1000 " +
        "        ? (value / 1000).toFixed(1) + 'k' " +
        "        : value.toFixed(0)" +
        "}";

    public const string PercentageFormat =
        "function (value, time, opts) {" +
        "  return value.toFixed(1) + '%'" +
        "}";

    private const string BackgroundColor = "transparent";



    public Theme Theme { get; set; }

    public ApexChart<DataPoint> BalanceChart { get; set; }
    public ApexChartOptions<DataPoint> BalanceOptions { get; }

    public ApexChart<DataPoint> TotalCostsChart { get; set; }
    public ApexChartOptions<DataPoint> TotalCostsOptions { get; }

    public ApexChart<DataPoint> CostsChart { get; set; }
    public ApexChartOptions<DataPoint> CostsOptions { get; }

    public ApexChart<Relation> ExpensesChart { get; set; }
    public ApexChartOptions<Relation> ExpensesOptions { get; }


    public ApexChart<InterestDataPoint> InterestChart { get; set; }
    public ApexChartOptions<InterestDataPoint> InterestOptions { get; }


    public ApexChart<InterestDataPoint> InterestProportionChart { get; set; }
    public ApexChartOptions<InterestDataPoint> InterestProportionOptions { get; }


    public ApexChart<BalanceDataPoint> BalanceCostChart { get; set; }
    public ApexChartOptions<BalanceDataPoint> BalanceCostOptions { get; } 

    public Charts(bool darkMode = true)
    {
        Theme = new()
        {
            Mode = darkMode 
                ? Mode.Dark
                : Mode.Light,
            
        };
        
        BalanceOptions = new()
        {
            Theme = Theme,
            Chart = new Chart()
            {
                Background = BackgroundColor
            },
            Yaxis = new List<YAxis>()
            {
                new YAxis()
                {
                    DecimalsInFloat = 0,
                    AxisTicks = new AxisTicks()
                    {
                    },
                    Title = new AxisTitle()
                    {
                        Text = "Loan balance"
                    },
                    Labels = new YAxisLabels()
                    {
                        Formatter = MoneyFormat
                    }
                }
            },
            Xaxis = new XAxis()
            {
                Type = XAxisType.Numeric,
                // TickAmount = 10,
                TickPlacement = TickPlacement.On,
                Title = new AxisTitle()
                {
                    Text = "Time (years)"
                }
            }
        };
        
        CostsOptions = new()
        {
            Theme = Theme,
            Chart = new Chart()
            {
                Background = BackgroundColor
            },
            Yaxis = new List<YAxis>()
            {
                new YAxis()
                {
                    DecimalsInFloat = 0,
                    AxisTicks = new AxisTicks()
                    {
                    },
                    Title = new AxisTitle()
                    {
                        Text = "Monthly expense"
                    },
                }
            },
            Xaxis = new XAxis()
            {
                Type = XAxisType.Numeric,
                // TickAmount = 10,
                TickPlacement = TickPlacement.On,
                Title = new AxisTitle()
                {
                    Text = "Time (years)"
                }
            }
        };
        
        TotalCostsOptions = new()
        {
            Theme = Theme,
            Chart = new Chart()
            {
                Background = BackgroundColor
            },
            Yaxis = new List<YAxis>()
            {
                new YAxis()
                {
                    DecimalsInFloat = 0,
                    AxisTicks = new AxisTicks()
                    {
                    },
                    Title = new AxisTitle()
                    {
                        Text = "Total expense"
                    },
                }
            },
            Xaxis = new XAxis()
            {
                Type = XAxisType.Numeric,
                // TickAmount = 10,
                TickPlacement = TickPlacement.On,
                Title = new AxisTitle()
                {
                    Text = "Time (years)"
                }
            }
        };
        
        ExpensesOptions = new()
        {
            Theme = Theme,
            Chart = new Chart()
            {
                Background = BackgroundColor
            },
            Yaxis = new List<YAxis>()
            {
                new YAxis()
                {
                    DecimalsInFloat = 0,
                    AxisTicks = new AxisTicks()
                    {
                    },
                    Labels = new YAxisLabels()
                    {
                        Format = MoneyFormat
                    }
                }
            },
            Xaxis = new XAxis()
            {
                Type = XAxisType.Numeric,
                // TickAmount = 10,
                TickPlacement = TickPlacement.On
            }
        };
        
        InterestOptions = new()
        {
            Theme = Theme,
            Chart = new Chart()
            {
                Background = BackgroundColor
            },
            Yaxis = new List<YAxis>()
            {
                new YAxis()
                {
                    DecimalsInFloat = 0,
                    AxisTicks = new AxisTicks()
                    {
                    },
                    Title = new AxisTitle()
                    {
                        Text = "Min monthly payment"
                    }
                }
            },
            Xaxis = new XAxis()
            {
                Type = XAxisType.Numeric,
                // TickAmount = 10,
                DecimalsInFloat = 1,
                Labels = new XAxisLabels()
                {
                    Formatter = PercentageFormat
                },
                Title = new AxisTitle()
                {
                    Text = "Interest rate"
                }
            }
        };
        
        InterestProportionOptions = new()
        {
            Theme = Theme,
            Chart = new Chart()
            {
                Background = BackgroundColor
            },
            Yaxis = new List<YAxis>()
            {
                new YAxis()
                {
                    DecimalsInFloat = 0,
                    AxisTicks = new AxisTicks()
                    {
                    },
                    Labels = new()
                    {
                        Formatter = "function (value, time, formatter) { return value.toFixed(0) + '%' }"
                    },
                    Title = new AxisTitle()
                    {
                        Text = "Proportion"
                    }
                }
            },
            Xaxis = new XAxis()
            {
                Type = XAxisType.Numeric,
                // TickAmount = 10,
                DecimalsInFloat = 1,
                Labels = new XAxisLabels()
                {
                    Formatter = PercentageFormat
                },
                Title = new AxisTitle()
                {
                    Text = "Interest rate"
                }
            }
        };
        
        BalanceCostOptions = new()
        {
            Theme = Theme,
            Chart = new Chart()
            {
                Background = BackgroundColor
            },
            Yaxis = new List<YAxis>()
            {
                new YAxis()
                {
                    DecimalsInFloat = 0,
                    AxisTicks = new AxisTicks()
                    {
                    },
                    Title = new AxisTitle()
                    {
                        Text = "Max required monthly payment"
                    }
                }
            },
            Xaxis = new XAxis()
            {
                Type = XAxisType.Numeric,
                DecimalsInFloat = 1,
                Labels = new XAxisLabels()
                {
                    Formatter = MoneyFormat
                },
                Title = new AxisTitle()
                {
                    Text = "Loan"
                }
            }
        };
    }

    public void SetDarkMode(bool darkMode = true)
    {
        Theme.Mode = darkMode 
            ? Mode.Dark
            : Mode.Light;
    }

    public async Task UpdateAll()
    {
        var tasks = new []
        {
            BalanceChart?.UpdateSeriesAsync(),
            CostsChart?.UpdateSeriesAsync(),
            ExpensesChart?.UpdateSeriesAsync(),
            InterestChart?.UpdateSeriesAsync(),
            InterestProportionChart?.UpdateSeriesAsync(),
            BalanceCostChart?.UpdateSeriesAsync(),
            TotalCostsChart?.UpdateSeriesAsync()
        };

        await Task.WhenAll(tasks
            .Where(task => task != null)
            .Select(task => task!)
        );
    }

    public async Task UpdateAllOptions()
    {
        var tasks = new[]
        {
            BalanceChart?.UpdateOptionsAsync(true, true, true),
            CostsChart?.UpdateOptionsAsync(true, true, true),
            ExpensesChart?.UpdateOptionsAsync(true, true, true),
            InterestChart?.UpdateOptionsAsync(true, true, true),
            InterestProportionChart?.UpdateOptionsAsync(true, true, true),
            BalanceCostChart?.UpdateOptionsAsync(true, true, true),
            TotalCostsChart?.UpdateOptionsAsync(true, true, true)
        };

        await Task.WhenAll(tasks
            .Where(task => task != null)
            .Select(task => task!)
        );
    }
}