using System.Collections.Generic;

namespace com.emecca.service
{
    public interface IEmeccaService
    {
        object CallMethod(string methodName, List<object> parameters);
    }
}
