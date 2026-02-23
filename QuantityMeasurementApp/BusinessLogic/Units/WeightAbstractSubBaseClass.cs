using QuantityMeasurement.Core;

namespace QuantityMeasurement.Units;

public abstract class Weight : Quantity
{
    protected Weight(double value, Unit unit) : base(value, unit)
    {
    }

    // Convert current value to base (Kilogram)
    protected double ToBase()
    {
        return unit.ConvertToBaseUnit(value);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        if (obj is null) return false;

        // Only allow comparison between Weight types
        if (obj is not Weight other) return false;

        double thisBase = this.unit.ConvertToBaseUnit(this.value);
        double otherBase = other.unit.ConvertToBaseUnit(other.value);

        return Math.Abs(thisBase - otherBase) < 0.0001;
    }

    public override int GetHashCode()
    {
        return Math.Round(ToBase(), 4).GetHashCode();
    }

    public override string ToString()
    {
        return $"{Math.Round(value, 4)} {unit}";
    }

    // --------------------------
    // Convert
    // --------------------------
    public Weight ConvertTo(Unit targetUnit)
    {
        if (targetUnit is null)
            throw new ArgumentNullException(nameof(targetUnit));

        if (this.unit.Category != targetUnit.Category) throw new InvalidOperationException("Cannot convert between different unit categories.");

        double valueInBase = this.unit.ConvertToBaseUnit(this.value);
        double convertedValue = targetUnit.ConvertFromBaseUnit(valueInBase);

        return CreateInstance(convertedValue, targetUnit);
    }

    // --------------------------
    // Add (default to this unit)
    // --------------------------
    public Weight Add(Weight other)
    {
        return Add(other, this.unit);
    }

    // --------------------------
    // Add with target unit
    // --------------------------
    public Weight Add(Weight other, Unit targetUnit)
    {
        if (other is null)
            throw new ArgumentNullException(nameof(other));

        if (targetUnit is null)
            throw new ArgumentNullException(nameof(targetUnit));

        if (double.IsNaN(other.value) || double.IsInfinity(other.value))
            throw new ArgumentException("Value must be a finite number.", nameof(other));

        double firstInBase = this.unit.ConvertToBaseUnit(this.value);
        double secondInBase = other.unit.ConvertToBaseUnit(other.value);

        double sumInBase = firstInBase + secondInBase;

        double resultValue = targetUnit.ConvertFromBaseUnit(sumInBase);

        return CreateInstance(resultValue, targetUnit);
    }

    // --------------------------
    // Factory Method
    // --------------------------
    private Weight CreateInstance(double value, Unit unit)
    {
        if (unit == Unit.Kilogram) return new Kilogram(value);
        if (unit == Unit.Gram) return new Gram(value);
        if (unit == Unit.Pound) return new Pound(value);

        throw new InvalidOperationException("Unsupported weight unit.");
    }
}