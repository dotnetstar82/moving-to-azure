using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace MovingToAzure.Data;

public interface IImageRepository
{
    Task<string?> SaveProfilePic(int profileId, IFormFile file);
}

// https://docs.microsoft.com/en-us/azure/storage/common/storage-samples-dotnet
public class ImageRepository : IImageRepository
{
    private readonly BlobContainerClient container;

    public ImageRepository(string connectionString, string containerName)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString));
        }
        if (string.IsNullOrEmpty(containerName))
        {
            throw new ArgumentNullException(nameof(containerName));
        }

        container = new BlobContainerClient(connectionString, containerName);
        container.CreateIfNotExists();
        container.SetAccessPolicy(PublicAccessType.Blob);
    }

    public async Task<string?> SaveProfilePic(int profileId, IFormFile file)
    {
        if (file.Length <= 0)
        {
            return null;
        }
        string filename = Uri.EscapeDataString($"{profileId}-{file.FileName}".Replace(" ", "-").ToLowerInvariant());

        BlobClient blob = container.GetBlobClient(filename);

        var stream = file.OpenReadStream();
        await blob.UploadAsync(stream, overwrite: true);

        // ASSUME: 200 characters or less
        return blob.Uri.AbsoluteUri;
    }

}
