using Contracts.DevicesMetricsGenerator;
using DevicesMetricsGenerator.Models;
using MathNet.Numerics.Distributions;

namespace DevicesMetricsGenerator;

internal sealed class TemperatureSimulator
{
    private readonly Normal _normalDist;
    private readonly TimeProvider _timeProvider;
    private double _currentTemperature;
    private int _currentHour; // 24-hour format
    private int _currentMonth; // 1 to 12

    public TemperatureSimulator(double initialTemperature, int initialHour, int initialMonth, TimeProvider timeProvider)
    {
        _currentTemperature = initialTemperature;
        _currentHour = initialHour;
        _currentMonth = initialMonth;
        _timeProvider = timeProvider;
        //TODO use different random Distribution for months?
        _normalDist = new Normal(0, 0.5); // Std deviation to simulate random fluctuations
    }

    private TemperatureAdjustment Adjustment(int month, int hour)
    {
        var dayHours = hour is >= 9 and <= 17;
        
        // Simplified adjustment: higher in June (6) and lower in December (12)
        // This is a very basic approximation
        switch (month)
        {
            case 12:
            case 1:
            case 2:
                var adjustment = new TemperatureAdjustment(-0.5, dayHours ? 1 : -0.5, 0);
                if (_currentTemperature <= -15)
                {
                    adjustment = adjustment with { AdditionalAdjustment = 2 };
                }
                return adjustment;
            case 3:
            case 4:
            case 5:
                var adjustmentSpring = new TemperatureAdjustment(0, dayHours ? 1.5 : -1, 0);
                if (_currentTemperature <= 0)
                {
                    adjustmentSpring = adjustmentSpring with { AdditionalAdjustment = 2 };
                }
                return adjustmentSpring;
            case 6:
            case 7:
            case 8:
                return new TemperatureAdjustment(1, dayHours ? 1 : -2, 0);
            default:
                return new TemperatureAdjustment(0, dayHours ? 1 : -1, 0);
        }
    }

    private double DiurnalAdjustment(int hour)
    {
        // Warmer during the day (9 AM to 5 PM) and cooler at night
        if (hour is >= 9 and <= 17)
            return 1;
        else if (hour is >= 18 and <= 23)
            return -2;
        else
            return 0;
    }

    /// <summary>
    /// Gets temperature for next hour. After each 24h takes temparature for next month
    /// </summary>
    /// <returns></returns>
    public SensorMetric GetNextTemperature()
    {
        var baseChange = _normalDist.Sample();
        var adjustment = Adjustment(_currentMonth, _currentHour);
        // var diurnalAdjustment = DiurnalAdjustment(_currentHour);

        // Adjust base change by seasonal and diurnal factors
        double adjustedChange = baseChange + adjustment.SeasonalAdjustment + adjustment.DiurnalAdjustment + adjustment.AdditionalAdjustment;
        _currentTemperature += adjustedChange;

        // Update hour and month for next call (simplified, assumes each call advances one hour)
        _currentHour = (_currentHour + 1) % 24;
        if (_currentHour == 0) // Assuming a new day
        {
            // Advance the month every 24 calls to this method (simplified monthly progression)
            _currentMonth = (_currentMonth % 12) + 1;
        }

        return new SensorMetric(MetricType.Temperature, _timeProvider.GetUtcNow().UtcDateTime,  _currentTemperature);
    }
}