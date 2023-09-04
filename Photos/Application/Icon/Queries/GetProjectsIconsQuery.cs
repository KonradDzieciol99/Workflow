using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using MediatR;
using Photos.Common.Models;

namespace Photos.Application.Icon.Queries;

public record GetProjectsIconsQuery() : IRequest<List<Domain.Entity.Icon>>
{
}
public class GetProjectsIconsQueryHandler : IRequestHandler<GetProjectsIconsQuery, List<Domain.Entity.Icon>>
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;

    public GetProjectsIconsQueryHandler(BlobServiceClient blobServiceClient,IConfiguration configuration)
    {
        this._blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(BlobServiceClient));
        this._configuration = configuration ?? throw new ArgumentNullException(nameof(IConfiguration));
    }

    public async Task<List<Domain.Entity.Icon>> Handle(GetProjectsIconsQuery request, CancellationToken cancellationToken)
    {
        
        var blobPhotosContainerClient = _blobServiceClient.GetBlobContainerClient(_configuration.GetValue<string>("AzureBlobStorage:BlobContainerProjectsIcons"));

        var icons = new List<Domain.Entity.Icon>();

        await foreach (BlobItem file in blobPhotosContainerClient.GetBlobsAsync())
        {
            string uri = blobPhotosContainerClient.Uri.ToString();
            var name = file.Name;
            var fullUri = $"{uri}/{name}";

            icons.Add(new Domain.Entity.Icon(fullUri, name));
        }

        return icons;
    }
}