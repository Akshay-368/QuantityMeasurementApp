namespace QuantityMeasurement.Core;
using System;
/// <summary>
/// This is a class for the unit of any quantity that may arise in future .
/// It requires a name to be set, which must be a string.
/// </summary>
public sealed class Unit
{
    private const string LENGTH = "Length";
    private const string WEIGHT = "Weight";
    public static readonly Unit Feet = new Unit ("Feet" , 1.0 , LENGTH ); // Treating it as a base as per the suggestion of the UC-3
    public static readonly Unit Inch  = new Unit("Inch", 1.0 / 12.0 , LENGTH);
    public static readonly Unit Yard  = new Unit("Yard", 3.0 , LENGTH);
    public static readonly Unit Meter = new Unit("Meter", 3.28084 , LENGTH);
    public static readonly Unit Centimeter = new Unit("Centimeter", 0.0328084 , LENGTH); // Added for the UC-4

    // ------------------Weight --------------------------
    public static readonly Unit Kilogram = new Unit("Kilogram", 1.0 , WEIGHT );

    public static readonly Unit Gram = new Unit("Gram", 0.001, WEIGHT);

    public static readonly Unit Pound =  new Unit("Pound", 0.453592 , WEIGHT); 

    public string Name { get; }
    // converion factor to base unit ( eg, feet )
    public double ConversionFactorToBase { get; }

    public string Category { get; }

    
    private Unit(string name, double conversionFactorToBase , string category)
    {
        Name = name;
        ConversionFactorToBase = conversionFactorToBase;
        Category = category;
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

