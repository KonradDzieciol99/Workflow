using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MediatR;

namespace Photos.Application.Icon.Queries;

public record GetProjectsIconsQuery(string? ProjectId) : IRequest<List<Domain.Entity.AppIcon>>;

public class GetProjectsIconsQueryHandler
    : IRequestHandler<GetProjectsIconsQuery, List<Domain.Entity.AppIcon>>
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;

    public GetProjectsIconsQueryHandler(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration
    )
    {
        this._blobServiceClient =
            blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        this._configuration =
            configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<List<Domain.Entity.AppIcon>> Handle(
        GetProjectsIconsQuery request,
        CancellationToken cancellationToken
    )
    {
        var url =
            _configuration.GetValue<string>("urls:external:azureBlobStorage")
            ?? throw new InvalidOperationException(
                "The expected configuration value 'urls:external:azureBlobStorage' is missing."
            );
        var blobContainerProjectsIconsName =
            _configuration.GetValue<string>("AzureBlobStorage:BlobContainerProjectsIcons")
            ?? throw new InvalidOperationException(
                "The expected configuration value 'AzureBlobStorage:BlobContainerProjectsIcons' is missing."
            );

        var blobPhotosContainerClient = _blobServiceClient.GetBlobContainerClient(
            blobContainerProjectsIconsName
        );

        var icons = new List<Domain.Entity.AppIcon>();

        await foreach (
            BlobItem file in blobPhotosContainerClient.GetBlobsAsync(
                cancellationToken: cancellationToken
            )
        )
        {
            string uri = $"{url}/{blobContainerProjectsIconsName}";
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            icons.Add(new Domain.Entity.AppIcon(fullUri, name));
        }

        if (request.ProjectId is null)
            return icons;

        var blobContainerProjectsPrivateIcons =
            _configuration.GetValue<string>("AzureBlobStorage:BlobContainerProjectsPrivateIcons")
            ?? throw new InvalidOperationException(
                "The expected configuration value 'AzureBlobStorage:BlobContainerProjectsPrivateIcons' is missing."
            );

        var blobContainerProjectsPrivateIconsClient = _blobServiceClient.GetBlobContainerClient(
            blobContainerProjectsPrivateIcons
        );

        await foreach (BlobItem file in blobContainerProjectsPrivateIconsClient.GetBlobsAsync(cancellationToken: cancellationToken , prefix: request.ProjectId))
        {
            string uri = $"{url}/{blobContainerProjectsPrivateIcons}";
            var name = file.Name;
            var fullUri = $"{uri}/{name}";
            icons.Add(new Domain.Entity.AppIcon(fullUri, name));
        }

        return icons;
    }
}
