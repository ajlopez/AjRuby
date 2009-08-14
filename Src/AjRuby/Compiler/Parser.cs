namespace AjRuby.Compiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjRuby;
    using AjRuby.Language;

    public class Parser
    {
        private Lexer lexer;

        public Parser(string text)
            : this(new Lexer(text))
        {
        }

        public Parser(TextReader reader)
            : this(new Lexer(reader))
        {
        }

        public Parser(Lexer lexer)
        {
            this.lexer = lexer;
        }

        public ICommand ParseCommand()
        {
            Token token = lexer.NextToken();

            if (token == null)
                return null;

            if (token.TokenType == TokenType.Name)
                return ParseSetCommand(token.Value);

            throw new UnexpectedTokenException(token);
        }

        public IExpression ParseExpression()
        {
            Token token = lexer.NextToken();

            if (token == null)
                return null;

            switch (token.TokenType) 
            {
                case TokenType.Boolean:
                    bool booleanValue = Convert.ToBoolean(token.Value);
                    return new ConstantExpression(booleanValue);
                case TokenType.Integer:
                    int intValue = Convert.ToInt32(token.Value);
                    return new ConstantExpression(intValue);
                case TokenType.Real:
                    double realValue = Convert.ToDouble(token.Value);
                    return new ConstantExpression(realValue);
                case TokenType.String:
                    return new ConstantExpression(token.Value);
                case TokenType.Name:
                    if (token.Value[0] != '@' && token.Value[0] != '$')
                        return new LocalVariableExpression(token.Value);
                    break;
            }

            throw new UnexpectedTokenException(token);
        }

        private ICommand ParseSetCommand(string variableName)
        {
            this.Parse("=", TokenType.Operator);

            IExpression expression = this.ParseExpression();

            return new SetLocalVariableCommand(variableName, expression);
        }

        private bool TryPeekName()
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                return false;

            this.lexer.PushToken(token);

            return token.TokenType == TokenType.Name;
        }

        private object ParseValue()
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                throw new UnexpectedEndOfInputException();

            if (token.TokenType == TokenType.String)
                return token.Value;

            if (token.TokenType == TokenType.Integer)
                return int.Parse(token.Value);

            throw new UnexpectedTokenException(token);
        }

        private bool TryParse(string value, TokenType type)
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                return false;

            if (type == TokenType.Name)
            {
                if (IsName(token, value))
                    return true;
                else
                {
                    this.lexer.PushToken(token);
                    return false;
                }
            }

            if (token.TokenType == type && token.Value == value)
                return true;

            this.lexer.PushToken(token);
            return false;
        }

        private void Parse(string value, TokenType type)
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                throw new UnexpectedEndOfInputException();

            if (type == TokenType.Name)
                if (IsName(token, value))
                    return;
                else
                    throw new UnexpectedTokenException(token);

            if (token.TokenType != type || token.Value != value)
                throw new UnexpectedTokenException(token);
        }

        private string ParseName()
        {
            Token token = this.lexer.NextToken();

            if (token == null)
                throw new ParserException(string.Format("Unexpected end of input"));

            if (token.TokenType == TokenType.Name)
                return token.Value;

            throw new UnexpectedTokenException(token);
        }

        private static bool IsName(Token token, string value)
        {
            return IsToken(token, value, TokenType.Name);
        }

        private static bool IsToken(Token token, string value, TokenType type)
        {
            if (token == null)
                return false;

            if (token.TokenType != type)
                return false;

            if (type == TokenType.Name)
                return token.Value.Equals(value, StringComparison.InvariantCultureIgnoreCase);

            return token.Value.Equals(value);
        }
    }
}
