using API.Aggregator.Application.Common.Models;
using HttpMessage;
using HttpMessage.Services;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace API.Aggregator.Infrastructure.Services;

public class PhotosService : BaseHttpService, IPhotosService
{
    private readonly string _photosServiceUrl;
    public PhotosService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        : base(httpClientFactory.CreateClient("InternalHttpClient"))
    {
        if (configuration is null)
            throw new ArgumentNullException(nameof(configuration));

        _photosServiceUrl =
            configuration.GetValue<string>("urls:internal:photos")
                ?? throw new InvalidOperationException("The expected configuration value 'urls:internal:photos' is missing.");
    }

    public async Task<AppIcon> AddProjectAppIcon(IFormFile file, string projectId, string name, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder(_photosServiceUrl);
        sb.Append($"/api/Icon?name={name}&projectId={projectId}");

        return await SendAsync<AppIcon>(new ApiRequest(HttpMethod.Post, sb.ToString(), fromForm: file), cancellationToken);
    }
}
