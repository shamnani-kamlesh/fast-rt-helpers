namespace FastRT
{
    /// <summary>
    /// Wraps access to object's property or field
    /// </summary>
    public interface IObjectMemberAccessor : IMemberInfo
    {
        object GetValue();
        void SetValue(object value);
    }

    public interface IObjectMemberAccessor<V> : IObjectMemberAccessor
    {
        V Value { get; set; }
    }

}