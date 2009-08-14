namespace AjRuby.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using AjRuby;
    using AjRuby.Language;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExpressionTests
    {
        [TestMethod]
        public void EvaluateConstantExpressions()
        {
            Assert.AreEqual(1, (new ConstantExpression(1)).Evaluate(null));
            Assert.AreEqual("foo", (new ConstantExpression("foo")).Evaluate(null));
            Assert.IsNull((new ConstantExpression(null)).Evaluate(null));
        }

        [TestMethod]
        public void EvaluateLocalVariableExpressions()
        {
            BindingEnvironment environment = new BindingEnvironment();

            environment.SetValue("foo", "bar");
            environment.SetValue("one", 1);

            LocalVariableExpression fooVar = new LocalVariableExpression("foo");
            LocalVariableExpression oneVar = new LocalVariableExpression("one");
            LocalVariableExpression twoVar = new LocalVariableExpression("two");

            Assert.AreEqual("bar", fooVar.Evaluate(environment));
            Assert.AreEqual(1, oneVar.Evaluate(environment));
            Assert.IsNull(twoVar.Evaluate(environment));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RaiseIfNullNameInLocalVariableExpression()
        {
            new LocalVariableExpression(null);
        }
    }
}
