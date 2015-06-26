using System;

namespace FastRT
{
    public sealed class FuncMemberAccessor<TObject, TMember> : IMemberAccessor<TObject, TMember>, IMemberInfo
    {
        private readonly Func<TObject, TMember> _readValue;
        private readonly Action<TObject, TMember> _writeValue;

        public FuncMemberAccessor(Func<TObject, TMember> readValue, Action<TObject, TMember> writeValue = null)
        {
            _readValue = readValue;
            _writeValue = writeValue;
        }

        public bool HasGetter
        {
            get { return _readValue != null; }
        }

        public bool HasSetter
        {
            get { return _writeValue != null; }
        }

        object IMemberAccessor.GetValue(object instance)
        {
            return GetValue((TObject) instance);
        }

        void IMemberAccessor.SetValue(object instance, object value)
        {
            SetValue((TObject) instance, (TMember) value);            
        }

        public TMember GetValue(TObject instance)
        {
            return HasGetter ? _readValue(instance) : default(TMember);
        }

        public void SetValue(TObject instance, TMember value)
        {
            if (HasSetter)
                _writeValue(instance, value);
        }

        public override string ToString()
        {
            return (_readValue ?? (object)_writeValue).ToString();
        }

        public string MemberName 
        {
            get { return "F<" + ObjectType.Name + "," + MemberType.Name + ">"; }
        }

        public Type MemberType 
        {
            get { return typeof (TMember); }
        }

        public Type ObjectType 
        {
            get { return typeof (TObject); }
        }
    }
}