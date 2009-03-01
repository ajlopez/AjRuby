namespace AjRuby.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjRuby;
    using AjRuby.Compiler;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void ShouldCreateWithString()
        {
            Lexer lexer = new Lexer("text");

            Assert.IsNotNull(lexer);
        }

        [TestMethod]
        public void ShouldCreateWithTextReader()
        {
            Lexer lexer = new Lexer(new StringReader("text"));

            Assert.IsNotNull(lexer);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldRaiseIfTextIsNull()
        {
            Lexer lexer = new Lexer((string)null);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldRaiseIfTextReaderIsNull()
        {
            Lexer lexer = new Lexer((TextReader)null);
        }

        [TestMethod]
        public void ShouldProcessOneCharOperators()
        {
            string operators = "!+~-*/%&<^>|.=";
            Lexer lexer = new Lexer(operators);

            Token token;

            foreach (char ch in operators)
            {
                token = lexer.NextToken();

                Assert.IsNotNull(token);
                Assert.AreEqual(TokenType.Operator, token.TokenType);
                Assert.AreEqual(ch.ToString(), token.Value);
            }

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessMultiCharOperators()
        {
            string operators = "** >> << <= >= == === != =~ !~ <=> && || .. ... **= *= /= %= += -= <<= >>= &&= &= ||= |= ^=";
            string[] otherOperators = new string[] { "**", ">>", "<<", "<=", ">=", "==", "===", "!=", "=~", "!~", "<=>", "&&", "||", "..", "...", "**=", "*=", "/=", "%=", "+=", "-=", "<<=", ">>=", "&&=", "&=", "||=", "|=", "^=" };

            Lexer lexer = new Lexer(operators);

            Token token;

            foreach (string op in otherOperators)
            {
                token = lexer.NextToken();

                Assert.IsNotNull(token);
                Assert.AreEqual(TokenType.Operator, token.TokenType);
                Assert.AreEqual(op, token.Value);
            }

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessSeparators()
        {
            string separators = "()[]{},:;";
            Lexer lexer = new Lexer(separators);

            Token token;

            foreach (char ch in separators)
            {
                token = lexer.NextToken();

                Assert.IsNotNull(token);
                Assert.AreEqual(TokenType.Separator, token.TokenType);
                Assert.AreEqual(ch.ToString(), token.Value);
            }

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessName()
        {
            Lexer lexer = new Lexer("name");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.TokenType);
            Assert.AreEqual("name", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessNameWithInitialUnderscore()
        {
            Lexer lexer = new Lexer("_name");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.TokenType);
            Assert.AreEqual("_name", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessNameWithUnderscore()
        {
            Lexer lexer = new Lexer("my_name");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.TokenType);
            Assert.AreEqual("my_name", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessInstanceVariableName()
        {
            Lexer lexer = new Lexer("@name");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.TokenType);
            Assert.AreEqual("@name", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessClassVariableName()
        {
            Lexer lexer = new Lexer("@@name");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.TokenType);
            Assert.AreEqual("@@name", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessGlobalVariableName()
        {
            Lexer lexer = new Lexer("$name");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.TokenType);
            Assert.AreEqual("$name", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidInputException))]
        public void ShouldRaiseIfOnlyAts()
        {
            Lexer lexer = new Lexer("@@");

            Token token;

            token = lexer.NextToken();
        }

        [TestMethod]
        public void ShouldProcessNameWithSpaces()
        {
            Lexer lexer = new Lexer(" name ");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Name, token.TokenType);
            Assert.AreEqual("name", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessInteger()
        {
            Lexer lexer = new Lexer("123");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Integer, token.TokenType);
            Assert.AreEqual("123", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessIntegerWithSpaces()
        {
            Lexer lexer = new Lexer(" 123 ");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Integer, token.TokenType);
            Assert.AreEqual("123", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessReal()
        {
            Lexer lexer = new Lexer("12.34");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Real, token.TokenType);
            Assert.AreEqual("12.34", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessBoolean()
        {
            Lexer lexer = new Lexer("true false");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Boolean, token.TokenType);
            Assert.AreEqual("true", token.Value);

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.Boolean, token.TokenType);
            Assert.AreEqual("false", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessString()
        {
            Lexer lexer = new Lexer("\"foo\"");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.TokenType);
            Assert.AreEqual("foo", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessStringWithBackslashes()
        {
            Lexer lexer = new Lexer("\"\\\\\\r\\n\\t\\f\\s\\v\\\\\"");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.TokenType);
            Assert.AreEqual("\\\r\n\t\f \v\\", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfInputException))]
        public void ShouldRaiseIfUnclosedString()
        {
            Lexer lexer = new Lexer("\"foo");

            Token token;

            token = lexer.NextToken();
        }

        [TestMethod]
        public void ShouldProcessMultilineString()
        {
            Lexer lexer = new Lexer("\"foo\r\nbar\"");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.TokenType);
            Assert.AreEqual("foo\r\nbar", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessQuotedString()
        {
            Lexer lexer = new Lexer("'bar'");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.TokenType);
            Assert.AreEqual("bar", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        [ExpectedException(typeof(EndOfInputException))]
        public void ShouldRaiseIfUnclosedQuotedString()
        {
            Lexer lexer = new Lexer("'bar");

            Token token;

            token = lexer.NextToken();
        }

        [TestMethod]
        public void ShouldProcessQuotedStringWithQuote()
        {
            Lexer lexer = new Lexer("'bar\\'foo'");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.TokenType);
            Assert.AreEqual("bar'foo", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessQuotedStringWithBackslash()
        {
            Lexer lexer = new Lexer("'bar\\\\foo'");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.TokenType);
            Assert.AreEqual("bar\\foo", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessQuotedStringWithSuperfluosBackslash()
        {
            Lexer lexer = new Lexer("'bar\\foo'");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.TokenType);
            Assert.AreEqual("bar\\foo", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessMultilineQuotedString()
        {
            Lexer lexer = new Lexer("'bar\r\nfoo'");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.TokenType);
            Assert.AreEqual("bar\r\nfoo", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }

        [TestMethod]
        public void ShouldProcessMultilineQuotedStringWithBackslash()
        {
            Lexer lexer = new Lexer("'bar\\\r\nfoo'");

            Token token;

            token = lexer.NextToken();

            Assert.IsNotNull(token);
            Assert.AreEqual(TokenType.String, token.TokenType);
            Assert.AreEqual("bar\\\r\nfoo", token.Value);

            token = lexer.NextToken();

            Assert.IsNull(token);
        }
    }
}

