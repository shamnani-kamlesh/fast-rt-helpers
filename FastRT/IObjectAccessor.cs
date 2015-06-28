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
}