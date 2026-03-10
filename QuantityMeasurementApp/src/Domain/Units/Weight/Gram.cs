using QuantityMeasurement.Domain.Core;

namespace QuantityMeasurement.Domain.Units;

public class Gram : Weight
{
    public Gram(double value) : base(value, Unit.Gram)
    {
    }
}