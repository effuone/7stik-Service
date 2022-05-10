namespace Zhetistik.Data.ViewModels
{
    public class LocationViewModel
    {
        public int LocationId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
    }
    public class CreateLocationViewModel
    {
        public int LocationId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
    }
    public class UpdateLocationViewModel
    {
        public int LocationId { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
    }
}