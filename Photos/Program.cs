
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Photos.Common;
using Photos.Models;

namespace Photos;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddWebAPIServices(builder.Configuration);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        //app.UseHttpsRedirection();

        app.UseRouting();

        app.UseCors("allowAny");

        app.UseAuthentication();

        app.UseAuthorization();

        var group = app.MapGroup("/api")
                       .RequireAuthorization("ApiScope")
                       .WithOpenApi();

        group.MapPost("/uploadIcon", async ([AsParameters] IconUploadRequest IconUploadRequest) =>
        {

            var blobPhotosContainerClient = IconUploadRequest.blobServiceClient.GetBlobContainerClient(builder.Configuration.GetValue<string>("AzureBlobStorage:BlobContainerProjectsIcons"));
            var blobClient = blobPhotosContainerClient.GetBlobClient(IconUploadRequest.Name);
            var blobUploadOptions = new BlobUploadOptions
            {
                HttpHeaders = new BlobHttpHeaders
                {
                    ContentType = "image/jpeg",
                    ContentDisposition = "inline"
                },
            };

            var result = await blobClient.UploadAsync(IconUploadRequest.File.OpenReadStream(), blobUploadOptions);

            return Results.Ok();
        })
        .AddEndpointFilter<ValidatorFilter<IconUploadRequest>>();

        group.MapGet("/getProjectsIcons", async ([FromServices] BlobServiceClient blobServiceClient) =>
        {
            var blobPhotosContainerClient = blobServiceClient.GetBlobContainerClient(builder.Configuration.GetValue<string>("AzureBlobStorage:BlobContainerProjectsIcons"));

            List<Icon> files = new List<Icon>();

            await foreach (BlobItem file in blobPhotosContainerClient.GetBlobsAsync())
            {
                string uri = blobPhotosContainerClient.Uri.ToString();
                var name = file.Name;
                var fullUri = $"{uri}/{name}";

                files.Add(new Icon
                {
                    Url = fullUri,
                    Name = name,
                });
            }

            return Results.Ok(files);
        });

        app.Run();
    }
}