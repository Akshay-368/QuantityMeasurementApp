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
        // return ToBase().GetHashCode();
        return Math.Round(ToBase(), 4).GetHashCode();
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
        // if (targetUnit == Unit.Feet) return new Feet(convertedValue);
        // if (targetUnit == Unit.Inch) return new Inches(convertedValue);
        // if (targetUnit == Unit.Yard) return new Yard(convertedValue);
        // if (targetUnit == Unit.Meter) return new Meter(convertedValue);
        // if (targetUnit == Unit.Centimeter) return new Centimeter(convertedValue);

        // throw new InvalidOperationException("Unsupported unit conversion.");
        return CreateInstance(convertedValue, targetUnit);
    }



    public Length Add(Length other)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (double.IsNaN(other.value) || double.IsInfinity(other.value))
            throw new ArgumentException("Value must be a finite number.", nameof(other));

        // Step 1: Convert both to base (Feet)
        double firstInBase = this.ToBase();
        double secondInBase = other.ToBase();

        // Step 2: Add in base
        double sumInBase = firstInBase + secondInBase;

        // Step 3: Convert back to THIS unit
        double resultValue = sumInBase / this.unit.ConversionFactorToBase;

        // Step 4: Return correct concrete type
        return CreateInstance(resultValue, this.unit);
    }


    private Length CreateInstance(double value, Unit unit)
    {
        if (unit == Unit.Feet) return new Feet(value);
        if (unit == Unit.Inch) return new Inches(value);
        if (unit == Unit.Yard) return new Yard(value);
        if (unit == Unit.Meter) return new Meter(value);
        if (unit == Unit.Centimeter) return new Centimeter(value);

        throw new InvalidOperationException("Unsupported unit.");
    }
}