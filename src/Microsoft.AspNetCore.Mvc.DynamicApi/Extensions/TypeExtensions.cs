using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.DynamicApi
{
    static class TypeExtensions
    {
        public static bool IsProxyApi(this Type type)
        {
            return !type.IsInterface && typeof(IProxyApi).IsAssignableFrom(type);
        }

        public static bool IsProxyApi(this object obj)
        {
            return obj is IProxyApi;
        }

        public static Type GetProxyApiType(this Type type)
        {
            return type.GetInterfaces()[0];
        }

        public static T GetCustomAttributeIncludingBaseInterfaces<T>(this Type type) where T : Attribute
        {
            T attribute = type.GetCustomAttribute<T>();
            if (attribute != null)
            {
                return attribute;
            }
            return GetCustomAttributeFromBaseInterfaces<T>(type);
        }

        static T GetCustomAttributeFromBaseInterfaces<T>(this Type type) where T : Attribute
        {
            T attribute = null;
            foreach (var item in type.GetInterfaces())
            {
                attribute = item.GetCustomAttribute<T>();
                if (attribute != null)
                {
                    return attribute;
                }
            }
            foreach (var item in type.GetInterfaces())
            {
                attribute = GetCustomAttributeFromBaseInterfaces<T>(item);
                if (attribute != null)
                {
                    return attribute;
                }
            }
            return null;
        }


        public static bool IsDefinedIncludingBaseInterfaces<T>(this Type type)
        {
            return type.IsDefined(typeof(T)) || type.GetInterfaces().Any(s => IsDefinedIncludingBaseInterfaces<T>(s));
        }

        public static MethodInfo[] GetMethodsIncludingBaseInterfaces(this Type type)
        {
            List<MethodInfo> methods = new List<MethodInfo>(type.GetMethods());
            GetMethodsFromBaseInterfaces(type, methods);
            return methods.ToArray();
        }

        static void GetMethodsFromBaseInterfaces(this Type type, List<MethodInfo> methods)
        {
            foreach (var item in type.GetInterfaces())
            {
                foreach (var method in item.GetMethods())
                {
                    if (!methods.Contains(method))
                    {
                        methods.Add(method);
                    }
                }
            }
            foreach (var item in type.GetInterfaces())
            {
                GetMethodsFromBaseInterfaces(item, methods);
            }
        }
    }
}
