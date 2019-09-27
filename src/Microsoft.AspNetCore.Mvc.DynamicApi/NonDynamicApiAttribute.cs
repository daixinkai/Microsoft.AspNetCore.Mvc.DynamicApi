using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.DynamicApi
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class NonDynamicApiAttribute : Attribute
    {
    }
}
