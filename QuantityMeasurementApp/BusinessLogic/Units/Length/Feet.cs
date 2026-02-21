namespace QuantityMeasurement.Units; // for concrete units

using QuantityMeasurement.Core; // Importing the base logic from the Core.

/// <summary>
/// This is a class for the Feet unit.
/// This has been kept simple as per UC1
/// </summary>
// It is a concrete unit and kept public because of the fact that later maybe different projects like ASP.NET Core might need to access it and in that case it will be a headache if i keep it internal.
public class Feet : Quantity
{
    // Keeping it simple as per UC1
    public Feet(double value) : base(value, Unit.Feet)
    {
    }
}