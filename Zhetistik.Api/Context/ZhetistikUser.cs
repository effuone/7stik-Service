using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Zhetistik.Api.Context
{
    public class ZhetistikUser : IdentityUser
    {
        [MaxLength(30)]
        public string FirstName { get; set; }
        [MaxLength(40)]
        public string LastName { get; set; }
        public int? CandidateId { get; set; }
        [ForeignKey("CandidateId")]
        public Candidate Candidate { get; set; }
    }
}