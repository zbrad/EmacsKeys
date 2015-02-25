using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Formatting;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using Microsoft.VisualStudio.Shell.Interop;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
	/// <summary>
	/// 
	/// Keys: Ctrl+X, S
	/// </summary>
	[EmacsCommand(EmacsCommandID.FileSaveDirty)]
	internal class SaveFileDirtyCommand : EmacsCommand
	{
		internal override void Execute(EmacsCommandContext context)
		{
			var rdt = context.Manager.ServiceProvider.GetService<SVsRunningDocumentTable, IVsRunningDocumentTable>();
			if (rdt != null)
			{
				rdt.SaveDocuments((uint)__VSRDTSAVEOPTIONS.RDTSAVEOPT_PromptSave, null, (uint)VSConstants.VSITEMID.Root, 0);
			}
		}
	}
}
