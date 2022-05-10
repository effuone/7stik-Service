using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Zhetistik.Data.Models;

namespace Zhetistik.Data.ViewModels
{
    public class AchievementViewModel
    {
        public int AchievementId { get; set; }
        public string AchievementTypeName { get; set; }
        public string AchievementName { get; set; }
        public string Description { get; set; }
        public string FilePath { get; set; }
    }
    public class CreateAchievementViewModel
    {
        [Required]
        public string AchievementName { get; set; }
        [Required]
        public int AchievementTypeId { get; set; }
        [Required]
        public string Description { get; set; }
        #nullable enable
        public IFormFile? File { get; set; }
        #nullable disable
    }
    public class UpdateAchievementViewModel
    {
        [Required]
        public string AchievementName { get; set; }
        [Required]
        public int AchievementTypeId { get; set; }
        [Required]
        public string Description { get; set; }
        #nullable enable
        public IFormFile? File { get; set; }
        #nullable disable
    }
}