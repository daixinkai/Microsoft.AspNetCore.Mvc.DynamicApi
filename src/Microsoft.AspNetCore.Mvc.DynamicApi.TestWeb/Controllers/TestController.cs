using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Mvc.DynamicApi.TestWeb.Controllers
{
    [Route("api/test")]
    public class TestController : ControllerBase
    {
        public TestController(ITestService testService)
        {
            _testService = testService;
        }

        ITestService _testService;

        [HttpGet("name/{id}")]
        public Task<string> GetName(int id)
        {
            return _testService.GetName(id);
        }

    }
}
