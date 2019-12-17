using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace bug_tracker.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }
        [Display (Name="First Name: ")]
        [Required (ErrorMessage = "Enter your first name.")]
        [MinLength(2, ErrorMessage = "First name must be at least 2 characters.")]
        public string firstName { get; set; }
        [Display (Name="Last Name: ")]
        [Required (ErrorMessage = "Enter your last name.")]
        [MinLength(2, ErrorMessage = "Last name must be at least 2 characters.")]
        public string lastName { get; set; }
        [Display (Name="Email: ")]
        [Required (ErrorMessage = "Enter your email.")]
        [EmailAddress]
        public string email { get; set; }
        [Display (Name="Password: ")]
        [Required (ErrorMessage = "Enter your password.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        [ContainsNumber]
        [ContainsSpecialCharacter]
        [ContainsLetter]
        public string password { get; set; }
        [NotMapped]
        [Display (Name="Confirm Password: ")]
        [Required (ErrorMessage = "Please confirm your password.")]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [Compare("password", ErrorMessage = "Passwords do not match.")]
        [DataType(DataType.Password)]
        public string confirmPassword { get; set; }
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;
        public List<Ticket> CreatedTickets { get; set; }
    }
}