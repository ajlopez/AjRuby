﻿namespace AjRuby.Commands
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public interface ICommand
    {
        void Execute(BindingEnvironment environment);
    }
}
