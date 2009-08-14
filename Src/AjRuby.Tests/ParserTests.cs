namespace AjRuby.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjRuby;
    using AjRuby.Compiler;
    using AjRuby.Language;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ParserTests
    {
        [TestMethod]
        public void ParseConstantExpressions()
        {
            IExpression expression;

            expression = ParseExpression("1");
            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ConstantExpression));
            Assert.AreEqual(1, expression.Evaluate(null));

            expression = ParseExpression("1.2");
            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ConstantExpression));
            Assert.AreEqual(1.2, expression.Evaluate(null));

            expression = ParseExpression("false");
            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ConstantExpression));
            Assert.IsFalse((bool)expression.Evaluate(null));

            expression = ParseExpression("\"foo\"");
            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ConstantExpression));
            Assert.AreEqual("foo", expression.Evaluate(null));

            Assert.IsNull(ParseExpression(""));
        }

        [TestMethod]
        public void ParseSetLocalVariableCommand()
        {
            ICommand command = ParseCommand("a = 1");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(SetLocalVariableCommand));

            SetLocalVariableCommand setcmd = (SetLocalVariableCommand)command;

            Assert.AreEqual("a", setcmd.VariableName);
            Assert.IsNotNull(setcmd.Expression);
            Assert.IsInstanceOfType(setcmd.Expression, typeof(ConstantExpression));
            Assert.AreEqual(1, setcmd.Expression.Evaluate(null));
        }

        private static IExpression ParseExpression(string text)
        {
            Parser parser = new Parser(text);

            IExpression expression = parser.ParseExpression();

            Assert.IsNull(parser.ParseExpression());

            return expression;
        }

        private static ICommand ParseCommand(string text)
        {
            Parser parser = new Parser(text);

            ICommand command = parser.ParseCommand();

            Assert.IsNull(parser.ParseExpression());

            return command;
        }
    }
}
