namespace AjRuby.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjRuby.Language;

    internal class MockClass : BaseClass
    {
        private static MockClass instance = new MockClass();

        public static new MockClass Instance { get { return instance; } }

        public override object Invoke(object instance, string methodName, params object[] arguments)
        {
            if (arguments == null || arguments.Length == 0)
                return methodName;

            string result = methodName;

            foreach (object argument in arguments)
                result += ":" + (argument == null ? "null" : argument.ToString());

            return result;
        }
    }
}
