using Microsoft.AspNetCore.Mvc.DynamicApi.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Microsoft.AspNetCore.Mvc.DynamicApi.Reflection
{
    static class DynamicApiProxy
    {
        static readonly IDictionary<Type, TypeInfo> _interfaceProxyMap = new Dictionary<Type, TypeInfo>();
        static readonly DynamicAssembly _dynamicAssembly = new DynamicAssembly();
        static readonly string _suffix = Guid.NewGuid().ToString("N").ToUpper();

        public static DynamicAssembly DynamicAssembly => _dynamicAssembly;

        public static TypeInfo GetProxyType(Type interfaceType)
        {
            if (!interfaceType.IsInterface || interfaceType.IsGenericType)
            {
                return null;
            }

            TypeInfo proxyType;
            if (_interfaceProxyMap.TryGetValue(interfaceType, out proxyType))
            {
                return proxyType;
            }
            proxyType = BuildProxyType(interfaceType);

            _interfaceProxyMap.Add(interfaceType, proxyType);

            return proxyType;
        }

        static TypeInfo BuildProxyType(Type interfaceType)
        {
            //Type parentType = typeof(ControllerBase);
            Type parentType = null;
            TypeBuilder typeBuilder = CreateTypeBuilder(GetTypeFullName(interfaceType), parentType);
            typeBuilder.AddInterfaceImplementation(interfaceType);

            FieldBuilder interfaceInstanceFieldBuilder = typeBuilder.DefineField("_interfaceInstance", interfaceType, FieldAttributes.Private);
            #region Constructor
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(
                MethodAttributes.Public,
                CallingConventions.Standard,
                new Type[] { interfaceType });
            ILGenerator constructorIlGenerator = constructorBuilder.GetILGenerator();
            constructorIlGenerator.Emit(OpCodes.Ldarg_0);
            constructorIlGenerator.Emit(OpCodes.Ldarg_1);
            constructorIlGenerator.Emit(OpCodes.Stfld, interfaceInstanceFieldBuilder);
            constructorIlGenerator.Emit(OpCodes.Ret);
            #endregion

            #region IProxyApi
            Type proxyApiType = typeof(IProxyApi<>);
            proxyApiType = proxyApiType.MakeGenericType(interfaceType);
            typeBuilder.AddInterfaceImplementation(proxyApiType);

            #region Service
            //private final hidebysig newslot virtual 
            PropertyBuilder servicePropertyBuilder = typeBuilder.DefineProperty("Service", PropertyAttributes.None, interfaceType, Type.EmptyTypes);
            MethodBuilder servicePropertyGet = typeBuilder.DefineMethod("get_Service", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual, interfaceType, Type.EmptyTypes);
            ILGenerator serviceILGenerator = servicePropertyGet.GetILGenerator();
            serviceILGenerator.Emit(OpCodes.Ldarg_0);
            serviceILGenerator.Emit(OpCodes.Ldfld, interfaceInstanceFieldBuilder);
            serviceILGenerator.Emit(OpCodes.Ret);
            servicePropertyBuilder.SetGetMethod(servicePropertyGet);
            typeBuilder.DefineMethodOverride(servicePropertyGet, proxyApiType.GetProperty("Service").GetGetMethod());
            #endregion

            #region ServiceType
            //private final hidebysig newslot virtual 
            PropertyBuilder serviceTypePropertyBuilder = typeBuilder.DefineProperty("ServiceType", PropertyAttributes.None, typeof(Type), Type.EmptyTypes);
            MethodBuilder serviceTypePropertyGet = typeBuilder.DefineMethod("get_ServiceType", MethodAttributes.Private | MethodAttributes.Final | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.NewSlot | MethodAttributes.Virtual, typeof(Type), Type.EmptyTypes);
            ILGenerator serviceTypeILGenerator = serviceTypePropertyGet.GetILGenerator();
            ReflectionHelper.EmitType(serviceTypeILGenerator, interfaceType);
            serviceTypeILGenerator.Emit(OpCodes.Ret);
            serviceTypePropertyBuilder.SetGetMethod(serviceTypePropertyGet);
            typeBuilder.DefineMethodOverride(serviceTypePropertyGet, proxyApiType.GetProperty("ServiceType").GetGetMethod());
            #endregion

            #endregion



            foreach (var method in interfaceType.GetMethodsIncludingBaseInterfaces())
            {
                BuildMethod(typeBuilder, interfaceType, method, interfaceInstanceFieldBuilder);
            }

            var datas = CustomAttributeData.GetCustomAttributes(interfaceType);
            foreach (var data in datas)
            {
                CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(data.Constructor, data.ConstructorArguments.Select(s => s.Value).ToArray());
                typeBuilder.SetCustomAttribute(customAttributeBuilder);
            }

            return typeBuilder.CreateTypeInfo();
        }

        static void BuildMethod(TypeBuilder typeBuilder, Type interfaceType, MethodInfo method, FieldBuilder interfaceInstanceFieldBuilder)
        {
            MethodAttributes methodAttributes =
                    MethodAttributes.Public
                    | MethodAttributes.HideBySig
                    | MethodAttributes.NewSlot
                    | MethodAttributes.Virtual
                    | MethodAttributes.Final;
            var parameters = method.GetParameters();
            Type[] parameterTypes = parameters.Select(s => s.ParameterType).ToArray();
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(method.Name, methodAttributes, CallingConventions.Standard, method.ReturnType, parameterTypes);

            #region parameterName

            for (int i = 0; i < parameters.Length; i++)
            {
                methodBuilder.DefineParameter(i + 1, ParameterAttributes.None, parameters[i].Name);
            }

            #endregion

            typeBuilder.DefineMethodOverride(methodBuilder, method);
            ILGenerator iLGenerator = methodBuilder.GetILGenerator();
            iLGenerator.Emit(OpCodes.Ldarg_0); // this
            iLGenerator.Emit(OpCodes.Ldfld, interfaceInstanceFieldBuilder);
            for (int i = 0; i < parameterTypes.Length; i++)
            {
                iLGenerator.Emit(OpCodes.Ldarg_S, i + 1);
            }
            iLGenerator.Emit(OpCodes.Call, method);
            iLGenerator.Emit(OpCodes.Ret);
            var datas = CustomAttributeData.GetCustomAttributes(method);
            foreach (var data in datas)
            {
                CustomAttributeBuilder customAttributeBuilder = new CustomAttributeBuilder(data.Constructor, data.ConstructorArguments.Select(s => s.Value).ToArray());
                methodBuilder.SetCustomAttribute(customAttributeBuilder);
            }
        }

        static string GetTypeFullName(Type serviceType)
        {
            return serviceType.FullName + "_ProxyApi_" + _suffix;
        }

        static TypeBuilder CreateTypeBuilder(string typeName, Type parentType)
        {
            return _dynamicAssembly.ModuleBuilder.DefineType(typeName,
                          TypeAttributes.Public |
                          TypeAttributes.Class |
                          TypeAttributes.AutoClass |
                          TypeAttributes.AnsiClass |
                          TypeAttributes.BeforeFieldInit |
                          TypeAttributes.AutoLayout,
                          parentType);
        }

    }
}
