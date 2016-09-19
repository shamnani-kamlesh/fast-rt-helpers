using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace FastRT
{
    public interface IObjectCache<in TKey>
    {
        T GetValue<T>(TKey key, Func<T> objectFactory);
        void Clear();
    }

    public class EmptyObjectCache<TKey> : IObjectCache<TKey>
    {
        public T GetValue<T>(TKey key, Func<T> objectFactory)
        {
            return objectFactory();
        }

        public void Clear()
        {            
        }

        private static readonly EmptyObjectCache<TKey> s_instance = new EmptyObjectCache<TKey>();
        public static IObjectCache<TKey> Instance => s_instance;
    }

    public class ObjectCache<TKey> : IObjectCache<TKey>
    {
        private readonly ConcurrentDictionary<TKey, object> _dictionary;

        public ObjectCache(IEqualityComparer<TKey> keyComparer = null)
        {
            _dictionary = new ConcurrentDictionary<TKey, object>(keyComparer ?? EqualityComparer<TKey>.Default);
        }

        /// <summary>
        /// Try to find a cached value for the provided key. 
        /// If value is not found, it is created using the objectFactory, cached and returned back
        /// </summary>
        public T GetValue<T>(TKey key, Func<T> objectFactory)
        {
            return (T)_dictionary.GetOrAdd(key, k => objectFactory());
        }

        public void Clear()
        {
            _dictionary.Clear();
        }
    }
}