using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using HttpMessage.Authorization;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Photos.Domain.Entity;
using System;

namespace Photos.Application.Icon.Commands;

public record ProjectIconUploadCommand(IFormFile File, string ProjectId, string Name) : IAuthorizationRequest<AppIcon>
{
    public List<IAuthorizationRequirement> GetAuthorizationRequirement()
    {
        return new List<IAuthorizationRequirement>();
    }
}

public class IconUploadCommandHandler : IRequestHandler<ProjectIconUploadCommand, AppIcon>
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

    public async Task<AppIcon> Handle(ProjectIconUploadCommand request, CancellationToken cancellationToken)
    {
        var url = _configuration.GetValue<string>("urls:external:azureBlobStorage")
             ?? throw new InvalidOperationException(
                 "The expected configuration value 'urls:external:azureBlobStorage' is missing."
             );

        var blobContainerProjectsPrivateIconsName = _configuration.GetValue<string>("AzureBlobStorage:BlobContainerProjectsPrivateIcons")
                ?? throw new InvalidOperationException(
                    "The expected configuration value 'AzureBlobStorage:BlobContainerProjectsPrivateIcons' is missing."
                );

        var blobPhotosContainerClient = _blobServiceClient.GetBlobContainerClient(blobContainerProjectsPrivateIconsName);

        var blobClient = blobPhotosContainerClient.GetBlobClient($"{request.ProjectId}/{request.Name}");
        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "image/jpeg",
                ContentDisposition = "inline"
            },
        };

        var result = await blobClient.UploadAsync(
            request.File.OpenReadStream(),
            blobUploadOptions,
            cancellationToken
        );

        if (result.GetRawResponse().IsError)
            throw new InvalidOperationException(result.GetRawResponse().ReasonPhrase);

        string uri = $"{url}/{blobContainerProjectsPrivateIconsName}/{request.ProjectId}/{request.Name}";

        return new AppIcon(uri, request.Name);
    }
}
