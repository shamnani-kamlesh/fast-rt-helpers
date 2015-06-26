namespace FastRT
{
    public interface IMemberAccessor : IMemberInfo
    {
        bool HasGetter { get; }
        bool HasSetter { get; }
        object GetValue(object instance);
        void SetValue(object instance, object value);
    }

    public interface IMemberAccessor<in TObjectType, TMemberType> : IMemberAccessor
    {
        TMemberType GetValue(TObjectType instance);
        void SetValue(TObjectType instance, TMemberType value);
    }
}