using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Microsoft.AspNetCore.Mvc.DynamicApi.TestWeb.NETCORE30.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILogger<IndexModel> logger)
        {
            _logger = logger;
        }

        public void OnGet([FromServices]ITestService testService, int? id)
        {
            ViewData["value"] = testService.GetName(id.GetValueOrDefault()).Result;
        }
    }
}
