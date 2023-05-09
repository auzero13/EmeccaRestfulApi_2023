using System;
using System.Collections.Generic;
using com.emecca.model;
using EmeccaRestfulApi.DBContext;
using EmeccaRestfulApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace com.emecca.service
{
    public class EmeccaDeletePacsApplyServiceImpl : EmeccaService, IEmeccaDeletePacsApplyService
    {
        private readonly DCMASPEAFContext _context;
        private readonly ILogger<EmeccaDeletePacsApplyServiceImpl> _logger;
        protected override object GetServiceInstance()
        {
            return new EmeccaDeletePacsApplyServiceImpl(_context, _logger);
        }

        public EmeccaDeletePacsApplyServiceImpl(DCMASPEAFContext context_ea, ILogger<EmeccaDeletePacsApplyServiceImpl> logger)
        {
            _context = context_ea;
            _logger = logger;
        }


        [MethodAlias("GetStudyList")]
        public List<STUDYINFOVO> GetStudyList(string? pat_no, string? acc_no, string? start_date, string? end_date)
        {
            try
            {
                //var query1 = _context.study_info_vo.ToList();
                var query =  _context.study_info_vo.Where(c => (String.IsNullOrWhiteSpace(pat_no) || c.PAT_NO == pat_no) &&
                (String.IsNullOrWhiteSpace(acc_no) || c.ACC_NO == acc_no) &&
                (String.IsNullOrWhiteSpace(start_date) || c.STD_DATE.CompareTo(start_date) >= 0) &&
                (String.IsNullOrWhiteSpace(end_date) || c.STD_DATE.CompareTo(end_date) <= 0)
                ).ToList();

                return query;
            }
            catch (Exception ex)
            {
                this._logger.LogError("ERROR", ex);
                return null;
            }
        }

    }
}
