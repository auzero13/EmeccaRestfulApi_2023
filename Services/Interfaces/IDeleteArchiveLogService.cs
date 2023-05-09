using com.emecca.model;
using EmeccaRestfulApi.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace com.emecca.service
{
    [ServiceAlias("DeleteArchiveLogService")]
    public interface IDeleteArchiveLogService : IEmeccaService
    {
        ResponseModel ConfirmStudyDelete(JsonElement root);
        
        List<DeleteArchiveLogVO> GetDeleteArchiveLogList(string pat_no, string acc_no, DateTime? str_date, DateTime? end_date, string status);

        ResponseModel AddDeleteArchiveLog(JsonElement root);
    }
}
