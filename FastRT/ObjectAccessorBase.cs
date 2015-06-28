using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FastRT.Impl;
using System.Reflection;

namespace FastRT
{
    /// <summary>
    /// Provides access to type members (properties and fields) using index or member name
    /// </summary>
    public interface IObjectAccessor
    {
        V GetValue<V>(int idx);
        void SetValue<V>(int idx, V value);
        V GetValue<V>(string name);
        void SetValue<V>(string name, V value);
        
        object this[int idx] { get; set; }
        object this[string name] { get; set; }
    }

    /// <summary>
    /// Base class that supports simple and efficient implementation of IObjectAccessor interface
    /// Derived class looks like this:
    /// class ClassA : ObjectAccessorBase[ClassA] {...}
    /// All public properties and fields that are declared in ClassA, will be available via IObjectAccessor
    /// </summary>
    /// <typeparam name="T">Generic parameter must be a type that is derived from ObjectAccessorBase</typeparam>
    public abstract class ObjectAccessorBase<T> : IObjectAccessor where T : ObjectAccessorBase<T>
    {
        //correspondence between member names and indexes
        private static Dictionary<string, int> s_nameIdx;

        private static Func<T, object>[] s_getters;
        private static Action<T, object>[] s_setters;

        static ObjectAccessorBase()
        {
            InitPropertyAccessors();
        }

        private static void InitPropertyAccessors()
        {
            //select public properties and fields declared for the derived type (T), in order of their declaration
            var members = (from member in typeof(T).GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.DeclaredOnly)
                            where member is PropertyInfo || member is FieldInfo
                            let order = member.GetCustomAttributes(typeof(OrderAttribute), false).Cast<OrderAttribute>().SingleOrDefault()
                            orderby order == null ? 0 : order.Order
                            select member).ToArray();

            s_getters = new Func<T, object>[members.Length];
            s_setters = new Action<T, object>[members.Length];
            s_nameIdx = new Dictionary<string, int>();

            for (int i = 0; i < members.Length; i++)
            {
                s_getters[i] = RuntimeDelegateFactory.RetrieveMemberGetValueDelegate<T, object>(members[i].Name, EmptyObjectCache<string>.Instance);
                s_setters[i] = RuntimeDelegateFactory.RetrieveMemberSetValueDelegate<T, object>(members[i].Name, EmptyObjectCache<string>.Instance);
                s_nameIdx.Add(members[i].Name, i);
            }
        }

        public V GetValue<V>(int idx)
        {
            return (V)this[idx];
        }

        public void SetValue<V>(int idx, V value)
        {
            this[idx] = value;
        }

        public V GetValue<V>(string name)
        {
            return (V)this[name];
        }

        public void SetValue<V>(string name, V value)
        {
            this[name] = value;
        }

        public object this[int idx]
        {
            get { return s_getters[idx]((T)this); }
            set { s_setters[idx]((T)this, value); }
        }

        public object this[string name]
        {
            get { return this[s_nameIdx[name]]; }
            set { this[s_nameIdx[name]] = value; }
        }
    }

}
