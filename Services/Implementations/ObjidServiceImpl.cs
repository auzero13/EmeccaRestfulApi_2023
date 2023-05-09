using com.emecca.service;
using EmeccaRestfulApi.DBContext;
using EmeccaRestfulApi.Services.Interfaces;
using EmeccaRestfulApi.Utils;
using System.Reflection.Emit;

namespace EmeccaRestfulApi.Services.Implementations
{
    public class ObjidServiceImpl : EmeccaService, IObjidService
    {
        private readonly EmeccaDotNetContext _context;
        private readonly ILogger<ObjidServiceImpl> _logger;
        private readonly EmeccaObjectIdGenerator _generator;
        protected override object GetServiceInstance()
        {
            // Here you can get the instance of your MenuService
            return new ObjidServiceImpl(_context, _logger, _generator);
        }

        public ObjidServiceImpl(EmeccaDotNetContext context, ILogger<ObjidServiceImpl> logger, EmeccaObjectIdGenerator generator)
        {
            _context = context;
            _logger = logger;
            _generator = generator;
        }

        [MethodAlias("GetEmeccaObjectId")]
        public string GetEmeccaObjectId(string app_name)
        {
            try
            {
                string id = _generator.NextEmeccaObjectId(app_name);
                return id;
            }
            catch (Exception ex)
            {
                throw new NotImplementedException();
                return null;
            }
            
        }
      

    }
}
