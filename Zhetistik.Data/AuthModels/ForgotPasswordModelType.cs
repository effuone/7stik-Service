using System.ComponentModel.DataAnnotations;

namespace Zhetistik.Data.AuthModels
{
    public class ForgotPasswordModelType
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}