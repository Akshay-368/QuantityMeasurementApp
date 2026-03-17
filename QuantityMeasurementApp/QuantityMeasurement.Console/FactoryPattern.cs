// My Factory Pattern implementation
namespace QuantityMeasurement.FactoryPattern;
static class Factory<K, T> where T : class, K, new()
{
    public static K CreateObject()
    {
        K obj = new T();
        return obj;
    }
}