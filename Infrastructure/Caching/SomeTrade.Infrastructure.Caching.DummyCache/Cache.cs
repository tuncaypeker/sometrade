namespace SomeTrade.Infrastructure.Caching.DummyCache
{
    using SomeTrade.Infrastructure.Interfaces;
    using System.Collections.Generic;

    public class Cache : ICache
    {
        private Dictionary<string, object> _cacheDictionary;

        public Cache()
        {
            _cacheDictionary = new Dictionary<string, object>();
        }

        public bool TryGetValue(string key, out object value)
        {
            if (!_cacheDictionary.ContainsKey(key))
            {
                value = null;
                return false;
            }

            value = _cacheDictionary[key];

            return true;
        }

        public void Set(string key, object value, int minutesToCache)
        {
            if (!_cacheDictionary.ContainsKey(key))
            {
                _cacheDictionary[key] = value;

                return;
            }

            _cacheDictionary.Add(key, value);
        }

        public void Remove(string key)
        {
            _cacheDictionary.Remove(key);
        }
    }
}