using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace FarmOps.Models.LoginModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Select valid account type.")]
        [StringLength(50)]
        [Column("type")]
        [DisplayName("User Type")]
        public string AccountType { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required.")]
        [Column("Email")]
        [DisplayName("Eamil ID")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(30)]
        [Column("Password")]
        [DisplayName("Password")]
        public string Password { get; set; }

    }
}
