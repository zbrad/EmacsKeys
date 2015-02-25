using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.TextManager.Interop;
using EnvDTE;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    /// <summary>
    /// A class that manages routing of commands in the emacs emulator. This class is added as a command filter to the
    /// view's command filter chain and routes the commands among command filters internal to the emacs emulator.
    /// </summary>
    class CommandRouter : IOleCommandTarget
    {
        LinkedList<IOleCommandTarget> _targets;
        ITextView _view;
        IOleCommandTarget _viewCommandTarget;
        ICompletionBroker _completionBroker;
        DTE _dte;

        /// <summary>
        /// True when the command router itself is executing the command down the view chain.
        /// </summary>
        bool _inExecute;

        /// <summary>
        /// The next command target in the view's command filter chain.
        /// </summary>
        public IOleCommandTarget Next { get; set; }

        public CommandRouter(ITextView view, IOleCommandTarget viewCommandTarget, ICompletionBroker completionBroker, DTE dte)
        {
            _targets = new LinkedList<IOleCommandTarget>();
            _view = view;
            _completionBroker = completionBroker;
            _inExecute = false;
            _viewCommandTarget = viewCommandTarget;
            _dte = dte;
        }

        /// <summary>
        /// Executes a command in the view's command chain without passing it to the internal command filters
        /// of the emulator.
        /// </summary>
        public int ExecuteCommand(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            Guid commandGroupCopy = pguidCmdGroup;

            return this.ExecuteClosuredCommand<int>(() =>
                {
                    return _viewCommandTarget.Exec(ref commandGroupCopy, nCmdID, nCmdexecopt, pvaIn, pvaOut);
                });
        }

        /// <summary>
        /// Executes a Visual Studio command. For example: "Edit.Replace", "Edit.GoTo", etc
        /// </summary>
        /// <param name="visualStudioCommandName"></param>
        public void ExecuteDTECommand(string visualStudioCommandName)
        {
            this.ExecuteClosuredCommand(() =>
                {
                    if (_dte != null)
                        _dte.ExecuteCommand(visualStudioCommandName);
                });
        }

        /// <summary>
        /// Adds a new command target to the head of the chain.
        /// </summary>
        public void AddCommandTarget(IOleCommandTarget target)
        {
            _targets.AddFirst(target);
        }

        /// <summary>
        /// Routing strategy: route to all internal command filters, if none executed anything, then continue routing to the
        /// view's command filter chain.
        /// </summary>
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (this.IsAllowed(ref pguidCmdGroup, nCmdID))
            {
                foreach (IOleCommandTarget target in _targets)
                {
                    if (target.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut) == VSConstants.S_OK)
                    {
                        return VSConstants.S_OK;
                    }
                }
            }

            return this.Next.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }

        /// <summary>
        /// Routing strategy: route to all internal command filters, if none accepted, then continue routing to the
        /// view's command filter chain.
        /// </summary>
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (this.IsAllowed(ref pguidCmdGroup, prgCmds[0].cmdID))
            {
                foreach (IOleCommandTarget target in _targets)
                {
                    if (target.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText) == VSConstants.S_OK)
                    {
                        return VSConstants.S_OK;
                    }
                }
            }

            return this.Next.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        private ReturnType ExecuteClosuredCommand<ReturnType>(Func<ReturnType> command)
        {
            if (_inExecute)
                throw new InvalidOperationException("Already executing closured command. The command filter has a loop");

            try
            {
                _inExecute = true;

                return command.Invoke();
            }
            finally
            {
                _inExecute = false;
            }
        }

        /// <remarks>
        /// This is a workaround for a defect in C#'s formatter. Their code gets confused when there is more than one nested Exec chain and to work
        /// around it, we queue a command to be executed on to the dispatcher so that it is not executed within a currently ongoing Exec (in the
        /// view's command filter chain). See Dev11 32570.
        /// </remarks>
        private void ExecuteClosuredCommand(Action command)
        {
            System.Windows.Threading.Dispatcher dispatcher = System.Windows.Threading.Dispatcher.CurrentDispatcher;

            dispatcher.BeginInvoke(new Action(() =>
            {
                if (_inExecute)
                    throw new InvalidOperationException("Already executing closured command. The command filter has a loop");

                try
                {
                    _inExecute = true;

                    command.Invoke();
                }
                finally
                {
                    _inExecute = false;
                }
            }), System.Windows.Threading.DispatcherPriority.Input);
        }

        /// <summary>
        /// Determines whether the state of the emulator allows it to process the given command.
        /// </summary>
        private bool IsAllowed(ref Guid pguidCmdGroup, uint nCmdID)
        {
            // If we're in the middle of an execute, then don't route the command to internal 
            // command filters.
            if (_inExecute)
            {
                return false;
            }

            if (IsIntellisenseActive())
            {
                return !IsIntellisenseCommand(ref pguidCmdGroup, nCmdID);
            }

            return true;
        }

        private bool IsIntellisenseActive()
        {
            return _completionBroker.IsCompletionActive(_view);
        }

        private bool IsIntellisenseCommand(ref Guid pguidCmdGroup, uint nCmdID)
        {
            // The following command set has been copied from the product (env\Editor\Pkg\Impl\Intellisense\ShimCompletionController.cs).
            // These are all the commands that Intellisense completion could react to.
            if (pguidCmdGroup == VSConstants.VSStd2K)
            {
                switch (nCmdID)
                {
                    case (uint)VSConstants.VSStd2KCmdID.CANCEL:
                    case (uint)VSConstants.VSStd2KCmdID.BACKSPACE:
                    case (uint)VSConstants.VSStd2KCmdID.DELETE:
                    case (uint)VSConstants.VSStd2KCmdID.LEFT:
                    case (uint)VSConstants.VSStd2KCmdID.RIGHT:
                    case (uint)VSConstants.VSStd2KCmdID.LEFT_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.LEFT_EXT_COL:
                    case (uint)VSConstants.VSStd2KCmdID.RIGHT_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.RIGHT_EXT_COL:
                    case (uint)VSConstants.VSStd2KCmdID.FIRSTCHAR:
                    case (uint)VSConstants.VSStd2KCmdID.FIRSTCHAR_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.WORDPREV:
                    case (uint)VSConstants.VSStd2KCmdID.WORDPREV_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.WORDNEXT:
                    case (uint)VSConstants.VSStd2KCmdID.WORDNEXT_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.BOL:
                    case (uint)VSConstants.VSStd2KCmdID.BOL_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.EOL:
                    case (uint)VSConstants.VSStd2KCmdID.EOL_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.UP:
                    case (uint)VSConstants.VSStd2KCmdID.DOWN:
                    case (uint)VSConstants.VSStd2KCmdID.PAGEUP:
                    case (uint)VSConstants.VSStd2KCmdID.PAGEDN:
                    case (uint)VSConstants.VSStd2KCmdID.BOTTOMLINE:
                    case (uint)VSConstants.VSStd2KCmdID.TOPLINE:
                    case (uint)VSConstants.VSStd2KCmdID.UP_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.DOWN_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.PAGEUP_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.PAGEDN_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.BOTTOMLINE_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.TOPLINE_EXT:
                    case (uint)VSConstants.VSStd2KCmdID.ECMD_DECREASEFILTER:
                    case (uint)VSConstants.VSStd2KCmdID.ECMD_LEFTCLICK:
                    case (uint)/*ECMD_INCREASEFILTER*/145:
                    case (uint)VSConstants.VSStd2KCmdID.ToggleConsumeFirstCompletionMode:
                    case (uint)VSConstants.VSStd2KCmdID.TYPECHAR:
                    case (uint)VSConstants.VSStd2KCmdID.TAB:
                    case (uint)VSConstants.VSStd2KCmdID.OPENLINEABOVE:
                    case (uint)VSConstants.VSStd2KCmdID.RETURN:
                        return true;
                }
            }
            else if (pguidCmdGroup == VSConstants.GUID_VSStandardCommandSet97)
            {
                switch (nCmdID)
                {
                    case (uint)VSConstants.VSStd97CmdID.Delete:
                        return true;
                }
            }

            // If the command is none of the above, the it's not an Intellisense command
            return false;
        }
    }
}
