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
        double thisBase = this.unit.ConvertToBaseUnit(this.value);
        double otherBase = other.unit.ConvertToBaseUnit(other.value);
        // return Math.Abs(this.ToBase() - other.ToBase()) < 0.0001; // This was till UC-7
        return Math.Abs(thisBase - otherBase) < 0.0001; // since UC-8
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
        
        if (this.unit.Category != targetUnit.Category) throw new InvalidOperationException("Cannot convert between different unit categories.");

        // First Converting current value to base ( which is feet here as per the suggestion )
        // double valueInBase = this.ToBase();
        double valueInBase = this.unit.ConvertToBaseUnit(this.value); // Since UC-8

        // Then  converting base value to target unit
        // double convertedValue = valueInBase / targetUnit.ConversionFactorToBase;
        double convertedValue = targetUnit.ConvertFromBaseUnit(valueInBase); // since uc- 8

        // Finally, Returning the  appropriate concrete type
        // if (targetUnit == Unit.Feet) return new Feet(convertedValue);
        // if (targetUnit == Unit.Inch) return new Inches(convertedValue);
        // if (targetUnit == Unit.Yard) return new Yard(convertedValue);
        // if (targetUnit == Unit.Meter) return new Meter(convertedValue);
        // if (targetUnit == Unit.Centimeter) return new Centimeter(convertedValue);

        // throw new InvalidOperationException("Unsupported unit conversion.");
        return CreateInstance(convertedValue, targetUnit);
    }


    // For uc - 6
    public Length Add(Length other)
    {
        /* if (other is null)
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
        return CreateInstance(resultValue, this.unit); */
        return Add(other, this.unit); // delegate to UC7 method
    }

    // For UC-7
    public Length Add(Length other, Unit targetUnit)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (targetUnit is null)
            throw new ArgumentNullException(nameof(targetUnit));

        if (double.IsNaN(other.value) || double.IsInfinity(other.value))
            throw new ArgumentException("Value must be a finite number.", nameof(other));

        // Step 1: Convert both to base (Feet)
        // double firstInBase = this.ToBase();
        // double secondInBase = other.ToBase();

        double firstInBase = this.unit.ConvertToBaseUnit(this.value);
        double secondInBase = other.unit.ConvertToBaseUnit(other.value);

        // Step 2: Add in base
        double sumInBase = firstInBase + secondInBase;

        // Step 3: Convert sum to target unit
        // double resultValue = sumInBase / targetUnit.ConversionFactorToBase;
        double resultValue = targetUnit.ConvertFromBaseUnit(sumInBase); // since uc -8 

        // Step 4: Return correct concrete instance
        return CreateInstance(resultValue, targetUnit);
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