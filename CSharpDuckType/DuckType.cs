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
            return convert<T>(toType, duck);
        }

        private static bool canCast(Type toType, object duck)
        {
            Type duckType = duck.GetType();
            return toType.IsAssignableFrom(duckType);
        }

        private static T convert<T>(Type toType, object duck)
        {
            Type duckType = duck.GetType();

            AssemblyName an = new AssemblyName("DuckType_" + duckType.FullName + ".dll");
            AssemblyBuilder ab = System.Threading.Thread.GetDomain().DefineDynamicAssembly(
                an,
                AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mb = ab.DefineDynamicModule(an.Name, an.Name + ".dll");
            TypeBuilder tp = mb.DefineType(duckType.Name + "Ducked",TypeAttributes.Public);
            tp.AddInterfaceImplementation(toType);

            FieldBuilder fb = tp.DefineField("priv_proxy", duckType, FieldAttributes.Private);

            ConstructorBuilder cb = tp.DefineConstructor(MethodAttributes.Public, CallingConventions.HasThis, new Type[] { duckType });
            ILGenerator cbIL = cb.GetILGenerator();
            cbIL.Emit(OpCodes.Ldarg_0);
            cbIL.Emit(OpCodes.Ldarg_1);
            cbIL.Emit(OpCodes.Stfld, fb);
            cbIL.Emit(OpCodes.Ret);

            MethodInfo[] methods = toType.GetMethods();

            foreach(var methodInfo in methods)
            {
                var instanceMethod = findMatchingMethod(methodInfo, duckType);

                implementMethod(tp, fb, methodInfo, instanceMethod);
            }
            var newType = tp.CreateType();
            //ab.Save(an.Name + ".dll");
            return (T)Activator.CreateInstance(newType, duck);
        }

        private static MethodInfo findMatchingMethod(MethodInfo method, Type searchType)
        {
            var methods = searchType.GetMethods();
            foreach(MethodInfo m in methods)
            {
                if (m.Name == method.Name)
                {
                    return m;
                }
            }
            return null;
        }

        private static void implementMethod(TypeBuilder tp, FieldInfo f, MethodInfo interfaceMethod, MethodInfo implementation)
        {
            ParameterInfo[] parameters = interfaceMethod.GetParameters();
            var paramTypes = parameters.Select(x => x.ParameterType).ToArray();
            MethodBuilder meth = tp.DefineMethod(interfaceMethod.Name, MethodAttributes.Public |
                MethodAttributes.Virtual | MethodAttributes.Final,
                CallingConventions.HasThis, interfaceMethod.ReturnType, paramTypes);
            ILGenerator methIL = meth.GetILGenerator();
            methIL.Emit(OpCodes.Ldarg_0);
            methIL.Emit(OpCodes.Ldfld, f);
            for(int i = 1; i <= parameters.Length; i++)
            {
                methIL.Emit(OpCodes.Ldarg, i);
            }
            methIL.Emit(OpCodes.Callvirt, implementation);
            methIL.Emit(OpCodes.Ret);
        }
    }
}
