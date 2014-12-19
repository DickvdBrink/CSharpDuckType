using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpDuckType.Test
{
    [TestClass]
    public class MethodReturnValueTest
    {

        public interface Foo
        {
            bool GetBool();
            int GetInt();
            string GetString();
        }

        public class Baz
        {
            public bool GetBool() { return true; }
            public int GetInt() { return 3; }
            public string GetString() { return "Return value from GetString"; }
        }

        private Foo foo;

        [TestInitialize()]
        public void Initialize()
        {
            Baz baz = new Baz();
            foo = DuckType.Cast<Foo>(baz);
            Assert.AreNotSame(baz, foo);
        }


        [TestMethod]
        public void TestBool()
        {
            var value = foo.GetBool();
            Assert.AreEqual(true, value);
        }

        [TestMethod]
        public void TestInt()
        {
            var value = foo.GetInt();
            Assert.AreEqual(3, value);
        }

        [TestMethod]
        public void TestString()
        {
            var value = foo.GetString();
            Assert.AreEqual("Return value from GetString", value);
        }
    }
}
