using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmOps.Models.LoginModels
{
    [Table("tbl_login", Schema = "dbo")]
    public class LoginModel
    {
        [Key]
        public string? UserId { get; set; }
        public string? EmailAddress { get; set; }
        public string? UserPassword { get; set; }
        public string? UserType { get; set; }
        public bool? IsPasswordChanged { get; set; }
        public int? VisaVerification { get; set; }
        public string? LastIpAddress { get; set; }
        public string? LastLoginTime { get; set; }
        public bool? IsSuperuser { get; set; }
        public string? PromoCode { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public int? IndustryId { get; set; }
        public int? Package { get; set; }
    }
}
