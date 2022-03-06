namespace Zhetistik.Api.Settings
{
    public class ZhetistikDbSettings
    {
        public string Server { get; set; } = "localhost";
        public string Database { get; set; } = "ZhetistikDb";
        public string UserId { get; set; } = "SA";
        public string Password { get; set; }
        public ZhetistikDbSettings()
        {
            
        }
        public ZhetistikDbSettings(string server, string database, string userId, string password)
        {
            Server = server;
            Database = database;
            UserId = userId;
            Password = password;
        }
        public string GetAzureConnectionString()
        {
            return $"Server=tcp:{Server};Database={Database};User Id={UserId};Password={Password}";
        }
        public string GetSSMSConnectionString()
        {
            return $"Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog={Database}";
        }
    }
}