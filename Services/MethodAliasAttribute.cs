using System;

namespace com.emecca.service
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MethodAliasAttribute : Attribute
    {
        public string Alias { get; }

        public MethodAliasAttribute(string alias)
        {
            Alias = alias;
        }
    }
}
