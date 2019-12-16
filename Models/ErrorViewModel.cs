using System;
using System.ComponentModel.DataAnnotations;

namespace bug_tracker.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
    public class ContainsNumber : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value==null)
            {
                return new ValidationResult("Password required.");
            }
            string numbers = @"0123456789";
            foreach (var item in numbers)
            {
                foreach(var c in (string)value)
                {
                    if (c==item)
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            return new ValidationResult("Password must contain at least one number");
        }
    }
    public class ContainsSpecialCharacter : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value==null)
            {
                return new ValidationResult("Password required.");
            }
            string specialChar = @"\|!#$%&/()=?»«@£§€{}.-;'<>_,";
            foreach (var item in specialChar)
            {
                foreach(var c in (string)value)
                {
                    if (c==item)
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            return new ValidationResult("Password must contain at least one special character.");
        }
    }
    public class ContainsLetter : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if(value==null)
            {
                return new ValidationResult("Password required.");
            }
            string letter = @"abcdefghijklmnopqrstuvwxyz";
            foreach (var item in letter)
            {
                foreach(var c in (string)value)
                {
                    if (c==item)
                    {
                        return ValidationResult.Success;
                    }
                }
            }
            return new ValidationResult("Password must contain at least one letter.");
        }
    }
}