using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpDuckType.Test
{
    [TestClass]
    public class CastToImplementedInterfaceTest
    {
        interface Foo
        {
            void Bar();
        }
        class Baz : Foo
        {
            public void Bar()
            {
            }
        }

        [TestMethod]
        public void CastToImplementedType()
        {
            Baz baz = new Baz();
            Foo foo = DuckType.Cast<Foo>(baz);
            Assert.AreSame(baz, foo);
            foo.Bar();
        }
    }
}
