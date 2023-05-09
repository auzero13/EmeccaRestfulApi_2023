using System.ComponentModel.DataAnnotations;

namespace EmeccaRestfulApi.Models
{
    public class DeleteArchiveLogVO
    {
        [Required]
        public String? ObjId { get; set; }
        public String? ApplicantUserName { get; set; }
        public String? ReviewUserName { get; set; }
        public String? AccessionNo { get; set; }
        public String? StudyDescription { get; set; }
        public String? PatientNo { get; set; }
        [Required]
        public String? StudyUid { get; set; }
        public String? Status { get; set; }
        public String? DeleteReason { get; set; }
        public String? AgentName { get; set; }
        public String? AgentReason { get; set; }
        public String? Modality { get; set; }
        public String? StudyDate { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? ApplicantDateTime { get; set; }
        [DataType(DataType.DateTime)]
        public DateTime? ReviewDateTime { get; set; }
        public String? RejectReason { get; set; }
        public String? ImageNum { get; set; }
    }
}
