using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Achievements")]
    public class Achievement
    {
        [Key]
        public int AchievementId { get; set; }
        public int PortfolioId { get; set; }
        [JsonIgnore]
        public Portfolio Portfolio { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string AchievementName { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string Description { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string ImageName { get; set; }
        public int AchievementTypeId {get; set;}
        public AchievementType AchievementType {get; set;}
    }
}
