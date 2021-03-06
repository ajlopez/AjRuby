﻿namespace AjRuby.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjRuby;
    using AjRuby.Commands;
    using AjRuby.Compiler;
    using AjRuby.Expressions;
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
        public void ParseSimpleUnaryExpression()
        {
            IExpression expression = ParseExpression("-2");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticUnaryExpression));

            ArithmeticUnaryExpression operation = (ArithmeticUnaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Minus, operation.Operation);
            Assert.IsNotNull(operation.Expression);
            Assert.IsInstanceOfType(operation.Expression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseSimpleBinaryExpression()
        {
            IExpression expression = ParseExpression("a + 2");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression operation = (ArithmeticBinaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Add, operation.Operation);
            Assert.IsNotNull(operation.LeftExpression);
            Assert.IsInstanceOfType(operation.LeftExpression, typeof(LocalVariableExpression));
            Assert.IsNotNull(operation.RightExpression);
            Assert.IsInstanceOfType(operation.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseSimpleBinaryExpressionWithParenthesis()
        {
            IExpression expression = ParseExpression("((a) + (2))");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression operation = (ArithmeticBinaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Add, operation.Operation);
            Assert.IsNotNull(operation.LeftExpression);
            Assert.IsInstanceOfType(operation.LeftExpression, typeof(LocalVariableExpression));
            Assert.IsNotNull(operation.RightExpression);
            Assert.IsInstanceOfType(operation.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseTwoBinaryExpression()
        {
            IExpression expression = ParseExpression("a + 2 - 3");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression operation = (ArithmeticBinaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Subtract, operation.Operation);
            Assert.IsNotNull(operation.LeftExpression);
            Assert.IsInstanceOfType(operation.LeftExpression, typeof(ArithmeticBinaryExpression));
            Assert.IsNotNull(operation.RightExpression);
            Assert.IsInstanceOfType(operation.RightExpression, typeof(ConstantExpression));
        }

        [TestMethod]
        public void ParseTwoBinaryExpressionDifferentLevels()
        {
            IExpression expression = ParseExpression("a + 2 * 3");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression arithmeticExpression = (ArithmeticBinaryExpression)expression;

            Assert.AreEqual(ArithmeticOperator.Add, arithmeticExpression.Operation);
            Assert.IsNotNull(arithmeticExpression.LeftExpression);
            Assert.IsInstanceOfType(arithmeticExpression.LeftExpression, typeof(LocalVariableExpression));
            Assert.IsNotNull(arithmeticExpression.RightExpression);
            Assert.IsInstanceOfType(arithmeticExpression.RightExpression, typeof(ArithmeticBinaryExpression));

            ArithmeticBinaryExpression rigthExpression = (ArithmeticBinaryExpression) arithmeticExpression.RightExpression;

            Assert.AreEqual(ArithmeticOperator.Multiply, rigthExpression.Operation);
            Assert.IsInstanceOfType(rigthExpression.LeftExpression, typeof(ConstantExpression));
            Assert.IsInstanceOfType(rigthExpression.RightExpression, typeof(ConstantExpression));
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

        [TestMethod]
        public void ParseSimpleDotExpression()
        {
            IExpression expression = ParseExpression("a.class");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(DotExpression));
        }

        [TestMethod]
        public void ParseSimpleDotExpressionWithArguments()
        {
            IExpression expression = ParseExpression("foo.bar(1,2)");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(DotExpression));
        }

        [TestMethod]
        public void ParseSimpleDotAddExpressions()
        {
            IExpression expression = ParseExpression("foo.bar(1,2)+4");

            Assert.IsNotNull(expression);
            Assert.IsInstanceOfType(expression, typeof(ArithmeticBinaryExpression));
        }

        [TestMethod]
        public void ParseExpressionCommand()
        {
            ICommand command = ParseCommand("foo.bar(1,2);");

            Assert.IsNotNull(command);
            Assert.IsInstanceOfType(command, typeof(ExpressionCommand));

            ExpressionCommand exprcmd = (ExpressionCommand)command;

            Assert.IsNotNull(exprcmd.Expression);
            Assert.IsInstanceOfType(exprcmd.Expression, typeof(DotExpression));
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
