using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpDuckType.Test
{
    [TestClass]
    public class MethodNoParameterTest
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
        public void EmptyMethodNoReturnType()
        {
            Baz baz = new Baz();
            Foo foo = DuckType.Cast<Foo>(baz);
            Assert.AreNotSame(baz, foo);
            foo.Bar();
        }
    }
}
