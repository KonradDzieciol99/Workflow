using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Projects.IntegrationTests.Application.Projects.Commands;

[Collection("Base")]
public class CreateProjectCommandTests : IAsyncLifetime
{
    private readonly Base _base;

    public CreateProjectCommandTests(Base @base)
    {
        _base = @base;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    public async Task InitializeAsync()
    {
        await _base._checkpoint.ResetAsync(_base._msSqlContainer.GetConnectionString());
    }
}
