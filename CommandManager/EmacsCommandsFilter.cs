using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Editor.EmacsEmulation.Commands;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    /// <summary>
    /// Custom filter to have a chance to control the status (enabled|disabled|etc) and execution of the Emacs commands
    /// </summary>
    internal class EmacsCommandsFilter : IOleCommandTarget
    {
        ITextView view;
        EmacsCommandsManager manager;
        CommandRouter router;

        public EmacsCommandsFilter(ITextView view, EmacsCommandsManager manager, CommandRouter router)
        {
            this.view = view;
            this.manager = manager;
            this.router = router;
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (this.manager.IsEnabled)
            {
                var command = this.manager.GetCommandMetadata((int)nCmdID, pguidCmdGroup);

                if (command != null)
                {
                    try
                    {
                        // we did find a match so we execute the corresponding command
                        this.manager.Execute(this.view, command);
                        manager.AfterSearch = false;
                    }
                    catch (Exception ex)
                    {
                        this.manager.UpdateStatus(ex.Message);
                        return VSConstants.S_FALSE;
                    }

                    // return S_OK to signal we successfully handled the command
                    return VSConstants.S_OK;
                }
                else
                {
                    // The command was not a command understood by the command manager and therefore not a kill command
                    // so we need to flush any accumulated kill string
                    if (this.IsKillwordFlushCommand(pguidCmdGroup, nCmdID))
                    {
                        view.FlushKillSring(manager.ClipboardRing);
                    }

                    // Check if we should insert chars multiple times
                    if (pguidCmdGroup == VSConstants.VSStd2K &&
                        nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR &&
                        this.manager.UniversalArgument.HasValue &&
                        this.manager.UniversalArgument.Value > 1)
                    {
                        var count = this.manager.UniversalArgument.Value;
                        while (count-- > 0)
                        {
                            var result = router.ExecuteCommand(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);

                            if (result != VSConstants.S_OK)
                                return result;
                        }

                        return VSConstants.S_OK;
                    }
                    else if (pguidCmdGroup == VSConstants.VSStd2K &&
                        (nCmdID == (uint)VSConstants.VSStd2KCmdID.ISEARCH || nCmdID == (uint)VSConstants.VSStd2KCmdID.ISEARCHBACK))
                    {
                        MarkSession.GetSession(view).PushMark();
                        manager.AfterSearch = true;
                    }
                }
            }

            // if there is no match just pass the command along to the next registered filter
            return VSConstants.S_FALSE;
        }

        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (this.manager.IsEnabled)
            {
                if (pguidCmdGroup == typeof(EmacsCommandID).GUID && cCmds > 0)
                {
                    var command = this.manager.GetCommandMetadata((int)prgCmds[0].cmdID, pguidCmdGroup);
                    if (command != null)
                    {
                        prgCmds[0].cmdf = (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDF_ENABLED | (int)Microsoft.VisualStudio.OLE.Interop.Constants.MSOCMDF_SUPPORTED;
                        return VSConstants.S_OK;
                    }
                }
            }

            return VSConstants.S_FALSE;
        }

        /// <summary>
        /// Returns true for commands that would cause the currently accumulated killed words to be flushed to 
        /// the clipboard.
        /// </summary>
        private bool IsKillwordFlushCommand(Guid pguidCmdGroup, uint nCmdID)
        {
            if (manager.IsEnabled)
            {
                if (pguidCmdGroup == typeof(EmacsCommandID).GUID)
                {
                    if (nCmdID == (int)EmacsCommandID.DocumentEnd || nCmdID == (int)EmacsCommandID.DocumentStart)
                    {
                        return true;
                    }
                }

                if (pguidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID)
                {
                    if (nCmdID == (uint)VSConstants.VSStd97CmdID.Move)
                    {
                        return true;
                    }
                }

                if (pguidCmdGroup == typeof(VSConstants.VSStd2KCmdID).GUID)
                {
                    if (nCmdID == (int)VSConstants.VSStd2KCmdID.BACKSPACE || nCmdID == (int)VSConstants.VSStd2KCmdID.DELETE
                            || nCmdID == (int)VSConstants.VSStd2KCmdID.UP || nCmdID == (int)VSConstants.VSStd2KCmdID.DOWN
                            || nCmdID == (int)VSConstants.VSStd2KCmdID.PAGEDN || nCmdID == (int)VSConstants.VSStd2KCmdID.PAGEUP
                            || nCmdID == (int)VSConstants.VSStd2KCmdID.LEFT || nCmdID == (int)VSConstants.VSStd2KCmdID.RIGHT
                            || nCmdID == (int)VSConstants.VSStd2KCmdID.TYPECHAR)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
