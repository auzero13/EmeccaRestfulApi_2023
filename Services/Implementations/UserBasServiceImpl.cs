using com.emecca.model;
using com.emecca.service;
using EmeccaRestfulApi.DBContext;
using EmeccaRestfulApi.Models;
using EmeccaRestfulApi.Services.Interfaces;
using EmeccaRestfulApi.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json;

namespace EmeccaRestfulApi.Services.Implementations
{
    public class UserBasServiceImpl : EmeccaService, IUserBasService
    {
        private readonly EmeccaDotNetContext _context;
        private readonly ILogger<UserBasServiceImpl> _logger;
        private readonly EmeccaObjectIdGenerator _generator;
        protected override object GetServiceInstance()
        {
            return new UserBasServiceImpl(_context, _logger, _generator);
        }
        public UserBasServiceImpl(EmeccaDotNetContext context, ILogger<UserBasServiceImpl> logger, EmeccaObjectIdGenerator generator)
        {
            _context = context;
            _logger = logger;
            _generator = generator;
        }
        [MethodAlias("GetUserList")]
        public  List<EmeUserBasVO> GetUserList(string user_no)
        {
            try
            {
                var result =  _context.emeuser_vo.Where(c => string.IsNullOrEmpty(user_no) || c.UserName == user_no).ToList();
                return result;
            }
            catch(Exception ex) 
            {
                this._logger.LogError("ERROR", ex);
                return null;
            }
        }

        [MethodAlias("AddUser")]
        public ResponseModel AddUser(JsonElement root)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    //EmeUserBasVO vo = new EmeUserBasVO
                    //{
                    //    ObjId = _generator.NextEmeccaObjectId("API"),
                    //    UserName = root.GetProperty("userName").GetString(),
                    //    Password = root.GetProperty("password").GetString(),
                    //    Name = root.GetProperty("name").GetString(),
                    //    Status = root.GetProperty("status").GetString(),
                    //    IsApplicant = root.GetProperty("isApplicant").GetString(),
                    //    IsApprover = root.GetProperty("isApprover").GetString(),
                    //    IsAdmin = root.GetProperty("isAdmin").GetString()
                    //};
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    EmeUserBasVO vo = JsonSerializer.Deserialize<EmeUserBasVO>(root.GetRawText(), options);
                    vo.ObjId = _generator.NextEmeccaObjectId("API");
                    _context.emeuser_vo.Add(vo);
                    int result = _context.SaveChanges();
                    if (result > 0)
                    {
                        transaction.Commit();
                        return new ResponseModel { Success = true, Message = "新增成功" };
                    }
                    else
                    {
                        return new ResponseModel { Success = false, Message = "新增失敗" };
                    }
                }
                catch (Exception e)
                {
                    //return StatusCode(500);
                    return new ResponseModel { Success = false, Message = e.Message+":"+e.InnerException.Message };
                }
            }

        }
        [MethodAlias("UpdateUserInfo")]
        public ResponseModel UpdateUserInfo(JsonElement root)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                //參考說明
                //https://blog.darkthread.net/blog/ef-core-upsert/
                //https://blog.darkthread.net/blog/ef-core-attach/
                try
                {
                    EmeUserBasVO vo = new EmeUserBasVO
                    {
                        ObjId = root.GetProperty("objId").GetString(),
                        UserName = root.GetProperty("userName").GetString(),
                        Password = root.GetProperty("password").GetString(),
                        Name = root.GetProperty("name").GetString(),
                        Status = root.GetProperty("status").GetString(),
                        IsApplicant = root.GetProperty("isApplicant").GetString(),
                        IsApprover = root.GetProperty("isApprover").GetString(),
                        IsAdmin = root.GetProperty("isAdmin").GetString()
                    };

                    var use_bas = _context.emeuser_vo.FirstOrDefault(o => o.ObjId.Equals(vo.ObjId));
                    //if(use_bas!=null)
                    //    _context.Entry(vo).State = EntityState.Modified;
                    if (use_bas != null)
                    {
                        _context.Entry(use_bas).CurrentValues.SetValues(vo);
                        _context.SaveChanges();
                        transaction.Commit();
                        return new ResponseModel { Success = true, Message = "更新成功" };
                    }
                    else
                    {
                        return new ResponseModel { Success = false, Message = "沒有資料可更新" };
                    }

                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return new ResponseModel { Success = false, Message = ex.Message +":"+ex.InnerException.Message };
                }
            }
        }
    }
}
