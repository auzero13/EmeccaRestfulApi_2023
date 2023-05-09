using System;

namespace com.emecca.model
{
    public class StudyInfoModel
    {
        public int Id { get; set; }
        public string PAT_NAME { get; set; }
        public string PAT_NO { get; set; }
        public string BIRTH_DT { get; set; }
        public string PAT_SEX { get; set; }
        public DateTime STD_DATE_TIME { get; set; }
        public string ACC_NO { get; set; }
        public string STD_UID { get; set; }
        public string REF_DOC { get; set; }
        public string Modality { get; set; }
        public string PAT_ID { get; set; }
        public string STD_DESC { get; set; }
        public int SERIES_NUM { get; set; }
        public int IMAGE_NUM { get; set; }
    }
}
