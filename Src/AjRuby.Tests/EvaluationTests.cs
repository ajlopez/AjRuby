namespace AjRuby.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using AjRuby;
    using AjRuby.Expressions;
    using AjRuby.Language;
    using AjRuby.Compiler;
    using AjRuby.Tests.Mocks;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class EvaluationTests
    {
        private BindingEnvironment environment;

        [TestInitialize]
        public void SetupEnvironment()
        {
            this.environment = new BindingEnvironment();
            this.environment.SetValue("one", 1);
            this.environment.SetValue("two", 2);
            this.environment.SetValue("foo", "bar");
            this.environment.SetValue("mock", new MockObject());
        }

        [TestMethod]
        public void EvaluateSimpleConstants()
        {
            Assert.AreEqual(1, this.Evaluate("1"));
            Assert.AreEqual(2, this.Evaluate("2"));
            Assert.AreEqual("bar", this.Evaluate("\"bar\""));
        }

        [TestMethod]
        public void EvaluateLocalVariables()
        {
            Assert.AreEqual(1, this.Evaluate("one"));
            Assert.AreEqual(2, this.Evaluate("two"));
            Assert.AreEqual("bar", this.Evaluate("foo"));
        }

        [TestMethod]
        public void EvaluateSimpleAddUsingConstants()
        {
            Assert.AreEqual(1, this.Evaluate("1+0"));
            Assert.AreEqual(2, this.Evaluate("1+1"));
            Assert.AreEqual(0, this.Evaluate("-1+1"));
            Assert.AreEqual(3, this.Evaluate("1 + 2"));
        }

        [TestMethod]
        public void EvaluateSimpleSubtractUsingConstants()
        {
            Assert.AreEqual(1, this.Evaluate("1-0"));
            Assert.AreEqual(0, this.Evaluate("1-1"));
            Assert.AreEqual(-2, this.Evaluate("-1-1"));
            Assert.AreEqual(-1, this.Evaluate("1 - 2"));
        }

        [TestMethod]
        public void EvaluateArithmeticUsingConstants()
        {
            Assert.AreEqual(4, this.Evaluate("1+1+2"));
            Assert.AreEqual(4, this.Evaluate("1-1+4"));
            Assert.AreEqual(2, this.Evaluate("-1-1+4"));
            Assert.AreEqual(4, this.Evaluate("1 - 2 + 5"));
        }

        [TestMethod]
        public void EvaluateArithmeticUsingConstantsAndMultipleLevels()
        {
            Assert.AreEqual(7, this.Evaluate("1+3*2"));
            Assert.AreEqual(5.0, this.Evaluate("1/1+4"));
            Assert.AreEqual(5, this.Evaluate(@"1\1+4"));
            Assert.AreEqual(5, this.Evaluate("-1*-1+4"));
            Assert.AreEqual(-9, this.Evaluate("1 - 2 * 5"));
        }

        [TestMethod]
        public void EvaluateMockInvocation()
        {
            Assert.AreEqual("foo", this.Evaluate("mock.foo"));
            Assert.AreEqual("foo", this.Evaluate("mock.foo()"));
            Assert.AreEqual("foo:1:2", this.Evaluate("mock.foo(1,2)"));
            Assert.AreEqual("foo:1:2", this.Evaluate("mock.foo(one,two)"));
        }

        private object Evaluate(string text) 
        {
            using (Parser parser = new Parser(text))
            {
                IExpression expression = parser.ParseExpression();
                Assert.IsNull(parser.ParseExpression());
                return expression.Evaluate(this.environment);
            }
        }
    }
}
