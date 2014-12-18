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
            return convert<T>(typeof(T), duck);
        }

        private static T convert<T>(Type toType, object duck)
        {
            Type duckType = duck.GetType();
            if (toType.IsAssignableFrom(duckType))
            {
                return (T)duck;
            }

            AssemblyName an = new AssemblyName("DuckType_" + duckType.FullName);
            AssemblyBuilder ab = AppDomain.CurrentDomain.DefineDynamicAssembly(
                an,
                AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder mb = ab.DefineDynamicModule(an.Name);
            TypeBuilder builder = mb.DefineType(duckType.Name + "Ducked");
            builder.AddInterfaceImplementation(toType);

            MethodBuilder meth =  builder.DefineMethod("Bar", MethodAttributes.Public |
                MethodAttributes.Virtual | MethodAttributes.NewSlot | MethodAttributes.Final);
            ILGenerator methIL = meth.GetILGenerator();
            // TODO: Call the duck method
            methIL.Emit(OpCodes.Ret);

            return (T)Activator.CreateInstance(builder.CreateType());
        }
    }
}
