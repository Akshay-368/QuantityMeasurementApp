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
    public override bool Equals (object obj)
    {
        // Reference check ( reflexive)
        if (ReferenceEquals (this, obj) ) return true;

        //  Null check
        if (obj is null) return false;

        //  Type check ( this Ensures Feet is only compared to Feet)
        if (this.GetType() != obj.GetType()) return false;

        //  Value and Unit equality
        Quantity other = (Quantity)obj;
        return value.Equals(other.value) && unit.Equals(other.unit);


    }

    public override int GetHashCode()
    {
        return HashCode.Combine(value, unit);
    }
}