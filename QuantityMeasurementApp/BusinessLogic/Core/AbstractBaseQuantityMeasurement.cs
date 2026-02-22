namespace QuantityMeasurement.Core;
public abstract class Quantity : IQuantity<Quantity>
{
    // Implementing the IQuantity interface properties here
    public double value { get; }
    public Unit unit { get; }
    protected Quantity (double value, Unit unit)
    {
        if (double.IsNaN(value) || double.IsInfinity(value))
             throw new ArgumentException("Value must be a finite number.", nameof(value));
             
        this.value = value;
        this.unit = unit ?? throw new ArgumentNullException(nameof(unit)) ;
    }

    /// <summary>
    /// UC1 Requirement: Handles Reflexive, Null, Type, and Value equality.
    /// </summary>
    public abstract override bool Equals (object? obj);

    public override int GetHashCode()
    {
        return HashCode.Combine(value, unit);
    }
}