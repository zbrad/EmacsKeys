using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
	/// <summary>
	/// Deletes a char
	/// 
	/// Keys: Delete
	/// </summary>
	[EmacsCommand(VSConstants.VSStd97CmdID.Delete, CanBeRepeated = true, UndoName="Delete")]
	internal class DeleteCommand : EmacsCommand
	{
		internal override void Execute(EmacsCommandContext context)
		{
			context.EditorOperations.Delete();
		}

		internal override void ExecuteInverse(EmacsCommandContext context)
		{
			context.EditorOperations.Backspace();
		}
	}
}