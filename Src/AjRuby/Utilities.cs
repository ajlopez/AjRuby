namespace AjRuby
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjRuby.Language;

    public class Utilities
    {
        public static IClass GetClass(object obj)
        {
            if (obj is IObject)
                return ((IObject)obj).GetClass();

            if (obj is int)
                return FixnumClass.Instance;

            throw new InvalidOperationException(string.Format("GetClass is not supported for '{0}'", obj.GetType().FullName));
        }
    }
}
