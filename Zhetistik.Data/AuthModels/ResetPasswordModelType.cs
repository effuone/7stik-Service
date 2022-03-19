using System.ComponentModel.DataAnnotations;

namespace Zhetistik.Data.AuthModels
{
    public class ResetPasswordModelType
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
 
        [Required]
        [StringLength(100, ErrorMessage = "Password length has to be more than 6", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; }
 
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; }
 
        public string Code { get; set; }
    }
}