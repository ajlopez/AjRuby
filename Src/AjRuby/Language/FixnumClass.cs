namespace AjRuby.Language
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class FixnumClass : BaseClass
    {
        private static FixnumClass instance = new FixnumClass();

        private FixnumClass()
        {
        }

        public static new FixnumClass Instance { get { return instance; } }
    }
}
