namespace QuantityMeasurement.Domain.Units;
using QuantityMeasurement.Domain.Core;
public class Centimeter : Length
{
    public Centimeter(double value) : base(value, Unit.Centimeter)
    {
    }
}