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

    /// <summary>
    /// Returns the string representation of the length
    /// As overriding is required here ,as when I tried printing 
    /// the object directly , C# will calls converted.ToString() 
    /// and since i would not have override it ,ToString() will 
    /// default to QuantityMeasurement.Units.Centimeter
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
        // return $"{value} {unit}";
        return $"{Math.Round(value, 4)} {unit}"; // rounding it , instead of just truncating or raw direct printing of the value. which could even lead to something like 10m = 999.99999999999 Centimeters
    }


    public Length ConvertTo(Unit targetUnit)
    {
        if (targetUnit == null)
            throw new ArgumentNullException(nameof(targetUnit));

        // First Converting current value to base ( which is feet here as per the suggestion )
        double valueInBase = this.ToBase();

        // Then  converting base value to target unit
        double convertedValue = valueInBase / targetUnit.ConversionFactorToBase;

        // Finally, Returning the  appropriate concrete type
        if (targetUnit == Unit.Feet) return new Feet(convertedValue);
        if (targetUnit == Unit.Inch) return new Inches(convertedValue);
        if (targetUnit == Unit.Yard) return new Yard(convertedValue);
        if (targetUnit == Unit.Meter) return new Meter(convertedValue);
        if (targetUnit == Unit.Centimeter) return new Centimeter(convertedValue);

        throw new InvalidOperationException("Unsupported unit conversion.");
    }

}