using com.emecca.model;
using EmeccaRestfulApi.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;

namespace com.emecca.service
{
    [ServiceAlias("StudyInfo")]
    public interface IEmeccaDeletePacsApplyService : IEmeccaService
    {
        List<STUDYINFOVO> GetStudyList(string pat_no, string acc_no, string start_date, string end_date);
    }
}
