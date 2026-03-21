namespace QuantityMeasurement.EntryPoint;
using QuantityMeasurement.Domain.Core;
using QuantityMeasurement.Domain.Units;
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
