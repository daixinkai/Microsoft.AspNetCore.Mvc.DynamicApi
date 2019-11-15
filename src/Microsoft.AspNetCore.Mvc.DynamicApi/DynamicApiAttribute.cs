using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.DynamicApi
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false, Inherited = true)]
    public class DynamicApiAttribute : Attribute, Microsoft.AspNetCore.Mvc.Routing.IRouteTemplateProvider
    {
        public DynamicApiAttribute() { }
        public DynamicApiAttribute(string template)
        {
            Template = template;
        }
        public string Template { get; set; }
        public int? Order { get; set; }
        public string Name { get; set; }
        public string ControllerName { get; set; }
    }
}
