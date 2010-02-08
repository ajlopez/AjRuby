namespace AjRuby.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjRuby.Expressions;

    public class ExpressionCommand : ICommand
    {
        private IExpression expression;

        public ExpressionCommand(IExpression expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            this.expression = expression;
        }

        public IExpression Expression { get { return this.expression; } }

        public void Execute(BindingEnvironment environment)
        {
            this.expression.Evaluate(environment);
        }
    }
}
