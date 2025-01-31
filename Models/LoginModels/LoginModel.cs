using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FarmOps.Models.LoginModels
{
    [Table("tbl_login", Schema = "dbo")]
    public class LoginModel
    {
        [Key]
        [Column("Id", TypeName = "nvarchar(20)")]
        [Required]
        [ScaffoldColumn(false)]
        public string UserId { get; set; }

        [Required]
        [StringLength(50)]
        [Column("type")]
        public string AccountType { get; set; }

        [Column("password_change_status", TypeName = "bit")]
        public bool? PasswordChangeStatus { get; set; }

        [EmailAddress]
        [Required]
        [Column("Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(30)]
        [Column("Password")]
        public string Password { get; set; }

        [Column("visaver", TypeName = "int")]
        public int? Visaver { get; set; }

        [Column("last_ip", TypeName = "nvarchar(50)")]
        public string? LastIp { get; set; }

        [Column("last_login", TypeName = "nvarchar(50)")]
        public string? LastLoginTime { get; set; }

        [Column("issuper", TypeName = "bit")]
        public bool? Issuper { get; set; }

        [Column("prmocode", TypeName = "nvarchar(50)")]
        public string? Prmocode { get; set; }

        [Column("register_date", TypeName = "datetime")]
        public DateTime? RegisterDate { get; set; }

        [Column("industry_id", TypeName = "int")]
        public int? IndustryId { get; set; }

        [Column("package", TypeName = "int")]
        public int? package { get; set; }

        [Column("attendace_type", TypeName = "int")]
        public int? AttendaceType { get; set; }
    }
}
