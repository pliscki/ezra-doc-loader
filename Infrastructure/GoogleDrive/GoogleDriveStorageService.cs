using Ezra.Scribe.Core.CloudStorage;
using Ezra.Scribe.Core.CloudStorage.GoogleDrive;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace Ezra.Scribe.Infrastructure.GoogleDrive
{
    public class GoogleDriveStorageService : ICloudStorageService
    {
        private readonly IGoogleDriveAuthService _authService;

        public GoogleDriveStorageService(IGoogleDriveAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        public async Task<IEnumerable<string>> ListFilesAsync(string location, string userId, CancellationToken cancellationToken)
        {
            var credential = await _authService.GetGoogleCredentialsAsync(userId, cancellationToken).ConfigureAwait(false);

            using var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Ezra Scribe",
            });

            var request = driveService.Files.List();
            var queryParts = new List<string> { "trashed = false" };

            if (!string.IsNullOrEmpty(location))
            {
                queryParts.Add($"'{location}' in parents");
            }

            request.Q = string.Join(" and ", queryParts);
            request.Fields = "files(id, name)";
            var result = await request.ExecuteAsync(cancellationToken);

            return result.Files.Select(f => f.Name);
        }

        public async Task<IEnumerable<GoogleDriveFileMetadata>> ListFilesWithMetadataAsync(string location, string userId, CancellationToken cancellationToken)
        {
            var credential = await _authService.GetGoogleCredentialsAsync(userId, cancellationToken).ConfigureAwait(false);

            using var driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "Ezra Scribe",
            });

            var request = driveService.Files.List();
            var queryParts = new List<string> { "trashed = false" };

            if (!string.IsNullOrEmpty(location))
            {
                queryParts.Add($"'{location}' in parents");
            }

            request.Q = string.Join(" and ", queryParts);
            request.Fields = "files(id, name, mimeType)";
            var result = await request.ExecuteAsync(cancellationToken);

            return result.Files.Select(f => new GoogleDriveFileMetadata
            {
                Id = f.Id,
                Name = f.Name,
                MimeType = f.MimeType
            });
        }

        public Task<Stream> DownloadFileAsync(string fileId, string userId, CancellationToken cancellationToken)
        {
            // TODO: Implement Google Drive file download using _authService
            throw new NotImplementedException();
        }
    }
}
