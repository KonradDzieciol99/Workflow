using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MediatR;

namespace Photos.Application.Icon.Queries;

public record GetProjectsIconsQuery() : IRequest<List<Domain.Entity.Icon>>;

public class GetProjectsIconsQueryHandler
    : IRequestHandler<GetProjectsIconsQuery, List<Domain.Entity.Icon>>
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

    public async Task<List<Domain.Entity.Icon>> Handle(
        GetProjectsIconsQuery request,
        CancellationToken cancellationToken
    )
    {
        var blobContainerProjectsIconsName = _configuration.GetValue<string>(
            "AzureBlobStorage:BlobContainerProjectsIcons"
        ) ?? throw new InvalidOperationException("The expected configuration value 'AzureBlobStorage:BlobContainerProjectsIcons' is missing.");

        var url = _configuration.GetValue<string>("urls:external:azureBlobStorage") 
            ?? throw new InvalidOperationException("The expected configuration value 'urls:external:azureBlobStorage' is missing.");

        var blobPhotosContainerClient = _blobServiceClient.GetBlobContainerClient(
            blobContainerProjectsIconsName
        );

        var icons = new List<Domain.Entity.Icon>();

        await foreach (
            BlobItem file in blobPhotosContainerClient.GetBlobsAsync(
                cancellationToken: cancellationToken
            )
        )
        {
            string uri = $"{url}/{blobContainerProjectsIconsName}";
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            icons.Add(new Domain.Entity.Icon(fullUri, name));
        }

        return icons;
    }
}
