using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Zhetistik.Data.Context
{
    public class ZhetistikUser : IdentityUser
    {
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(40)]
        public string LastName { get; set; }
    }
}