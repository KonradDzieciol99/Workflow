using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestsHelpers.Extensions;

public static class WebApplicationFactoryExtensions
{
    public static List<TEntity> SeedData<TProgram, TApplicationDbContext, TEntity>(
        this WebApplicationFactory<TProgram> factory,
        List<TEntity> entities
    )
        where TProgram : class
        where TApplicationDbContext : DbContext
        where TEntity : class
    {
        var scopeFactory =
            factory.Services.GetService<IServiceScopeFactory>()
            ?? throw new InvalidOperationException(
                $"{nameof(IServiceScopeFactory)} Missing required services."
            );

        using (var scope = scopeFactory.CreateScope())
        {
            var _dbContext =
                scope.ServiceProvider.GetService<TApplicationDbContext>()
                ?? throw new InvalidOperationException(
                    $"{nameof(TApplicationDbContext)} Missing required services."
                );

            _dbContext.AddRange(entities);

            var result = _dbContext.SaveChanges();
        }

        return entities;
    }

    public static async Task<TEntity?> FindAsync<TProgram, TApplicationDbContext, TEntity>(
        this WebApplicationFactory<TProgram> factory,
        params object[] keyValues
    )
        where TProgram : class
        where TApplicationDbContext : DbContext
        where TEntity : class
    {
        var scopeFactory =
            factory.Services.GetService<IServiceScopeFactory>()
            ?? throw new InvalidOperationException(
                $"{nameof(IServiceScopeFactory)} Missing required services."
            );

        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }
}
