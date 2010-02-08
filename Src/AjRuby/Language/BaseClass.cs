namespace AjRuby.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class BaseClass : BaseObject, IClass
    {
        private static BaseClass instance = new BaseClass();

        public BaseClass()
            : base(null)
        {
        }

        public BaseClass(IClass cls)
            : base(cls)
        {
        }

        public static BaseClass Instance { get { return instance; } }

        public override IClass GetClass()
        {
            IClass cls = base.GetClass();

            if (cls == null)
                return instance;

            return cls;
        }

        #region IClass Members

        public virtual object Invoke(object instance, string methodName, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
