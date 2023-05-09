[AttributeUsage(AttributeTargets.Interface)]
public class ServiceAliasAttribute : Attribute
{
    public string Alias { get; }

    public ServiceAliasAttribute(string alias)
    {
        Alias = alias;
    }
}