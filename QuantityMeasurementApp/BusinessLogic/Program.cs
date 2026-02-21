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

namespace QuantityMeasurement.EntryPoint;
using QuantityMeasurement.Core;
using QuantityMeasurement.Units;
using QuantityMeasurement.Interfaces;
using QuantityMeasurement.FactoryPattern;
using QuantityMeasurement.Menu;

class Program
{
    public static void Main(string[] args)
    {
        // Using the Factory to create an instance of Menu as an IMenu
        IMenu menu = Factory<IMenu, MenuClass>.CreateObject(); 
        
        // Start the logic of the menu
        menu.StartMenu();
    }
}

