using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.DynamicApi
{
    public class DynamicApiControllerConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers.Where(s => s.ControllerType.IsProxyApi()))
            {
                string controllerName = controller.ControllerType.GetCustomAttribute<DynamicApiAttribute>()?.ControllerName;
                if (!string.IsNullOrWhiteSpace(controllerName))
                {
                    controller.ControllerName = controllerName;
                }
            }
        }
    }
}
