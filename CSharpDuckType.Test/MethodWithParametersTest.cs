using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpDuckType.Test
{
    [TestClass]
    public class MethodWithParametersTest
    {
        public interface Foo
        {
            void MethodString(string test);
            void MethodInt(int test);
            void MethodBool(bool test);
            void MethodLong(long test);
        }

        public class Baz
        {
            public void MethodString(string test) { }
            public void MethodInt(int test) { }
            public void MethodBool(bool test) { }
            public void MethodLong(long test) { }
        }

        [TestMethod]
        public void TestMethodParameters()
        {
            Baz baz = new Baz();
            Foo foo = DuckType.Cast<Foo>(baz);
            Assert.AreNotSame(baz, foo);
            foo.MethodString("test");
            foo.MethodInt(1337);
            foo.MethodBool(true);
            foo.MethodLong(1);
        }
    }
}
