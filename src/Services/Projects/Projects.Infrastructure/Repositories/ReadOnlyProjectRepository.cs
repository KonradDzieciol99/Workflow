﻿using Microsoft.EntityFrameworkCore;
using Projects.Application.Common.Interfaces;
using Projects.Domain.AggregatesModel.ProjectAggregate;
using Projects.Infrastructure.DataAccess;

namespace Projects.Infrastructure.Repositories;

public class ReadOnlyProjectRepository : IReadOnlyProjectRepository
{
    private IQueryable<Project> _projectsQuery;

    public ReadOnlyProjectRepository(ApplicationDbContext applicationDbContext)
    {
        this._projectsQuery = applicationDbContext.Projects.AsNoTracking();
    }
    public async Task<Project?> GetOneAsync(string projectId)
    {
        return await _projectsQuery.Include(x => x.ProjectMembers).SingleOrDefaultAsync(x => x.Id == projectId);
    }
}