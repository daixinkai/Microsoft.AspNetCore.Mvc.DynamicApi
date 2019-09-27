using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.DynamicApi.Internal
{
    static class ReflectionHelper
    {
        static readonly MethodInfo GetTypeFromHandleMethodInfo = typeof(Type).GetMethod("GetTypeFromHandle");

        public static void EmitType(ILGenerator iLGenerator, Type type)
        {
            iLGenerator.Emit(OpCodes.Ldtoken, type);
            iLGenerator.Emit(OpCodes.Call, GetTypeFromHandleMethodInfo);
        }
    }
}
