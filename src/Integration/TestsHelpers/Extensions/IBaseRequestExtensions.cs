using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TestsHelpers.Extensions;

public static class IBaseRequestExtensions
{
    public static StringContent ToStringContent(this IBaseRequest request) =>
        new(JsonSerializer.Serialize<object>(request), Encoding.UTF8, "application/json");
}
