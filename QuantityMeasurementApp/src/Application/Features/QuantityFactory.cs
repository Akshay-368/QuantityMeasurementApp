using System.Collections.Generic;
using QuantityMeasurement.Domain.Core;  // For Unit
using QuantityMeasurement.Domain.Units;  // For concretes (Feet, etc.)

namespace QuantityMeasurement.Application;

public static class QuantityFactory
{
    public static List<Unit> GetUnitsByCategory(string category)
    {
        return category switch
        {
            "Length" => new List<Unit> { Unit.Feet, Unit.Inch, Unit.Yard, Unit.Meter, Unit.Centimeter },
            "Weight" => new List<Unit> { Unit.Kilogram, Unit.Gram, Unit.Pound },
            "Volume" => new List<Unit> { Unit.Litre, Unit.Millilitre, Unit.Gallon },
            "Temperature" => new List<Unit> { Unit.Celsius, Unit.Fahrenheit, Unit.Kelvin },
            _ => throw new ArgumentException("Invalid category", nameof(category))
        };
    }

    public static Quantity CreateQuantity(double value, Unit unit)
    {
        return unit.Category switch
        {
            "Length" => new Feet(0).ConvertTo(unit).CreateInstance(value, unit),
            "Weight" => new Kilogram(0).ConvertTo(unit).CreateInstance(value, unit),
            "Volume" => new Litre(0).ConvertTo(unit).CreateInstance(value, unit),
            "Temperature" => new Celsius(0).ConvertTo(unit).CreateInstance(value, unit),
            _ => throw new ArgumentException("Invalid category", nameof(unit.Category))
        };
    }

    public static Unit GetUnitByIndex(string category, int index)
    {
        var units = GetUnitsByCategory(category);
        if (index < 0 || index >= units.Count)
            throw new ArgumentOutOfRangeException(nameof(index), "Invalid unit selection.");
        return units[index];
    }

    // Bonus: For API (string-based, like Menu's GetUnit but by name)
    public static Unit GetUnitByName(string category, string unitName)
    {
        var units = GetUnitsByCategory(category);
        var unit = units.FirstOrDefault(u => u.Name.Equals(unitName, StringComparison.OrdinalIgnoreCase));
        return unit ?? throw new ArgumentException($"Unit '{unitName}' not found in category '{category}'.");
    }
}