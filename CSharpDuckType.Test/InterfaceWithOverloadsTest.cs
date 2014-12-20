using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CSharpDuckType.Test
{
    [TestClass]
    public class InterfaceWithOverloadsTest
    {
        public interface Foo
        {
            void Bar();
            string Bar(string x);
            int Bar(int i);
            bool Bar(bool b);
        }

        public class Baz
        {
            public void Bar() {}
            public string Bar(string x) {
                return x + " World!";
            }
            public int Bar(int i) {
                return i * i;
            }
            public bool Bar(bool b) {
                return !b;
            }
        }

        [TestMethod]
        public void OverloadTest()
        {
            Baz baz = new Baz();
            Foo foo = DuckType.Cast<Foo>(baz);
            Assert.AreNotSame(baz, foo);

            Assert.AreEqual("Hello World!", foo.Bar("Hello"));
            Assert.AreEqual(4, foo.Bar(2));
            Assert.AreEqual(false, foo.Bar(true));
        }
    }
}
