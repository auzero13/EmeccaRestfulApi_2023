
using EmeccaRestfulApi.DBContext;
using EmeccaRestfulApi.Models;
using System.Collections.Concurrent;

namespace EmeccaRestfulApi.Utils
{
    public class EmeccaObjectIdGenerator
    {
        private static readonly object lockObject = new object();
        private static readonly Dictionary<string, ConcurrentQueue<string>> cache = new Dictionary<string, ConcurrentQueue<string>>();

        public  String NextEmeccaObjectId(string name)
        {
            try
            {
                ConcurrentQueue<string> queue;
                lock (lockObject)
                {
                    if (!cache.TryGetValue(name, out queue) || queue.Count == 0)
                    {
                        EmeccaObjectIdVO vo =  ReleaseNewEmeccaObjectId(name);

                        if(vo==null)
                        {
                            return DateTime.Now.ToString("yyyyMMddHHmmssff");
                        }
                        queue = new ConcurrentQueue<string>();
                        for (int i = vo.RVal; i > 0; i--)
                        {
                            vo.LVal++;
                            var obj = String.Format("{0:0000000000000000}", vo.LVal);
                            queue.Enqueue(obj);
                        }
                        cache[name] = queue;
                    }
                }
                string result; 
                queue.TryDequeue(out result);
                return result;
            }
            catch(Exception ex)
            {
                return DateTime.Now.ToString("yyyyMMddHHmmssff");
            }
            
        }

        private  EmeccaObjectIdVO ReleaseNewEmeccaObjectId(string name)
        {
            EmeccaObjectIdVO result = null;

            using (var context = new EmeccaDotNetContext())
            {
                var vo =  context.emecca_objid_vo.FirstOrDefault(x => x.SysName == name);

                if (vo == null)
                {
                    return result;
                }                
                //更新
                vo.LVal = vo.LVal + vo.RVal;
                vo.LastAccessDT = DateTime.Now;
                context.SaveChanges();
                result = vo;
            }

            return result;
        }
    }
}
