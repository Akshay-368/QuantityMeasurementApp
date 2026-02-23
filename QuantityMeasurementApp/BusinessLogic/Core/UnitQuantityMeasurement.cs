namespace QuantityMeasurement.Core;
using System;
/// <summary>
/// This is a class for the unit of any quantity that may arise in future .
/// It requires a name to be set, which must be a string.
/// </summary>
public sealed class Unit
{
    public static readonly Unit Feet = new Unit ("Feet" , 1.0 ); // Treating it as a base as per the suggestion of the UC-3
    public static readonly Unit Inch  = new Unit("Inch", 1.0 / 12.0);
    public static readonly Unit Yard  = new Unit("Yard", 3.0);
    public static readonly Unit Meter = new Unit("Meter", 3.28084);
    public static readonly Unit Centimeter = new Unit("Centimeter", 0.0328084); // Added for the UC-4

    public string Name { get; }
    // converion factor to base unit ( eg, feet )
    public double ConversionFactorToBase { get; }
    private Unit(string name, double conversionFactorToBase)
    {
        Name = name;
        ConversionFactorToBase = conversionFactorToBase;
    }

    public override string ToString()
    {
        return Name;
    }

    // Because of UC-8 and to avoid any circular dependencies
    public double ConvertToBaseUnit(double value)
    {
        return value * ConversionFactorToBase;
    }

    public double ConvertFromBaseUnit(double baseValue)
    {
        return baseValue / ConversionFactorToBase;
    }



    
}