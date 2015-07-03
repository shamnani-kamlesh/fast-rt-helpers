using System;

namespace FastRT
{
    public class FuncMemberAccessor<TObject, TMember> : IMemberAccessor<TObject, TMember>
    {
        private readonly Func<TObject, TMember> _readValue;
        private readonly Action<TObject, TMember> _writeValue;
        private readonly bool _throwIfNotDefined;

        public FuncMemberAccessor(Func<TObject, TMember> readValue, Action<TObject, TMember> writeValue = null, bool throwIfNotDefined = false)
        {
            _readValue = readValue;
            _writeValue = writeValue;
            _throwIfNotDefined = throwIfNotDefined;
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
            if (HasGetter)
                return _readValue(instance);
            if(_throwIfNotDefined)
                throw new InvalidOperationException(
                    String.Format("Member: '{0}' of Type: '{1}' does not have a Get accessor", MemberName, ObjectType.Name));
            return default(TMember);
        }

        public void SetValue(TObject instance, TMember value)
        {
            if (HasSetter)
                _writeValue(instance, value);
            else if (_throwIfNotDefined)
                throw new InvalidOperationException(
                    String.Format("Member: '{0}' of Type: '{1}' does not have a Get accessor",MemberName, ObjectType.Name));
        }

        public override string ToString()
        {
            return (_readValue ?? (object)_writeValue).ToString();
        }

        public virtual string MemberName 
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