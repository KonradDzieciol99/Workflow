using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.IntegrationTests.Application.FriendRequests.Queries;
[Collection("Base")]
public class GetReceivedFriendRequestsQueryTests: IAsyncLifetime
{
    private readonly Base _base;
    public GetReceivedFriendRequestsQueryTests(Base @base)
    {
        _base = @base;
    }
    public async Task InitializeAsync()
    {
        await _base._checkpoint.ResetAsync(_base._msSqlContainer.GetConnectionString());
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}
