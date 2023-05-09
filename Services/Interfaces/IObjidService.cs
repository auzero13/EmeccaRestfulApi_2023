using com.emecca.service;

namespace EmeccaRestfulApi.Services.Interfaces
{
    [ServiceAlias("EmeccaObjidService")]
    public interface IObjidService : IEmeccaService
    {
        string GetEmeccaObjectId(String app_name);
    }
}
