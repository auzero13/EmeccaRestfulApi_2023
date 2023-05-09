using com.emecca.service;
using EmeccaRestfulApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace EmeccaRestfulApi.Services.Interfaces
{
    [ServiceAlias("EmeUseInfo")]
    public interface IUserBasService : IEmeccaService
    {
        List<EmeUserBasVO> GetUserList(String user_no);
        ResponseModel AddUser(JsonElement root);

        ResponseModel UpdateUserInfo(JsonElement root);
    }
}
