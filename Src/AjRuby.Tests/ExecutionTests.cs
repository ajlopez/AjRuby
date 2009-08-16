namespace AjRuby.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using AjRuby;
    using AjRuby.Commands;
    using AjRuby.Compiler;
    using AjRuby.Language;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ExecutionTests
    {
        private BindingEnvironment environment;

        [TestInitialize]
        public void SetupEnvironment()
        {
            this.environment = new BindingEnvironment();
            this.environment.SetValue("one", 1);
            this.environment.SetValue("two", 2);
            this.environment.SetValue("foo", "bar");
        }

        [TestMethod]
        public void SimpleAssignment()
        {
            this.Execute("a = 1");
            Assert.AreEqual(1, this.environment.GetValue("a"));
        }

        [TestMethod]
        public void SimpleExpressionAssignment()
        {
            this.Execute("a = 1 + 3");
            Assert.AreEqual(4, this.environment.GetValue("a"));
        }

        private void Execute(string text)
        {
            using (Parser parser = new Parser(text))
            {
                ICommand command = parser.ParseCommand();
                Assert.IsNull(parser.ParseCommand());
                command.Execute(this.environment);
            }
        }
    }
}
