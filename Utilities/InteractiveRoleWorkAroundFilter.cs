using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor.EmacsEmulation.Commands;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    class InteractiveRoleWorkAroundFilter : IOleCommandTarget
    {
        ITextView view;
        EmacsCommandsManager manager;

        public InteractiveRoleWorkAroundFilter(ITextView view, EmacsCommandsManager manager)
        {
            this.view = view;
            this.manager = manager;
        }

        internal IOleCommandTarget NextCommandTarget { get; set; }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == typeof(EmacsCommandID).GUID && nCmdID == (uint)EmacsCommandID.BreakLine)
            {
                var guid = typeof(VSConstants.VSStd2KCmdID).GUID;
                return this.NextCommandTarget.Exec(ref guid, (uint)VSConstants.VSStd2KCmdID.RETURN, nCmdexecopt, pvaIn, pvaOut);
            }

            // if there is no match just pass the command along to the next registered filter
            return this.NextCommandTarget.Exec(pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == typeof(EmacsCommandID).GUID && cCmds > 0 && prgCmds[0].cmdID == (uint)EmacsCommandID.BreakLine)
            {
                prgCmds[0].cmdf = (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDF_ENABLED | (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDF_SUPPORTED;
                return VSConstants.S_OK;
            }

            return this.NextCommandTarget.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
    }
}
