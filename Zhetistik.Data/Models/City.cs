using System;
using System.Collections.Generic;
using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Cities")]
    public class City
    {
        [Key]
        public int CityId { get; set; }
        [System.ComponentModel.DataAnnotations.Required]
        public string CityName { get; set; }
    }
}
