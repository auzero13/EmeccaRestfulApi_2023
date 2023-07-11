using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using com.emecca.model;
using EmeccaRestfulApi.DBContext;
using EmeccaRestfulApi.Models;
using EmeccaRestfulApi.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace com.emecca.service
{
    public class DeleteArchiveLogServiceImpl : EmeccaService, IDeleteArchiveLogService
    {
        private readonly EmeccaDotNetContext _context;
        private readonly DCMASPEAFContext ea_context;
        private readonly ILogger<DeleteArchiveLogServiceImpl> _logger;
        private readonly IConfiguration _config;
        private readonly EmeccaObjectIdGenerator _generator;
        protected override object GetServiceInstance()
        {
            // 在此处获取您的DeleteArchiveLogService的实例
            return new DeleteArchiveLogServiceImpl(_context, ea_context, _logger,_config, _generator);
        }
        public DeleteArchiveLogServiceImpl(EmeccaDotNetContext context, DCMASPEAFContext context_ea, ILogger<DeleteArchiveLogServiceImpl> logger, IConfiguration config, EmeccaObjectIdGenerator generator)
        {          
            _context = context;
            ea_context = context_ea;
            _logger = logger;
            _config = config;
            _generator = generator;
        }
        //搬移(查STORE) -> 刪除-> 更新-> 回傳?
        [MethodAlias("ConfirmStudyDelete")]
        public ResponseModel ConfirmStudyDelete(JsonElement root)
        {
            // 在此处添加您的业务逻辑，例如将数据保存到数据库
            // 根据您的业务需求返回相应的响应
            try
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true,
                };
                DeleteArchiveLogVO vo = JsonSerializer.Deserialize<DeleteArchiveLogVO>(root.GetRawText(), options);
                _logger.LogInformation("執行影像刪除: "+ vo.AccessionNo);
                var backup_path = _config["BackupNasPath"];
                var store_info_list = ea_context.storage_info_vo.Where(a => a.STUDY_UID == vo.StudyUid).ToList();
                using (var transaction = ea_context.Database.BeginTransaction(System.Data.IsolationLevel.RepeatableRead))
                {
                    string sql = @"Delete tblDICOMImage WHERE _Id2 IN ( SELECT Id2 FROM tblDICOMSeries WHERE _Id1 IN ( SELECT Id1 FROM tblDICOMStudy WHERE [0020000D] = {0} ))";
                    try
                    {
                        if (!"R".Equals(vo.Status))
                        {
                            foreach (STORAGEINFOVO storage_vo in store_info_list)
                            {
                                var tmp = @"\\192.168.8.33\d$\Archives\EA4SP22NEW\LIB01\Incoming";
                                string image_path = Path.Combine(tmp, storage_vo.FILE_PATH);//storage_vo.IncomingPath
                                string targe_path = Path.Combine(backup_path, storage_vo.FILE_PATH);
                                string directoryPath = Path.GetDirectoryName(targe_path);
                                if (!Directory.Exists(directoryPath))
                                    Directory.CreateDirectory(directoryPath);
                                System.IO.File.Copy(image_path, targe_path, true);
                            }
                            int del_count = ea_context.Database.ExecuteSqlRaw(sql, vo.StudyUid);
                        }

                        using (var tran2 = _context.Database.BeginTransaction())
                        {

                            var delete_archive_exsit = _context.delete_archive_log_vo.Find(vo.ObjId);
                            if(delete_archive_exsit==null)
                            {
                                try
                                {
                                    vo.ObjId = _generator.NextEmeccaObjectId("API");
                                    _context.delete_archive_log_vo.Add(vo);
                                    int result = _context.SaveChanges();
                                    tran2.Commit();
                                    transaction.Commit();
                                    return new ResponseModel { Success = true, Message = "影像直接刪除成功" };
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("直接刪除異常，執行rollback:", ex.Message);
                                    tran2.Rollback();
                                    throw ex;
                                }
                                finally
                                { tran2.Dispose(); }

                            }
                            else
                            {
                                try
                                {
                                    _context.Entry(delete_archive_exsit).CurrentValues.SetValues(vo);
                                    _context.SaveChanges();
                                        tran2.Commit();
                                    transaction.Commit();
                                    return new ResponseModel { Success = true, Message = "影像審核成功" };
                                }
                                catch (Exception ex)
                                {
                                    _logger.LogError("更新審核異常，執行rollback:", ex.Message);
                                    tran2.Rollback();
                                    throw ex;
                                }
                                finally
                                {
                                    tran2.Dispose();
                                }
                            }
                            
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("刪除異常，執行rollback:", ex.Message);
                        transaction.Rollback();
                        throw ex;
                    }
                    finally
                    {
                        transaction.Dispose();
                    }

                }

            }
            catch (IOException ioex)
            {
                _logger.LogError("搬移檔案錯誤:" + ioex.Message);
                return new ResponseModel  { Success = false, Message = "影像備份搬移發生異常" + ioex.Message };
            }
            catch (Exception ex)
            {
                _logger.LogError("錯誤:" + ex.Message);
                return new ResponseModel { Success = false, Message = "發生異常" + ex.Message };
            }

        }

        [MethodAlias("GetDeleteArchiveLogList")]
        public List<DeleteArchiveLogVO> GetDeleteArchiveLogList(string pat_no, string acc_no, DateTime? str_date, DateTime? end_date, string status)
        {

            var last_date = end_date.Value;
            last_date = last_date.AddDays(1);
            try
            {
                var test = _context.delete_archive_log_vo.ToArray();
                var result = _context.delete_archive_log_vo.Where(c => (string.IsNullOrEmpty(pat_no) || c.PatientNo == pat_no) &&
                (string.IsNullOrEmpty(acc_no) || c.AccessionNo == acc_no) &&
                (string.IsNullOrEmpty(status) || c.Status == status) &&
                (str_date == null || c.ApplicantDateTime >= str_date) &&
                (end_date == null || c.ApplicantDateTime < last_date)).ToList();   //.Take(10)
                return result;
            }
            catch (Exception e)
            {
                this._logger.LogError("ERROR", e);
                //return StatusCode(500);
                return null;
            }
        }

        [MethodAlias("AddDeleteArchiveLog")]
        public ResponseModel AddDeleteArchiveLog(JsonElement root)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    DeleteArchiveLogVO vo = JsonSerializer.Deserialize<DeleteArchiveLogVO>(root.GetRawText(), options);
                    vo.ObjId = _generator.NextEmeccaObjectId("API");
                    _context.delete_archive_log_vo.Add(vo);
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
                catch (Exception ex)
                {
                    return new ResponseModel { Success = false, Message = ex.Message };
                    throw new NotImplementedException();
                }
            }
        }
    }
}
