
using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Users")]
    public class User
    {
        [Key]
        public int UserId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string FirstName { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string LastName { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Email { get; set; }
        [JsonIgnore]
        [System.ComponentModel.DataAnnotations.Required]
        public string Password { get; set; }
    }
}