using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.DynamicApi;
using Microsoft.AspNetCore.Mvc.DynamicApi.Reflection;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNetCore.Mvc
{
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class MvcBuilderExtensions
    {
        public static IMvcBuilder AddDynamicApi(this IMvcBuilder builder)
        {

            var feature = new ControllerFeature();

            foreach (AssemblyPart assemblyPart in builder.PartManager.ApplicationParts.OfType<AssemblyPart>())
            {
                foreach (var type in assemblyPart.Types)
                {
                    if (type.IsInterface && type.IsDefinedIncludingBaseInterfaces<DynamicApiAttribute>() && !type.IsDefined(typeof(NonDynamicApiAttribute)) && !type.IsGenericType)
                    {
                        feature.Controllers.Add(DynamicApiProxy.GetProxyType(type)); //feature.Controllers.Add没什么卵用
                    }
                }
            }

            builder.AddApplicationPart(DynamicApiProxy.DynamicAssembly.AssemblyBuilder);

            builder.PartManager.FeatureProviders.Add(new DynamicApiControllerFeatureProvider());

            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IControllerPropertyActivator, DynamicApiControllerPropertyActivator>());

            return builder;
        }
    }
}
