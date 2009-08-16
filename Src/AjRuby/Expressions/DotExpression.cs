namespace AjRuby.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjRuby.Language;

    public class DotExpression : IExpression
    {
        private IExpression expression;
        private string name;
        private List<IExpression> arguments;

        public DotExpression(IExpression expression, string name, List<IExpression> arguments)
        {
            this.expression = expression;
            this.name = name;
            this.arguments = arguments;
        }

        public object Evaluate(BindingEnvironment environment)
        {
            object value = this.expression.Evaluate(environment);
            IClass cls = Utilities.GetClass(value);
            object[] argumentValues = null;

            if (this.arguments != null && this.arguments.Count > 0)
            {
                argumentValues = new object[this.arguments.Count];

                for (int k = 0; k < arguments.Count; k++)
                    argumentValues[k] = arguments[k].Evaluate(environment);
            }

            return cls.Invoke(value, this.name, argumentValues);
        }
    }
}
