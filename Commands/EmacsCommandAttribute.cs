using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using System.ComponentModel.Design;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// Allows to specify strongly-typed metadata for Emacs-based commands.
    /// </summary>
    [MetadataAttribute]    
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=false)]
    public class EmacsCommandAttribute : ExportAttribute, IEmacsCommandMetadata
    {
        public EmacsCommandAttribute(EmacsCommandID command)
            : this((int)command, typeof(EmacsCommandID).GUID)
        { }

        public EmacsCommandAttribute(VSConstants.VSStd97CmdID command)
            : this((int)command, typeof(VSConstants.VSStd97CmdID).GUID)
        { }

        public EmacsCommandAttribute(VSConstants.VSStd2KCmdID command)
            : this((int)command, typeof(VSConstants.VSStd2KCmdID).GUID)
        { }

        private EmacsCommandAttribute(int command, Guid commandGroup)
            : base(typeof(EmacsCommand))
        {
            this.Command = command;
            this.CommandGroup = commandGroup.ToString();
            this.CanBeRepeated = false;
        }

        /// <summary>
        /// Gets the Guid of the command group
        /// </summary>
        public string CommandGroup { get; set; }

        /// <summary>
        /// Gets or sets whether the command is kill command. Kill commands contribute to the kill
        /// word list which stores all deleted text in the clipboard.
        /// </summary>
        public bool IsKillCommand { get; set;  }

        /// <summary>
        /// Gets the ID of the command
        /// </summary>
        public int Command { get; private set; }

        /// <summary>
        /// Gets the ID of the inverse command. 
        /// The command may provide declarative the command to be executed when inverse behavior is needed instead of implementing the <see cref="EmacsCommand.ExecuteInverse"/>.
        /// </summary>
        public EmacsCommandID InverseCommand { get; set; }

        /// <summary>
        /// Gets true if the command can be executed multiple times using the universal argument (C-u)
        /// </summary>
        public bool CanBeRepeated { get; set; }

        /// <summary>
        /// Gets name to be displayed in the Undo list. If no name is present, Undo is not available for this command.
        /// </summary>
        public string UndoName { get; set; }
    }
}