namespace SomeTrade.Infrastructure.Interfaces
{
    public interface ICache
    {
        bool TryGetValue(string key, out object value);
        void Set(string key, object value, int minutesToCache);
        void Remove(string key);
    }
}
