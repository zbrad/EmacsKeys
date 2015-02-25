using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using System.ComponentModel.Design;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Operations;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// Base class for all Emacs-based commands
    /// </summary>
    internal abstract class EmacsCommand
    {
        /// <summary>
        /// Implemenation of the command
        /// </summary>
        /// <remarks>Try to use public editor APIs over DTE and other APIs</remarks>
        /// <param name="context"></param>
        internal abstract void Execute(EmacsCommandContext context);

        /// <summary>
        /// Inverse implementation of the command. This methods should be overriden by commands that supports to be repeated.
        /// </summary>
        /// <remarks>Try to use public editor APIs over DTE and other APIs</remarks>
        /// <param name="context"></param>
        internal virtual void ExecuteInverse(EmacsCommandContext context)
        {
            throw new NotImplementedException();
        }
    }
}
