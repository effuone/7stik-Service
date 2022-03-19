using System.ComponentModel.DataAnnotations;

namespace Zhetistik.Data.AuthModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }
    }
}