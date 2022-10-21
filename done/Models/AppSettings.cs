namespace MovingToAzure.Models
{
    public class AppSettings
    {
        public string BlobContainerName { get; set; } = "";
        public int CacheSeconds { get; set; }
    }
}
