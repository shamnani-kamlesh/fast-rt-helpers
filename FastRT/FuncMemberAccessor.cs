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

        public bool HasGetter => _readValue != null;

        public bool HasSetter => _writeValue != null;

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
                    $"Member: '{MemberName}' of Type: '{ObjectType.Name}' does not have a Get accessor");
            return default(TMember);
        }

        public void SetValue(TObject instance, TMember value)
        {
            if (HasSetter)
                _writeValue(instance, value);
            else if (_throwIfNotDefined)
                throw new InvalidOperationException(
                    $"Member: '{MemberName}' of Type: '{ObjectType.Name}' does not have a Get accessor");
        }

        public override string ToString()
        {
            return (_readValue ?? (object)_writeValue).ToString();
        }

        public virtual string MemberName => "F<" + ObjectType.Name + "," + MemberType.Name + ">";

        public Type MemberType => typeof (TMember);

        public Type ObjectType => typeof (TObject);
    }
}