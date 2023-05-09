using System.ComponentModel.DataAnnotations;

namespace EmeccaRestfulApi.Models
{
    public class EmeUserBasVO
    {
        [Required]
        public String? ObjId { get; set; }
        public String UserName { get; set; }
        public String Password { get; set; }
        public String Name { get; set; }
        public String? Status { get; set; }
        public String? IsApplicant { get; set; }
        public String? IsApprover { get; set; }
        public String IsAdmin { get; set; }
    }
}
