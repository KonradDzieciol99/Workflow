using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Photos.Application.Common.Authorization;
using Photos.Common.Models;

namespace Photos.Application.Icon.Commands;

public record IconUploadCommand(IFormFile File, string Name) : IAuthorizationRequest
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>();
    }
}

public class IconUploadCommandHandler : IRequestHandler<IconUploadCommand>
{
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;

    public IconUploadCommandHandler(
        BlobServiceClient blobServiceClient,
        IConfiguration configuration
    )
    {
        this._blobServiceClient =
            blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        this._configuration =
            configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task Handle(IconUploadCommand request, CancellationToken cancellationToken)
    {
        var blobPhotosContainerClient = _blobServiceClient.GetBlobContainerClient(
            _configuration.GetValue<string>("AzureBlobStorage:BlobContainerProjectsIcons")
        );
        var blobClient = blobPhotosContainerClient.GetBlobClient(request.Name);
        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "image/jpeg",
                ContentDisposition = "inline"
            },
        };

        await blobClient.UploadAsync(
            request.File.OpenReadStream(),
            blobUploadOptions,
            cancellationToken
        );

        return;
    }
}
