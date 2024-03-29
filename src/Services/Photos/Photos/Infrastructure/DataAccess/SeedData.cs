﻿using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using MediatR;
using Microsoft.Azure.Amqp.Framing;
using Microsoft.Extensions.Configuration;
using System.Threading;

namespace Photos.Infrastructure.DataAccess;

public class SeedData
{
    private readonly ILogger<SeedData> _logger;
    private readonly BlobServiceClient _blobServiceClient;
    private readonly IConfiguration _configuration;
    private readonly BlobContainerClient _blobProjectsIconsContainerClient;
    private readonly BlobContainerClient _blobPhotosContainerClient;
    private readonly BlobContainerClient _blobProjectsPrivateIconsContainerClient;

    public SeedData(
        ILogger<SeedData> logger,
        BlobServiceClient blobServiceClient,
        IConfiguration configuration
    )
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _blobServiceClient =
            blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        ;
        _blobProjectsIconsContainerClient = _blobServiceClient.GetBlobContainerClient(
            blobContainerName: _configuration.GetValue<string>(
                "AzureBlobStorage:BlobContainerProjectsIcons"
            )
                ?? throw new InvalidOperationException(
                    "The expected configuration value 'AzureBlobStorage:BlobContainerProjectsIcons' is missing."
                )
        );
        _blobPhotosContainerClient = _blobServiceClient.GetBlobContainerClient(
            blobContainerName: _configuration.GetValue<string>(
                "AzureBlobStorage:BlobContainerPhotos"
            )
                ?? throw new InvalidOperationException(
                    "The expected configuration value 'AzureBlobStorage:BlobContainerPhotos' is missing."
                )
        );
        _blobProjectsPrivateIconsContainerClient = _blobServiceClient.GetBlobContainerClient(
            blobContainerName: _configuration.GetValue<string>("AzureBlobStorage:BlobContainerProjectsPrivateIcons")
                ?? throw new InvalidOperationException(
                    "The expected configuration value 'AzureBlobStorage:BlobContainerProjectsPrivateIcons' is missing."
                )
        );
    }

    public async Task InitialiseAsync()
    {
        try
        {
            await _blobProjectsIconsContainerClient.CreateIfNotExistsAsync();
            await _blobProjectsIconsContainerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
            await _blobPhotosContainerClient.CreateIfNotExistsAsync();
            await _blobPhotosContainerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
            await _blobProjectsPrivateIconsContainerClient.CreateIfNotExistsAsync();
            await _blobProjectsPrivateIconsContainerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising.");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding.");
            throw;
        }
    }

    public async Task TrySeedAsync()
    {
        var blobUploadOptions = new BlobUploadOptions
        {
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "image/png",
                ContentDisposition = "inline"
            },
        };

        string folderPath = "./Infrastructure/DataAccess/ProjectsIcons";
        string[] filePaths = Directory.GetFiles(folderPath);

        var uploadTasks = filePaths
            .Select(async filePath =>
            {
                string fileName = Path.GetFileName(filePath);
                var blobClient = _blobProjectsIconsContainerClient.GetBlobClient(fileName);
                using var fileStream = File.OpenRead(filePath);
                await blobClient.UploadAsync(fileStream, blobUploadOptions);
            })
            .ToList();

        folderPath = "./Infrastructure/DataAccess/Photos";
        filePaths = Directory.GetFiles(folderPath);

        uploadTasks.AddRange(
            filePaths.Select(async filePath =>
            {
                string fileName = Path.GetFileName(filePath);
                var blobClient = _blobPhotosContainerClient.GetBlobClient(fileName);
                using var fileStream = File.OpenRead(filePath);
                await blobClient.UploadAsync(fileStream, blobUploadOptions);
            })
        );

        await Task.WhenAll(uploadTasks);
    }
}
