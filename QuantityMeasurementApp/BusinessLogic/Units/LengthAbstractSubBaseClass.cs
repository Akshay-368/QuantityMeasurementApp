using  QuantityMeasurement.Core;
namespace QuantityMeasurement.Units;

public abstract class Length : Quantity
{
    protected Length(double value, Unit unit) : base(value, unit)
    {
    }

    protected double ToBase()
    {
        return value * unit.ConversionFactorToBase;// to get the value in base units
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null) return false;

        // Ensures only Length types compare
        if (obj is not Length other) return false;

        // return this.ToBase().Equals(other.ToBase()); 
        // Using Math.Abs to avoid rounding errors in double which could be a headache.
        return Math.Abs(this.ToBase() - other.ToBase()) < 0.0001;
    }

    public override int GetHashCode()
    {
        return ToBase().GetHashCode();
    }
}