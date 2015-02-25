using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// Metadata for all Emacs-based commands
    /// </summary>
    public interface IEmacsCommandMetadata
    {
        /// <summary>
        /// Gets the Guid of the Command group
        /// </summary>
        string CommandGroup { get; }

        /// <summary>
        /// Gets the ID of the command
        /// </summary>
        int Command { get; }

        /// <summary>
        /// Gets the ID of the inverse command. 
        /// The command can declarative provide the command to be executed when inverse behavior is needed instead of implementing the <see cref="EmacsCommand.ExecuteInverse"/>.
        /// </summary>
        EmacsCommandID InverseCommand { get; }

        /// <summary>
        /// Gets true if the command can be executed multiple times using the universal argument (C-u)
        /// </summary>
        bool CanBeRepeated { get; }

        /// <summary>
        /// Gets true if the deleted text of the command should be automatically copied to te clipboard
        /// </summary>
        bool IsKillCommand { get; }

        /// <summary>
        /// Gets name to be displayed in the Undo list. If no name is present, Undo is not available for this command.
        /// </summary>
        string UndoName { get; }
    }
}
