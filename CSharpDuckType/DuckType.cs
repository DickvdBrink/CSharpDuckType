using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace CSharpDuckType
{
    public class DuckType
    {
        public static T Cast<T>(object duck) where T : class
        {
            var toType = typeof(T);
            if (canCast(toType, duck))
            {
                return (T)duck;
            }
            return createProxyImplementation<T>(toType, duck);
        }

        private static bool canCast(Type toType, object duck)
        {
            Type duckType = duck.GetType();
            return toType.IsAssignableFrom(duckType);
        }

        private static T createProxyImplementation<T> (Type toType, object duck)
        {
            var type = createProxyType(toType, duck);
            return (T)Activator.CreateInstance(type, duck);
        }

        private static Type createProxyType(Type toType, object duck)
        {
            Type duckType = duck.GetType();

            AssemblyName an = new AssemblyName("DuckType_" + duckType.FullName + ".dll");
            AssemblyBuilder ab = System.Threading.Thread.GetDomain().DefineDynamicAssembly(
                an,
                AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mb = ab.DefineDynamicModule(an.Name, an.Name + ".dll");
            TypeBuilder typeBuilder = mb.DefineType(duckType.Name + "Ducked",TypeAttributes.Public);
            typeBuilder.AddInterfaceImplementation(toType);

            FieldBuilder privProxyField = typeBuilder.DefineField("priv_proxy", duckType, FieldAttributes.Private);

            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { duckType });
            ILGenerator cbIL = constructorBuilder.GetILGenerator();
            cbIL.Emit(OpCodes.Ldarg_0);
            cbIL.Emit(OpCodes.Ldarg_1);
            cbIL.Emit(OpCodes.Stfld, privProxyField);
            cbIL.Emit(OpCodes.Ret);

            MethodInfo[] methods = toType.GetMethods();

            foreach(var methodInfo in methods)
            {
                var instanceMethod = findMatchingMethod(methodInfo, duckType);
                implementMethod(typeBuilder, privProxyField, methodInfo, instanceMethod);
            }

            //ab.Save(an.Name + ".dll");

            var newType = typeBuilder.CreateType();
            return newType;
        }

        private static MethodInfo findMatchingMethod(MethodInfo method, Type searchType)
        {
            var duckMethodParameters = method.GetParameters();
            var returnType = method.ReturnType;
            var methods = searchType.GetMethods();

            foreach(MethodInfo m in methods)
            {
                // TODO: Check parameter types
                if (m.Name == method.Name &&
                    m.ReturnType == method.ReturnType &&
                    m.GetParameters().Length == duckMethodParameters.Length)
                {
                    return m;
                }
            }
            return null;
        }

        private static void implementMethod(TypeBuilder typeBuilder, FieldInfo privProxyField, MethodInfo interfaceMethod, MethodInfo implementation)
        {
            ParameterInfo[] parameters = interfaceMethod.GetParameters();
            var paramTypes = parameters.Select(x => x.ParameterType).ToArray();
            MethodBuilder meth = typeBuilder.DefineMethod(interfaceMethod.Name, MethodAttributes.Public |
                MethodAttributes.Virtual | MethodAttributes.Final,
                CallingConventions.HasThis, interfaceMethod.ReturnType, paramTypes);
            ILGenerator methIL = meth.GetILGenerator();
            methIL.Emit(OpCodes.Ldarg_0);
            methIL.Emit(OpCodes.Ldfld, privProxyField);
            for(int i = 1; i <= parameters.Length; i++)
            {
                methIL.Emit(OpCodes.Ldarg, i);
            }
            methIL.Emit(OpCodes.Callvirt, implementation);
            methIL.Emit(OpCodes.Ret);
        }
    }
}
