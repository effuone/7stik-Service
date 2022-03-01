using System;
using System.Collections.Generic;

namespace Zhetistik.Data.Models
{
    public class Achievement
    {
        public int AchievementId { get; set; }
        public string AchievementName { get; set; }
        public string Description { get; set; }
        public string ImageName { get; set; }
        public int AchievementTypeId {get; set;}
        public AchievementType AchievementType {get; set;}
    }
}
