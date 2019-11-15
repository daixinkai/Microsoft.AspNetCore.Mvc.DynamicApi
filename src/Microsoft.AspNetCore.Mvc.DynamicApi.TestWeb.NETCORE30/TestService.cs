using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.DynamicApi.TestWeb.NETCORE30
{
    public class TestService : ITestService
    {
        public Task<string> GetName(int id)
        {
            return Task.FromResult("Name" + id);
        }
    }
}
