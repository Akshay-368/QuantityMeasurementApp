namespace QuantityMeasurement.Core;
using System;
/// <summary>
/// This is a class for the unit of any quantity that may arise in future .
/// It requires a name to be set, which must be a string.
/// </summary>
public class Unit
{
    public static readonly Unit Feet = new Unit ("Feet");
    public static readonly Unit Meter = new Unit ("Meter");
    public static readonly Unit Inch = new Unit ("Inch");
    public static readonly Unit Yard = new Unit ("Yard");

    public string Name { get; }
    private Unit ( string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}