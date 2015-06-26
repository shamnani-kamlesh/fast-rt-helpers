using System;

namespace FastRT
{
    public sealed class ObjectMemberAccessor<TObject, TMember> : IObjectMemberAccessor<TMember> 
        where TObject : class
    {
        private readonly TObject _object;
        private readonly IMemberAccessor<TObject, TMember> _rtAccessor;

        public ObjectMemberAccessor(TObject obj, IMemberAccessor<TObject, TMember> rtAccessor)
        {
            _object = obj;
            _rtAccessor = rtAccessor;
        }

        public TMember GetValue()
        {
            return _rtAccessor.GetValue(_object);
        }

        public void SetValue(TMember value)
        {
            _rtAccessor.SetValue(_object, value);
        }

        object IObjectMemberAccessor.GetValue()
        {
            return GetValue();
        }

        void IObjectMemberAccessor.SetValue(object value)
        {
            SetValue((TMember)value);
        }

        public Type PropertyType
        {
            get { return typeof(TMember); }
        }

        public TMember Value 
        {
            get { return GetValue(); }
            set { SetValue(value); }
        }

        public string MemberName 
        {
            get { return _rtAccessor.MemberName; }
        }

        public Type MemberType 
        {
            get { return _rtAccessor.MemberType; }
        }

        public Type ObjectType 
        {
            get { return _rtAccessor.ObjectType; }
        }
    }
}