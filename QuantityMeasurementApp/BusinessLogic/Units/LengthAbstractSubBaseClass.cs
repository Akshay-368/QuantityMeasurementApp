using  QuantityMeasurement.Core;
namespace QuantityMeasurement.Units;

public abstract class Length : Quantity
{
    protected Length(double value, Unit unit) : base(value, unit)
    {
    }

    public new Length ConvertTo( Unit targetUnit)
        {
            return (Length)base.ConvertTo(targetUnit);
        }


    // For uc - 6
    public Length Add(Length other)
    {
       return (Length)base.Add(other); 
    }

    // For UC-7
    public Length Add(Length other, Unit targetUnit)
    {
        return (Length)base.Add(other, targetUnit);
    }


    protected override Quantity CreateInstance(double value, Unit unit)
    {
        if (unit == Unit.Feet) return new Feet(value);
        if (unit == Unit.Inch) return new Inches(value);
        if (unit == Unit.Yard) return new Yard(value);
        if (unit == Unit.Meter) return new Meter(value);
        if (unit == Unit.Centimeter) return new Centimeter(value);

        throw new InvalidOperationException("Unsupported unit.");
    }
}