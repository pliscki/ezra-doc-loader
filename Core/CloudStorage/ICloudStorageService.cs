namespace Ezra.Scribe.Core.CloudStorage
{
    public interface ICloudStorageService
    {
        Task<IEnumerable<string>> ListFilesAsync(string location, string credentials, CancellationToken cancellationToken);
        Task<Stream> DownloadFileAsync(string fileId, string credentials, CancellationToken cancellationToken);
    }
}
