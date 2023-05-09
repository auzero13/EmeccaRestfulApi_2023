using System.ComponentModel.DataAnnotations;
namespace EmeccaRestfulApi.Models
{
    public class STUDYINFOVO
    {
        [Required]
        public int? ID { get; set; }
        public String PAT_NAME { get; set; }
        public String PAT_NO { get; set; }
        public String BIRTH_DT { get; set; }
        public String PAT_SEX { get; set; }
        public String? STD_DATE { get; set; }
        public String? STD_TIME { get; set; }
        public String? ACC_NO { get; set; }
        public String STD_UID { get; set; }
        public String REF_DOC { get; set; }
        public String MODALITY { get; set; }
        public String PAT_ID { get; set; }
        public String STD_DESC { get; set; }
        public int SERIES_NUM { get; set; }
        public int IMAGE_NUM { get; set; }
    }
}
