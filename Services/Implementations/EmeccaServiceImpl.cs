using System;
using System.Collections.Generic;
using System.Reflection;

namespace com.emecca.service
{
    public abstract class EmeccaService : IEmeccaService
    {
        protected abstract object GetServiceInstance();

        public object CallMethod(string methodName, List<object> parameters)
        {
            object service = GetServiceInstance();

            if (service == null)
            {
                throw new NullReferenceException("Service is null");
            }

            Type type = service.GetType();

            var method = GetMethodFromAlias(service,methodName);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found in {type.FullName}");
            }

            return method.Invoke(service, parameters?.ToArray());
        }
        private MethodInfo GetMethodFromAlias(object service, string methodName)
        {
            var methods = service.GetType().GetMethods();
            foreach (var method in methods)
            {
                var aliasAttr = method.GetCustomAttribute<MethodAliasAttribute>();
                if (aliasAttr != null && aliasAttr.Alias == methodName)
                {
                    return method;
                }
            }

            return null;
        }

    }
}
