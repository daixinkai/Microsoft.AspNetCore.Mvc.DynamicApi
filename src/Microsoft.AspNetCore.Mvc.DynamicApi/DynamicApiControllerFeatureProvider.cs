using Microsoft.AspNetCore.Mvc.Controllers;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.DynamicApi
{
    public class DynamicApiControllerFeatureProvider : ControllerFeatureProvider
    {        
        protected override bool IsController(TypeInfo typeInfo)
        {
            return typeInfo.IsProxyApi();
        }
    }
}
