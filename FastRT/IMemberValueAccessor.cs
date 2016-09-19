namespace FastRT
{
    public interface IMemberValueAccessor
    {
        object GetValue(string memberName, object instance);
        void SetValue(string memberName, object instance, object value);
    }
}