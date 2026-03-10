using QuantityMeasurement.Core;

namespace QuantityMeasurement.Units;

public class Gram : Weight
{
    public Gram(double value) : base(value, Unit.Gram)
    {
    }
}