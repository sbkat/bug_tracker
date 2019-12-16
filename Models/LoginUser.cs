using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace bug_tracker.Models
{
    [NotMapped]
    public class LoginUser
    {
        [Display (Name="Email: ")]
        [Required (ErrorMessage = "Enter your email.")]
        [EmailAddress]
        public string email { get; set; }
        [Display (Name="Password: ")]
        [Required (ErrorMessage = "Enter your password.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string password { get; set; }
    }
}