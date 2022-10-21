namespace MovingToAzure.Data;

public interface IImageRepository
{
    Task<string?> SaveProfilePic(int profileId, IFormFile file);
}

public class ImageRepository : IImageRepository
{
    private readonly string webRootPath;

    public ImageRepository(IWebHostEnvironment env)
    {
        if (env == null)
        {
            throw new ArgumentNullException(nameof(env));
        }
        this.webRootPath = env.WebRootPath; // the wwwroot folder
    }

    public async Task<string?> SaveProfilePic(int profileId, IFormFile file)
    {
        if (file.Length <= 0)
        {
            return null;
        }
        string filename = Uri.EscapeDataString($"{profileId}-{file.FileName}".Replace(" ", "-").ToLowerInvariant());
        
        string appdata = Path.Combine(webRootPath, "AppData");
        string filePath = Path.Combine(appdata, filename);

        using (Stream fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return $"/appdata/{filename}"; // turn filename into URL
    }

}
