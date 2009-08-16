namespace AjRuby.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class LocalVariableExpression : IExpression
    {
        private string name;

        public LocalVariableExpression(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.name = name;
        }

        #region IExpression Members

        public object Evaluate(BindingEnvironment environment)
        {
            return environment.GetValue(this.name);
        }

        #endregion
    }
}
