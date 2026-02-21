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
            Console.WriteLine("Welcome to Quantity Measurement System (UC- 1 & 2) ");
            Console.ResetColor();

            // Options
            Console.WriteLine("1. Compare Feet");
            Console.WriteLine("2. Compare Inches");
            Console.WriteLine("0. Exit");
            Console.WriteLine("Select Choice: ");
            
            string choice = Console.ReadLine() ?? "";

            if (choice == "0") { running = false; continue; }

            //  Logic & Input
            try
            {
                Console.WriteLine("Enter first value: ");
                double v1 = double.Parse(Console.ReadLine() ?? "0");

                Console.WriteLine("Enter second value: ");
                double v2 = double.Parse(Console.ReadLine() ?? "0");

                bool result = false;

                if (choice == "1")
                {
                    result = new Feet(v1).Equals(new Feet(v2));
                    ShowResult($"{v1} ft == {v2} ft", result);
                }
                else if (choice == "2")
                {
                    result = new Inches(v1).Equals(new Inches(v2));
                    ShowResult($"{v1} in == {v2} in", result);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Invalid Selection!");
                    Console.ResetColor();
                }
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Error : Please enter numbers only ! ");
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
}