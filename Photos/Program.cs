
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Mvc;
using Photos.Models;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Net.Mime;
using static System.Net.WebRequestMethods;
using System.Xml.Linq;
using FluentValidation;
using System.Reflection;
using Photos.Common;

namespace Photos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddAuthorization();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddScoped(opt =>
            {
                return new BlobServiceClient(builder.Configuration.GetConnectionString("AzureStorage"));
            });

            builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            var CORSallowAny = "allowAny";
            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy(name: CORSallowAny,
                          policy =>
                          {
                              policy.WithOrigins("https://localhost:4200", "https://127.0.0.1:5500")
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                          });
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors(CORSallowAny);

            app.UseAuthorization();

            app.MapPost("/api/uploadIcon", async ([AsParameters] IconUploadRequest IconUploadRequest) =>
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
            .WithOpenApi()
            //.RequireAuthorization()
            .AddEndpointFilter<ValidatorFilter<IconUploadRequest>>();

            app.MapGet("/api/getProjectsIcons", async ([FromServices] BlobServiceClient blobServiceClient) =>
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
            })
            .WithOpenApi();

            app.Run();
        }
    }
}