namespace QuantityMeasurement.Menu;
using QuantityMeasurement.Interfaces;
using QuantityMeasurement.Units;
using QuantityMeasurement.Core;
// I Changed the name of Menu class to MenuClass as it was clashing with the name of the namespace
public class MenuClass : IMenu
{
    public  void StartMenu()
    {
        bool running = true;
        while (running)
        {

            
            //  Welcome Message
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Quantity Measurement System (Length Testing as per the UC5) ");
            Console.ResetColor();

            Console.WriteLine("\nSelect Quantity Type:");
            Console.WriteLine("1. Length");
            Console.WriteLine("2. Weight");
            Console.WriteLine("0. Exit");

            string mainChoice = Console.ReadLine() ?? "";

            switch (mainChoice)
            {
                case "1":
                    StartLengthMenu();
                    break;

                case "2":
                    StartWeightMenu();
                    break;

                case "0":
                    running = false;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Invalid option selected!");
                    Console.ResetColor();
                    break;
            }
            if (running)
            {
                Console.WriteLine("\nPress any key to return to menu...");
                Console.ReadKey();
            }

            
        }


    }
    private void StartLengthMenu()
    {
        Console.WriteLine("\n--- LENGTH OPERATIONS ---");
        Console.WriteLine("1. Compare Lengths");
        Console.WriteLine("2. Convert Length");
        Console.WriteLine("3. Add Lengths");
        Console.WriteLine("0. Back");

        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                HandleLengthComparison();
                break;
            case "2":
                HandleLengthConversion();
                break;
            case "3":
                HandleLengthAddition();
                break;
        }
    }

    private void StartWeightMenu()
    {
        Console.WriteLine("\n--- WEIGHT OPERATIONS ---");
        Console.WriteLine("1. Compare Weights");
        Console.WriteLine("2. Convert Weight");
        Console.WriteLine("3. Add Weights");
        Console.WriteLine("0. Back");

        string choice = Console.ReadLine() ?? "";

        switch (choice)
        {
            case "1":
                HandleWeightComparison();
                break;
            case "2":
                HandleWeightConversion();
                break;
            case "3":
                HandleWeightAddition();
                break;
        }
    }

    private static void ShowWeightOptions()
    {
        Console.WriteLine("1. Kilogram");
        Console.WriteLine("2. Gram");
        Console.WriteLine("3. Pound");
    }

    private static Weight CreateWeight(double value, string choice)
    {
        return choice switch
        {
            "1" => new Kilogram(value),
            "2" => new Gram(value),
            "3" => new Pound(value),
            _ => throw new Exception("Invalid Weight Unit")
        };
    }

    private void HandleWeightComparison()
    {
        Console.WriteLine("\nChoose Unit for First Value:");
        ShowWeightOptions();
        string unit1Choice = Console.ReadLine() ?? "";

        Console.WriteLine("Enter first value:");
        double value1 = double.Parse(Console.ReadLine() ?? "0");

        Console.WriteLine("\nChoose Unit for Second Value:");
        ShowWeightOptions();
        string unit2Choice = Console.ReadLine() ?? "";

        Console.WriteLine("Enter second value:");
        double value2 = double.Parse(Console.ReadLine() ?? "0");

        Weight w1 = CreateWeight(value1, unit1Choice);
        Weight w2 = CreateWeight(value2, unit2Choice);

        bool result = w1.Equals(w2);

        ShowResult($"{w1} == {w2}", result);
    }

    private void HandleWeightConversion()
    {
        Console.WriteLine("Choose Unit:");
        ShowWeightOptions();

        string fromChoice = Console.ReadLine() ?? "";

        Console.WriteLine("Enter value:");
        double value = double.Parse(Console.ReadLine() ?? "0");

        Console.WriteLine("Convert To:");
        ShowWeightOptions();

        string toChoice = Console.ReadLine() ?? "";

        Weight original = CreateWeight(value, fromChoice);
        Unit targetUnit = GetWeightUnitFromChoice(toChoice);

        Weight converted = original.ConvertTo(targetUnit);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nConverted Value: {converted}");
        Console.ResetColor();
    }

    private static Unit GetWeightUnitFromChoice(string choice)
    {
        return choice switch
        {
            "1" => Unit.Kilogram,
            "2" => Unit.Gram,
            "3" => Unit.Pound,
            _ => throw new Exception("Invalid Unit")
        };
    }

    private void HandleWeightAddition()
    {
        Console.WriteLine("\nChoose Unit for First Value:");
        ShowWeightOptions();
        string unit1Choice = Console.ReadLine() ?? "";

        Console.WriteLine("Enter first value:");
        double value1 = double.Parse(Console.ReadLine() ?? "0");

        Console.WriteLine("\nChoose Unit for Second Value:");
        ShowWeightOptions();
        string unit2Choice = Console.ReadLine() ?? "";

        Console.WriteLine("Enter second value:");
        double value2 = double.Parse(Console.ReadLine() ?? "0");

        Weight w1 = CreateWeight(value1, unit1Choice);
        Weight w2 = CreateWeight(value2, unit2Choice);

        Console.WriteLine("\nSelect Addition Mode:");
        Console.WriteLine("1. Add (Result in First Unit)");
        Console.WriteLine("2. Add and Choose Target Unit");

        string modeChoice = Console.ReadLine() ?? "";

        Weight result;

        if (modeChoice == "1")
        {
            result = w1.Add(w2);
        }
        else if (modeChoice == "2")
        {
            Console.WriteLine("\nChoose Target Unit:");
            ShowWeightOptions();
            string targetChoice = Console.ReadLine() ?? "";

            Unit targetUnit = GetWeightUnitFromChoice(targetChoice);

            result = w1.Add(w2, targetUnit);
        }
        else
        {
            throw new Exception("Invalid Addition Mode");
        }

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nResult: {w1} + {w2} = {result}");
        Console.ResetColor();
    }

    private void HandleLengthComparison()
    {
        try
        {
            Console.WriteLine("\nChoose Unit for First Value:");
            ShowUnitOptions();
            string unit1Choice = Console.ReadLine() ?? "";

            Console.WriteLine("Enter first value:");
            double value1 = double.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("\nChoose Unit for Second Value:");
            ShowUnitOptions();
            string unit2Choice = Console.ReadLine() ?? "";

            Console.WriteLine("Enter second value:");
            double value2 = double.Parse(Console.ReadLine() ?? "0");

            Length length1 = CreateLength(value1, unit1Choice);
            Length length2 = CreateLength(value2, unit2Choice);

            bool result = length1.Equals(length2);

            ShowResult($"{value1} {length1.unit} == {value2} {length2.unit}", result);
        }
        catch (Exception)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Error: Invalid input!");
            Console.ResetColor();
        }
    }

    private static void ShowUnitOptions()
    {
        Console.WriteLine("1. Feet");
        Console.WriteLine("2. Inches");
        Console.WriteLine("3. Yard");
        Console.WriteLine("4. Meter");
        Console.WriteLine("5. Centimeter");
    }


    private static void ShowResult(string comparison, bool isTrue)
    {
        Console.ForegroundColor = isTrue ? ConsoleColor.Green : ConsoleColor.Red;
        Console.WriteLine($"\nResult: {comparison} is {isTrue.ToString().ToUpper()}");
        Console.ResetColor();
    }

        /// <summary>
        /// Creates a new Length object based on the value and unit choice
        /// </summary>
        /// <param name="value">The value of the length</param>
        /// <param name="unitChoice">The unit of the length (1 for Feet, 2 for Inches, 3 for Yard, 4 for Meter)</param>
        /// <returns>A new Length object</returns>
        /// <exception cref="Exception">Thrown if the unit choice is invalid</exception>
    private static Length CreateLength(double value, string unitChoice)
    {
        return unitChoice switch
        {
            "1" => new Feet(value),
            "2" => new Inches(value),
            "3" => new Yard(value),
            "4" => new Meter(value),
            "5" => new Centimeter(value),
            _ => throw new Exception("Invalid Unit")
        };
    }

    private void HandleLengthConversion()
    {
        Console.WriteLine("Choose Unit:");
        ShowUnitOptions();

        string fromChoice = Console.ReadLine() ?? "";

        Console.WriteLine("Enter value:");
        double value = double.Parse(Console.ReadLine() ?? "0");

        Console.WriteLine("Convert To:");
        ShowUnitOptions();

        string toChoice = Console.ReadLine() ?? "";

        Length original = CreateLength(value, fromChoice);
        Unit targetUnit = GetUnitFromChoice(toChoice);

        Length converted = original.ConvertTo(targetUnit);

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"\nConverted Value: {converted}");
        Console.ResetColor();
    }

    private static Unit GetUnitFromChoice(string choice)
    {
        return choice switch
        {
            "1" => Unit.Feet,
            "2" => Unit.Inch,
            "3" => Unit.Yard,
            "4" => Unit.Meter,
            "5" => Unit.Centimeter,
            _ => throw new Exception("Invalid Unit")
        };
    }

    // For UC-6

    /* private void HandleAddition()
    {
        try
        {
            Console.WriteLine("\nChoose Unit for First Value:");
            ShowUnitOptions();
            string unit1Choice = Console.ReadLine() ?? "";

            Console.WriteLine("Enter first value:");
            double value1 = double.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("\nChoose Unit for Second Value:");
            ShowUnitOptions();
            string unit2Choice = Console.ReadLine() ?? "";

            Console.WriteLine("Enter second value:");
            double value2 = double.Parse(Console.ReadLine() ?? "0");

            Length length1 = CreateLength(value1, unit1Choice);
            Length length2 = CreateLength(value2, unit2Choice);

            Length result = length1.Add(length2);

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nResult: {length1} + {length2} = {result}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    } */


    // for uc- 6 and uc-7 
    private void HandleLengthAddition()
    {
        try
        {
            Console.WriteLine("\nChoose Unit for First Value:");
            ShowUnitOptions();
            string unit1Choice = Console.ReadLine() ?? "";

            Console.WriteLine("Enter first value:");
            double value1 = double.Parse(Console.ReadLine() ?? "0");

            Console.WriteLine("\nChoose Unit for Second Value:");
            ShowUnitOptions();
            string unit2Choice = Console.ReadLine() ?? "";

            Console.WriteLine("Enter second value:");
            double value2 = double.Parse(Console.ReadLine() ?? "0");

            Length length1 = CreateLength(value1, unit1Choice);
            Length length2 = CreateLength(value2, unit2Choice);

            Console.WriteLine("\nSelect Addition Mode:");
            Console.WriteLine("1. Add (Result in First Unit)  [UC6]");
            Console.WriteLine("2. Add and Choose Target Unit [UC7]");

            string modeChoice = Console.ReadLine() ?? "";

            Length result;

            if (modeChoice == "1")
            {
                result = length1.Add(length2);
            }
            else if (modeChoice == "2")
            {
                Console.WriteLine("\nChoose Target Unit:");
                ShowUnitOptions();
                string targetChoice = Console.ReadLine() ?? "";

                Unit targetUnit = GetUnitFromChoice(targetChoice);

                result = length1.Add(length2, targetUnit);
            }
            else
            {
                throw new Exception("Invalid Addition Mode");
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"\nResult: {length1} + {length2} = {result}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }

}