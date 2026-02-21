/*
The win form was made with 
dotnet new winforms -n QuantityMeasurement.WinUI from the path of (C:\Users\aksha\OneDrive\Documents\QuantityMeasurementApp>)

And then 
dotnet sln BusinessLogic/QuantityMeasurementApp.sln add QuantityMeasurement.WinUI/QuantityMeasurement.WinUI.csproj  ( from the path : C:\Users\aksha\OneDrive\Documents\QuantityMeasurementApp>)
dotnet add QuantityMeasurement.WinUI/QuantityMeasurement.WinUI.csproj reference BusinessLogic/QuantityMeasurementApp.csproj

and for runing
 using 
for building the whole solution :
dotnet build BusinessLogic/QuantityMeasurementApp.sln
for running the UI :
dotnet run --project QuantityMeasurement.WinUI/QuantityMeasurement.WinUI.csproj

for testing :
Using Nunit
# 1). Creating a new NUnit project
dotnet new nunit -n QuantityMeasurement.Tests

# 2). Adding it to the Solution
dotnet sln BusinessLogic/QuantityMeasurementApp.sln add QuantityMeasurement.Tests/QuantityMeasurement.Tests.csproj

# 3). Reference the Business Logic project
dotnet add QuantityMeasurement.Tests/QuantityMeasurement.Tests.csproj reference BusinessLogic/QuantityMeasurementApp.csproj

#4). For actualy running the test cases , I use 
dotnet test BusinessLogic/QuantityMeasurementApp.sln
*/

using QuantityMeasurement.Core;
using QuantityMeasurement.Units;
namespace QuantityMeasurement.EntryPoint;

class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine(" Quantity Measurement Console Mode ");
        Console.Write("Enter first value (Feet): ");
        double val1 = double.Parse(Console.ReadLine() ?? "0");

        Console.Write("Enter second value (Feet): ");
        double val2 = double.Parse(Console.ReadLine() ?? "0");

        Feet f1 = new Feet(val1);
        Feet f2 = new Feet(val2);

        Console.WriteLine($"Result: {f1.Equals(f2)}");
    }
}