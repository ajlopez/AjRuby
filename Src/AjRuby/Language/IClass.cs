namespace AjRuby.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface IClass : IObject
    {
        object Invoke(object instance, string methodName, params object[] arguments);
    }
}
