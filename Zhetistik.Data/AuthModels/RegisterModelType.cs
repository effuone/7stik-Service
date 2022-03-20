using System.ComponentModel.DataAnnotations;

namespace Zhetistik.Data.AuthModels
{
    public class RegisterModelType
    {
        [Required]
        [MaxLength(20)]
        public string UserName { get; set; }
        [Required]
        [MaxLength(20)]
        public string FirstName { get; set; }
        [MaxLength(20)]
        [Required]
        public string LastName { get; set; }
        [MaxLength(30)]
        [Required]
        public string Email { get; set; }
        [MaxLength(30)]
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string Role {get; set;}
        [Required]
        [MaxLength(40)]
        public string Password {get; set;}
    }
}