using System.Collections.Generic;
using System.Reflection;

namespace FastRT
{
    public class MemberValueAccessor : IMemberValueAccessor
    {
        private readonly Dictionary<string, IMemberAccessor> _accessors = new Dictionary<string, IMemberAccessor>();

        public MemberValueAccessor(IEnumerable<PropertyInfo> properties)
        {
            foreach(var prop in properties)
            {
                var ma = RTHelper.GetMemberAccessor(prop);
                _accessors[prop.Name] = ma;
            }
        }

        public MemberValueAccessor(IEnumerable<MemberInfo> properties)
        {
            foreach (var prop in properties)
            {
                var ma = RTHelper.GetMemberAccessor(prop);
                _accessors[prop.Name] = ma;
            }
        }

        public MemberValueAccessor(IEnumerable<FieldInfo> properties)
        {
            foreach (var prop in properties)
            {
                var ma = RTHelper.GetMemberAccessor(prop);
                _accessors[prop.Name] = ma;
            }
        }

        public object GetValue(string memberName, object instance)
        {
            return _accessors[memberName].GetValue(instance);
        }

        public void SetValue(string memberName, object instance, object value)
        {
            _accessors[memberName].SetValue(instance, value);
        }
    }
}