using API.Aggregator.Application.Common.Models;

namespace API.Aggregator.Infrastructure.Services;

public interface IPhotosService
{
    Task<AppIcon> AddProjectAppIcon(IFormFile file, string projectId, string name, CancellationToken cancellationToken);
}