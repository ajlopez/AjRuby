namespace AjRuby.Tests.Mocks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AjRuby.Language;

    internal class MockObject : BaseObject
    {
        internal MockObject() 
            : base(MockClass.Instance)
        {
        }
    }
}
