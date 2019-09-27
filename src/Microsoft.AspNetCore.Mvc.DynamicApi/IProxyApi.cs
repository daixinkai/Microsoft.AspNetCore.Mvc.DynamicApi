using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.DynamicApi
{
    public interface IProxyApi
    {
    }
    public interface IProxyApi<out TService> : IProxyApi
    {
        Type ServiceType { get; }
        TService Service { get; }
    }
}
