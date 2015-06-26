using System;
using System.Collections.Generic;

namespace FastRT
{
    public interface IObjectCache<in TKey>
    {
        T GetValue<T>(TKey key, Func<T> objectFactory);
    }

    public class EmptyObjectCache<TKey> : IObjectCache<TKey>
    {
        public T GetValue<T>(TKey key, Func<T> objectFactory)
        {
            return objectFactory();
        }

        static readonly EmptyObjectCache<TKey> s_instance = new EmptyObjectCache<TKey>();
        public static IObjectCache<TKey> Instance
        {
            get { return s_instance; }
        }
    }

    public class ObjectCache<TKey> : IObjectCache<TKey>
    {
        private readonly Dictionary<TKey, object> _dictionary;

        public ObjectCache(IEqualityComparer<TKey> keyComparer = null)
        {
            _dictionary = new Dictionary<TKey, object>(keyComparer ?? EqualityComparer<TKey>.Default);
        }

        public T GetValue<T>(TKey key, Func<T> objectFactory)
        {
            object result;
            if (!_dictionary.TryGetValue(key, out result))
            {
                lock (_dictionary)
                {
                    if (!_dictionary.TryGetValue(key, out result))
                    {
                        result = objectFactory();
                        _dictionary.Add(key, result);
                    }
                }
            }
            return (T)result;
        }
    }
}