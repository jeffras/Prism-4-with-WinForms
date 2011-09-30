using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.Prism.Commands;

namespace Common
{
    public static class Commands
    {
        public static CompositeCommand SaveAllCommand = new CompositeCommand();
    }

    /// <summary>
    /// Provides a class wrapper around the static SaveAll command.
    /// </summary>
    public class CommandProxy
    {
        public virtual CompositeCommand SaveAllCommand
        {
            get { return Commands.SaveAllCommand; }
        }
    }
}
