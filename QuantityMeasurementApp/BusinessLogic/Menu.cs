namespace QuantityMeasurement.Menu;

using QuantityMeasurement.Interfaces;
using QuantityMeasurement.Units;
using QuantityMeasurement.Core;

public class MenuClass : IMenu
{
    public void StartMenu()
    {
        bool running = true;

        while (running)
        {
           
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=== Quantity Measurement System ===");
            Console.ResetColor();

            Console.WriteLine("1. Length");
            Console.WriteLine("2. Weight");
            Console.WriteLine("3. Volume");
            Console.WriteLine("0. Exit");

            string mainChoice = Console.ReadLine() ?? "";

            switch (mainChoice)
            {
                case "1":
                    StartCategoryMenu("Length");
                    break;

                case "2":
                    StartCategoryMenu("Weight");
                    break;

                case "3":
                    StartCategoryMenu("Volume");
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    ShowError("Invalid option!");
                    break;
            }
        }
    }

    // --------------------------
    // Generic Category Menu
    // --------------------------

    private void StartCategoryMenu(string category)
    {
        Console.Clear();
        Console.WriteLine($"--- {category.ToUpper()} OPERATIONS ---");

        Console.WriteLine("1. Compare");
        Console.WriteLine("2. Convert");
        Console.WriteLine("3. Add");
        Console.WriteLine("4. Subtract");
        Console.WriteLine("5. Divide");
        Console.WriteLine("0. Back");

        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                HandleComparison(category);
                break;

            case "2":
                HandleConversion(category);
                break;

            case "3":
                HandleAddition(category);
                break;
            
            case "4": 
                HandleSubtraction(category); 
                break;
            
            case "5": 
                HandleDivision(category); 
                break;
            
            default:
                    ShowError("Invalid option!");
                    break;
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    // --------------------------
    // Comparison
    // --------------------------

    private void HandleComparison(string category)
    {
        var q1 = ReadQuantity(category, "First");
        var q2 = ReadQuantity(category, "Second");

        bool result = q1.Equals(q2);

        Console.ForegroundColor = result ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"\nResult: {q1} == {q2} → {result}");
        Console.ResetColor();
    }

    // --------------------------
    // Conversion
    // --------------------------

    private void HandleConversion(string category)
    {
        var original = ReadQuantity(category, "Source");

        Console.WriteLine("\nConvert To:");
        ShowUnits(category);

        string choice = Console.ReadLine() ?? "";
        Unit targetUnit = GetUnit(category, choice);

        var converted = original.ConvertTo(targetUnit);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nConverted: {converted}");
        Console.ResetColor();
    }

    // --------------------------
    // Addition
    // --------------------------

    private void HandleAddition(string category)
    {
        var q1 = ReadQuantity(category, "First");
        var q2 = ReadQuantity(category, "Second");

        Console.WriteLine("\n1. Add (result in first unit)");
        Console.WriteLine("2. Add and choose target unit");

        string mode = Console.ReadLine() ?? "";

        Quantity result;

        if (mode == "1")
        {
            result = q1.Add(q2);
        }
        else if (mode == "2")
        {
            Console.WriteLine("\nChoose Target Unit:");
            ShowUnits(category);
            string choice = Console.ReadLine() ?? "";
            Unit targetUnit = GetUnit(category, choice);
            result = q1.Add(q2, targetUnit);
        }
        else
        {
            ShowError("Invalid mode.");
            return;
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nResult: {q1} + {q2} = {result}");
        Console.ResetColor();
    }

    // --------------------------
    // Subtraction
    // --------------------------
    private void HandleSubtraction(string category)
    {
        var q1 = ReadQuantity(category, "First");
        var q2 = ReadQuantity(category, "Second");

        Quantity result = ChooseTargetMode(category, q1, q2, "Subtract");

        Console.WriteLine($"\nResult: {q1} - {q2} = {result}");
    }

    // --------------------------
    // Division
    // --------------------------
    private void HandleDivision(string category)
    {
        var q1 = ReadQuantity(category, "First");

        Console.WriteLine("\n1. Divide by another quantity");
        Console.WriteLine("2. Divide by scalar");
        string mode = Console.ReadLine() ?? "";

        if (mode == "1")
        {
            var q2 = ReadQuantity(category, "Second");
            double result = q1.Divide(q2);

            Console.WriteLine($"\nResult: {q1} / {q2} = {result}");
        }
        else if (mode == "2")
        {
            Console.WriteLine("Enter scalar:");
            double scalar = double.Parse(Console.ReadLine() ?? "1");

            var result = q1.Divide(scalar);
            Console.WriteLine($"\nResult: {q1} / {scalar} = {result}");
        }
        else
        {
            ShowError("Invalid mode.");
        }
    }

    // --------------------------
    // Helpers
    // --------------------------

    private Quantity ReadQuantity(string category, string label)
    {
        Console.WriteLine($"\nChoose Unit for {label} value:");
        ShowUnits(category);
        string unitChoice = Console.ReadLine() ?? "";

        Console.WriteLine("Enter value:");
        double value = double.Parse(Console.ReadLine() ?? "0");

        Unit unit = GetUnit(category, unitChoice);

        return CreateQuantity(value, unit);
    }

    private void ShowUnits(string category)
    {
        int index = 1;

        foreach (var unit in GetUnitsByCategory(category))
        {
            Console.WriteLine($"{index}. {unit}");
            index++;
        }
    }

    private Unit GetUnit(string category, string choice)
    {
        var units = GetUnitsByCategory(category);

        int index = int.Parse(choice) - 1;

        if (index < 0 || index >= units.Count)
            throw new Exception("Invalid unit selection.");

        return units[index];
    }

    private List<Unit> GetUnitsByCategory(string category)
    {
        return category switch
        {
            "Length" => new List<Unit> { Unit.Feet, Unit.Inch, Unit.Yard, Unit.Meter, Unit.Centimeter },
            "Weight" => new List<Unit> { Unit.Kilogram, Unit.Gram, Unit.Pound },
            "Volume" => new List<Unit> { Unit.Litre, Unit.Millilitre, Unit.Gallon },
            _ => throw new Exception("Invalid category")
        };
    }

    private Quantity CreateQuantity(double value, Unit unit)
    {
        return unit.Category switch
        {
            "Length" => new Feet(0).ConvertTo(unit).CreateInstance(value, unit),
            "Weight" => new Kilogram(0).ConvertTo(unit).CreateInstance(value, unit),
            "Volume" => new Litre(0).ConvertTo(unit).CreateInstance(value, unit),
            _ => throw new Exception("Invalid category")
        };
    }

    private void ShowError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }


    // ===============================
    // TARGET MODE (ADD/SUBTRACT)
    // ===============================

    private Quantity ChooseTargetMode(string category, Quantity q1, Quantity q2, string operation)
    {
        Console.WriteLine("\n1. Result in first unit");
        Console.WriteLine("2. Choose target unit");

        string mode = Console.ReadLine() ?? "";

        if (mode == "1")
        {
            return operation == "Add"
                ? q1.Add(q2)
                : q1.Subtract(q2);
        }
        else if (mode == "2")
        {
            Console.WriteLine("\nChoose Target Unit:");
            ShowUnits(category);

            Unit targetUnit = GetUnit(category, Console.ReadLine() ?? "");

            return operation == "Add"
                ? q1.Add(q2, targetUnit)
                : q1.Subtract(q2, targetUnit);
        }

        throw new Exception("Invalid mode selected.");
    }



}