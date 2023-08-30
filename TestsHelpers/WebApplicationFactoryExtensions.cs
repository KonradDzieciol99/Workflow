using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace TestsHelpers;
public static class WebApplicationFactoryExtensions
{
    public static List<TEntity> SeedData<TProgram, TApplicationDbContext, TEntity>(this WebApplicationFactory<TProgram> factory, List<TEntity> entities)
        where TProgram : class
        where TApplicationDbContext : DbContext
        where TEntity : class
    {
        var scopeFactory = factory.Services.GetService<IServiceScopeFactory>() ?? throw new ArgumentNullException(nameof(IServiceScopeFactory));

        using (var scope = scopeFactory.CreateScope())
        {
            var _dbContext = scope.ServiceProvider.GetService<TApplicationDbContext>() ?? throw new ArgumentNullException(nameof(TApplicationDbContext));

            _dbContext.AddRange(entities);

            var result = _dbContext.SaveChanges();
        }

        return entities;
    }
    public async static Task<TEntity?> FindAsync<TProgram, TApplicationDbContext, TEntity>(this WebApplicationFactory<TProgram> factory, params object[] keyValues)
        where TProgram : class
        where TApplicationDbContext : DbContext
        where TEntity : class
    {
        var scopeFactory = factory.Services.GetService<IServiceScopeFactory>();

        using var scope = scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<TApplicationDbContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }
}
