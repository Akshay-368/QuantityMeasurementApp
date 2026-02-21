namespace QuantityMeasurement.Menu;
using QuantityMeasurement.Interfaces;
using QuantityMeasurement.Units;
// Changed the name of Menu class to MenuClass as it was clashing with the name of the namespace
public class MenuClass : IMenu
{
    public  void StartMenu()
    {
        bool running = true;
        while (running)
        {

            
            //  Welcome Message
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Quantity Measurement System (Length Testing as per the UC3) ");
            Console.ResetColor();

            // Options
            Console.WriteLine("Choose Unit for First Value ( please ):");
            Console.WriteLine("1. Feet ");
            Console.WriteLine("2. Inches" );
            Console.WriteLine("3. Yard ");
            Console.WriteLine("4. Meter" );
            Console.WriteLine("5. Centimeter ");
            Console.WriteLine("0. Exit ");
                
            string unit1Choice = Console.ReadLine() ?? "";
            if (unit1Choice == "0") break;

            try
            {
                Console.WriteLine("Enter first value:");
                double v1 = double.Parse(Console.ReadLine() ?? "0");

                Console.WriteLine(" Choose an Unit for the Second Value ( please ):");
                Console.WriteLine("1. Feet ");
                Console.WriteLine("2. Inches ");
                Console.WriteLine("3. Yard ");
                Console.WriteLine("4. Meter ");
                Console.WriteLine("5. Centimeter ");

                string unit2Choice = Console.ReadLine() ?? "";

                Console.WriteLine("Enter second value:");
                double v2 = double.Parse(Console.ReadLine() ?? "0");

                Length length1 = CreateLength(v1, unit1Choice);
                Length length2 = CreateLength(v2, unit2Choice);

                bool result = length1.Equals(length2);

                ShowResult($"{v1} {length1.unit} == {v2} {length2.unit}", result);
            }
            catch (Exception)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Error: Invalid input!");
                    Console.ResetColor();
                }

        

            Console.WriteLine(" Press any key to return to menu ...");
            Console.ReadKey();
        }
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
}