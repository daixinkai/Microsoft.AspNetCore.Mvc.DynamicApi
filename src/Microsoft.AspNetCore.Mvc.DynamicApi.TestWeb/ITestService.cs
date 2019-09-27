using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.DynamicApi.TestWeb
{    
    [DynamicApi("api/testService")]
    public interface ITestService
    {
        [Microsoft.AspNetCore.Authorization.Authorize]
        [HttpGet("name/{id}")]
        Task<string> GetName(int id);
    }
}
