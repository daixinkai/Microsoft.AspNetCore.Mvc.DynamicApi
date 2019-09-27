using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.DynamicApi
{
    class DynamicApiControllerPropertyActivator : IControllerPropertyActivator
    {
        public void Activate(ControllerContext context, object controller)
        {
            if (!controller.IsProxyApi())
            {
                return;
            }
            var service = ((IProxyApi<object>)controller).Service;
        } 

        public Action<ControllerContext, object> GetActivatorDelegate(ControllerActionDescriptor actionDescriptor)
        {
            return new Action<ControllerContext, object>((context, obj) =>
            {
                if (!obj.IsProxyApi())
                {
                    return;
                }

                var service = ((IProxyApi<object>)obj).Service;

            });
        }

    }
}
