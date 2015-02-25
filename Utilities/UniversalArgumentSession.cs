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
    internal class UniversalArgumentSession : IOleCommandTarget
    {
        private const char NegativeArgumentSign = '-';
        private const int DefaultUniversalValue = 4;

        ITextView view;
        EmacsCommandsManager manager;

        StringBuilder universalArgumentString;

        internal UniversalArgumentSession(ITextView view, EmacsCommandsManager manager)
        {
            this.view = view;
            this.manager = manager;
        }

        private bool IsActive { get; set; }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (this.manager.IsEnabled)
            {
                if (this.IsActive)
                {
                    if (pguidCmdGroup == VSConstants.VSStd2K)
                    {
                        if ((nCmdID == (uint)VSConstants.VSStd2KCmdID.TYPECHAR))
                        {
                            // A char has been typed
                            char typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);

                            if ((typedChar == NegativeArgumentSign && universalArgumentString.Length == 0) || char.IsNumber(typedChar))
                            {
                                this.manager.UpdateStatus(typedChar.ToString(), true);
                                universalArgumentString.Append(typedChar);

                                return VSConstants.S_OK;
                            }
                            else
                            {
                                Commit();
                            }
                        }
                        else if ((nCmdID == (uint)VSConstants.VSStd2KCmdID.BACKSPACE))
                        {
                            Commit();
                        }
                    }
					else if (pguidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID)
					{ 
					    if ((nCmdID == (uint)VSConstants.VSStd97CmdID.Delete))
                        {
                            Commit();
                        }
                    }


                    if (pguidCmdGroup == typeof(EmacsCommandID).GUID)
                    {
                        if (nCmdID == (int)EmacsCommandID.Quit)
                            Cancel();
                        else
                            Commit(nCmdID != (int)EmacsCommandID.UniversalArgument);
                    }
                }
                else if (this.manager.UniversalArgument != null)
                {
                    // Reset the universal argument
                    this.manager.UniversalArgument = null;
                }
            }

            // if there is no match just pass the command along to the next registered filter
            return VSConstants.S_FALSE;
        }

        private void Commit(bool deactivate = true)
        {
            int universalArgument = int.MinValue;
            if (!int.TryParse(universalArgumentString.ToString(), out universalArgument))
            {
                if (universalArgumentString.Length == 0)
                    universalArgument = DefaultUniversalValue;
                else if (universalArgumentString.ToString() == NegativeArgumentSign.ToString())
                    universalArgument = -1 * DefaultUniversalValue;
            }

            if (universalArgument != int.MinValue)
            {
                if (!this.manager.UniversalArgument.HasValue)
                    this.manager.UniversalArgument = 1;

                this.manager.UniversalArgument = this.manager.UniversalArgument * universalArgument;
            }

            if (deactivate)
            {
                this.manager.ClearStatus();
                this.IsActive = false;
            }
        }

        private void Cancel()
        {
            this.manager.UniversalArgument = null;
            this.manager.ClearStatus();
            this.IsActive = false;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return VSConstants.S_FALSE;
        }

        internal void Start()
        {
            this.universalArgumentString = new StringBuilder();
            this.manager.UpdateStatus(" c-u ", this.IsActive);

            this.IsActive = true;
        }

        internal static UniversalArgumentSession GetSession(ITextView view)
        {
            return view.Properties.GetProperty<UniversalArgumentSession>(typeof(UniversalArgumentSession));
        }
    }
}
