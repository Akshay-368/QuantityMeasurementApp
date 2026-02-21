namespace QuantityMeasurement.Core;
public abstract class Quantity : IQuantity<Quantity>
{
    // Implementing the IQuantity interface properties here
    public double value { get; }
    public Unit unit { get; }
    protected Quantity (double value, Unit unit)
    {
        this.value = value;
        this.unit = unit;
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