
using System.Text.Json.Serialization;
using Dapper.Contrib.Extensions;

namespace Zhetistik.Data.Models
{
    [Table("Locations")]
    public class Location
    {
        [Key]
        public int LocationId { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }
        [JsonIgnore]
        public City City { get; set; }
        [JsonIgnore]
        public Country Country { get; set; }
        public Location()
        {
            
        }
        public Location(int locationId, int countryId, int cityId)
        {
            LocationId = locationId;
            CountryId = countryId;
            CityId = cityId;
        }
    }
}
