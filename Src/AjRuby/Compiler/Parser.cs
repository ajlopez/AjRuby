namespace AjRuby.Compiler
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    using AjRuby;
    using AjRuby.Commands;
    using AjRuby.Language;
    using AjRuby.Expressions;

    public class Parser : IDisposable
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
            return ParseBinaryExpressionFirstLevel();
        }

        private IExpression ParseBinaryExpressionFirstLevel()
        {
            IExpression expression = this.ParseBinaryExpressionSecondLevel();

            if (expression == null)
                return null;

            while (this.TryParse(TokenType.Operator, "+", "-"))
            {
                Token oper = this.lexer.NextToken();
                IExpression right = this.ParseBinaryExpressionSecondLevel();
                ArithmeticOperator op = (oper.Value == "+" ? ArithmeticOperator.Add : ArithmeticOperator.Subtract);

                expression = new ArithmeticBinaryExpression(op, expression, right);
            }

            return expression;
        }

        private IExpression ParseBinaryExpressionSecondLevel()
        {
            IExpression expression = this.ParseUnaryExpression();

            if (expression == null)
                return null;

            while (this.TryParse(TokenType.Operator, "*", "/", @"\"))
            {
                Token oper = this.lexer.NextToken();
                IExpression right = this.ParseUnaryExpression();
                ArithmeticOperator op = (oper.Value == "*" ? ArithmeticOperator.Multiply : (oper.Value == "/" ? ArithmeticOperator.Divide : ArithmeticOperator.IntegerDivide));

                expression = new ArithmeticBinaryExpression(op, expression, right);
            }

            return expression;
        }

        private IExpression ParseUnaryExpression()
        {
            if (this.TryParse(TokenType.Operator, "+", "-"))
            {
                Token oper = this.lexer.NextToken();

                IExpression unaryExpression = this.ParseUnaryExpression();

                ArithmeticOperator op = (oper.Value == "+" ? ArithmeticOperator.Plus : ArithmeticOperator.Minus);

                return new ArithmeticUnaryExpression(op, unaryExpression);
            }

            return this.ParseTermExpression();
        }

        private IExpression ParseTermExpression()
        {
            IExpression expression = this.ParseSimpleTermExpression();

            while (TryParse(TokenType.Operator, "."))
            {
                this.lexer.NextToken();
                string name = this.ParseName();
                List<IExpression> arguments = null;

                if (TryParse(TokenType.Separator, "("))
                    arguments = this.ParseArgumentList();

                expression = new DotExpression(expression, name, arguments);
            }

            return expression;
        }

        private List<IExpression> ParseArgumentList()
        {
            List<IExpression> expressions = new List<IExpression>();

            this.Parse(TokenType.Separator, "(");

            while (!this.TryParse(TokenType.Separator, ")"))
            {
                if (expressions.Count > 0)
                    this.Parse(TokenType.Separator, ",");

                expressions.Add(this.ParseExpression());
            }

            this.Parse(TokenType.Separator, ")");

            return expressions;
        }

        private IExpression ParseSimpleTermExpression()
        {
            Token token = lexer.NextToken();

            if (token == null)
                return null;

            switch (token.TokenType)
            {
                case TokenType.Separator:
                    if (token.Value == "(")
                    {
                        IExpression expr = this.ParseExpression();
                        this.Parse(TokenType.Separator, ")");
                        return expr;
                    }
                    break;
                case TokenType.Boolean:
                    bool booleanValue = Convert.ToBoolean(token.Value);
                    return new ConstantExpression(booleanValue);
                case TokenType.Integer:
                    int intValue = Int32.Parse(token.Value, System.Globalization.CultureInfo.InvariantCulture);
                    return new ConstantExpression(intValue);
                case TokenType.Real:
                    double realValue = Double.Parse(token.Value, System.Globalization.CultureInfo.InvariantCulture);
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
            this.Parse(TokenType.Operator, "=");

            IExpression expression = this.ParseExpression();

            return new SetLocalVariableCommand(variableName, expression);
        }

        private bool TryPeekName()
        {
            Token token = this.lexer.PeekToken();

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

        private bool TryParse(TokenType type, params string[] values)
        {
            Token token = this.lexer.PeekToken();

            if (token == null)
                return false;

            if (type == TokenType.Name)
            {
                foreach (string value in values)
                    if (IsName(token, value))
                        return true;

                return false;
            }

            if (token.TokenType == type)
                foreach (string value in values)
                    if (token.Value == value)
                        return true;

            return false;
        }

        private void Parse(TokenType type, string value)
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

        #region IDisposable Members

        public void Dispose()
        {
            if (this.lexer != null)
                this.lexer.Dispose();
        }

        #endregion
    }
}
