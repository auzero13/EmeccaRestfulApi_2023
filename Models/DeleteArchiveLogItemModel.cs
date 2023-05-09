using System;

namespace com.emecca.model
{
    public class DeleteArchiveLogItemModel
    {
        public string ObjId { get; set; }
        public string ApplicantUserName { get; set; }
        public string ReviewUserName { get; set; }
        public string AccessionNo { get; set; }
        public string StudyDescription { get; set; }
        public string PatientNo { get; set; }
        public string StudyUid { get; set; }
        public string Status { get; set; }
        public string DeleteReason { get; set; }
        public string AgentName { get; set; }
        public string AgentReason { get; set; }
        public string Modality { get; set; }
        public string StudyDate { get; set; }
        public DateTime ApplicantDateTime { get; set; }
        public DateTime ReviewDateTime { get; set; }
        public string RejectReason { get; set; }
        public string ImageNum { get; set; }
    }
}
