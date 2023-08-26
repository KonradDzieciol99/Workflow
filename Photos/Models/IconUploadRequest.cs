using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;

namespace Photos.Models;

//public class IconUploadRequest
//{
//    public IFormFile File { get; set; }
//    public string Name { get; set; }
//}
public record class IconUploadRequest(
    [FromServices] BlobServiceClient blobServiceClient,
    [FromForm] IFormFile File,
    [FromQuery] string Name);
