namespace AjRuby.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class SetLocalVariableCommand : ICommand
    {
        private string name;
        private IExpression expression;

        public SetLocalVariableCommand(string name, IExpression expression)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            if (expression == null)
                throw new ArgumentNullException("expression");

            this.name = name;
            this.expression = expression;
        }

        public string VariableName { get { return this.name; } }

        public IExpression Expression { get { return this.expression; } }

        #region ICommand Members

        public void Execute(BindingEnvironment environment)
        {
            environment.SetValue(this.name, this.expression.Evaluate(environment));
        }

        #endregion
    }
}
