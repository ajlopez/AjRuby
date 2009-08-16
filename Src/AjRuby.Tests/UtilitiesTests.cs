namespace AjRuby.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using AjRuby;
    using AjRuby.Language;
    using AjRuby.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class UtilitiesTests
    {
        [TestMethod]
        public void GetClassOnMockObject()
        {
            IObject obj = new MockObject();

            IClass cls = Utilities.GetClass(obj);

            Assert.IsNotNull(cls);
            Assert.IsInstanceOfType(cls, typeof(MockClass));
        }

        [TestMethod]
        public void GetFixnumClassOnInteger()
        {
            IClass cls = Utilities.GetClass(1);

            Assert.IsNotNull(cls);
            Assert.IsInstanceOfType(cls, typeof(FixnumClass));
        }
    }
}
