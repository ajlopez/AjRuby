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
    public class CommandsTests
    {
        [TestMethod]
        public void ExecuteSetLocalVariable()
        {
            BindingEnvironment environment = new BindingEnvironment();

            Assert.IsNull(environment.GetValue("foo"));

            SetLocalVariableCommand cmd = new SetLocalVariableCommand("foo", new ConstantExpression("bar"));

            cmd.Execute(environment);

            Assert.AreEqual("bar", environment.GetValue("foo"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RaiseIfNameIsNullInSetLocalVariableCommand()
        {
            new SetLocalVariableCommand(null, new ConstantExpression(1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RaiseIfExpressionIsNullInSetLocalVariableCommand()
        {
            new SetLocalVariableCommand("foo", null);
        }
    }
}
