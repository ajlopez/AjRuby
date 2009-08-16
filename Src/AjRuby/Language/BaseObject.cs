using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AjRuby.Language
{
    public class BaseObject : IObject
    {
        private IClass objectClass;

        public BaseObject(IClass cls)
        {
            this.objectClass = cls;
        }

        public virtual IClass GetClass()
        {
            return this.objectClass;
        }
    }
}
