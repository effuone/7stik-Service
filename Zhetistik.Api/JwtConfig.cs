namespace Zhetistik.Api
{
    public class JwtConfig
    {
        public string Key { get; set; }
        public string ValidIssuer { get; set; }
        public string ValidAudience { get; set; }
    }
}