using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("AchievementTypes")]
    public class AchievementType
    {
        [Key]
        public int AchievementTypeId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string AchievementTypeName { get; set; }
    }
}
