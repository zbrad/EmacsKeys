using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Editor.EmacsEmulation.Commands;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    /// <summary>
    /// Controls the stack of marks and selection in a single view.
    /// </summary>
    internal class MarkSession : MouseProcessorBase, IOleCommandTarget
    {
        ITextView view;
        EmacsCommandsManager manager;
        ITrackingPoint activeMark;
        ITrackingPoint currentMark;
        Stack<ITrackingPoint> marks = new Stack<ITrackingPoint>();

        internal MarkSession(ITextView view, EmacsCommandsManager manager)
        {
            this.manager = manager;

            this.view = view;
            this.view.Selection.SelectionChanged += new EventHandler(Selection_SelectionChanged);

            this.activeMark = this.currentMark = CreateTrackingPoint(0);
        }

        void Selection_SelectionChanged(object sender, EventArgs e)
        {
            if (!view.Selection.IsEmpty && !this.IsActive)
            {
                PushMark(view.Selection.Start.Position.Position);
            }
        }

        private void UpdateSelection()
        {
            if (this.IsActive)
            {
                var currentPosition = this.view.GetCaretPosition();
                this.view.Selection.Select(new VirtualSnapshotPoint(currentMark.GetPoint(this.view.TextSnapshot)), new VirtualSnapshotPoint(currentPosition));
                this.view.Caret.EnsureVisible();
            }
        }

        private void ClearSelection()
        {
            if (!this.view.Selection.IsEmpty)
                this.view.Selection.Clear();
        }

        /// <summary>
        /// Gets true the mark session is active
        /// </summary>
        internal bool IsActive
        {
            get;
            private set;
        }

        /// <summary>
        /// Moves the cursor to the current mark in the location stack and moves the current mark to the location where the cursor mark was when the command was invoked. 
        /// </summary>
        internal void SwapPointAndMark()
        {
            var previousActiveMark = this.activeMark;
            this.activeMark = this.currentMark = CreateTrackingPoint(view.GetCaretPosition());
            this.view.Caret.MoveTo(previousActiveMark.GetPoint(view.TextSnapshot));
            this.IsActive = true;
            UpdateSelection();
        }

        internal void Activate()
        {
            this.IsActive = true;
            UpdateSelection();
        }

        private ITrackingPoint CreateTrackingPoint(int position)
        {
            return view.TextSnapshot.CreateTrackingPoint(position, PointTrackingMode.Negative);
        }

        /// <summary>
        /// Adds a mark to the location stack for the current cursor. 
        /// </summary>
        /// <param name="activateSession">False if the session should not be activated after pushing the mark</param>
        internal void PushMark(bool activateSession = true)
        {
            this.PushMark(this.view.GetCaretPosition(), activateSession);

            if(activateSession)
                this.UpdateSelection();
        }

        private void PushMark(int position, bool activateSession = true)
        {
            this.marks.Push(this.activeMark);
            this.activeMark = this.currentMark = this.CreateTrackingPoint(position);

            if(activateSession)
                this.IsActive = true;
        }

        /// <summary>
        /// Moves the cursor to the current mark and then removes the mark for the location stack. 
        /// </summary>
        internal void PopMark()
        {
            if (this.marks.Count > 0)
            {
                if (this.currentMark != activeMark)
                    this.currentMark = activeMark;

                this.activeMark = this.marks.Pop();
            }
            else
            {
                this.currentMark = activeMark = CreateTrackingPoint(0);
            }

            this.view.Caret.MoveTo(currentMark.GetPoint(this.view.TextSnapshot));
            this.UpdateSelection();
        }

        /// <summary>
        /// Pops the top mark and throws it away with no effect on the point
        /// </summary>
        internal void RemoveTopMark()
        {
            if (this.marks.Count > 0)
            {            
                this.activeMark = this.marks.Pop();
            }            
        }

        /// <summary>
        /// Returns the mark session instance in the view
        /// </summary>
        /// <param name="view"></param>
        /// <returns></returns>
        internal static MarkSession GetSession(ITextView view)
        {
            if (view.Properties.ContainsProperty(typeof(MarkSession)))
                return view.Properties.GetProperty<MarkSession>(typeof(MarkSession));

            return null;
        }

        int IOleCommandTarget.Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (this.IsActive && this.manager.IsEnabled)
            {
                if (pguidCmdGroup == VSConstants.VSStd2K)
                {
                    switch (nCmdID)
                    {
                        // Translate Left/Right/Up/Down to Emacs commands
                        case (uint)VSConstants.VSStd2KCmdID.LEFT: 
                            this.manager.Execute(this.view, EmacsCommandID.CharLeft); 
                            return VSConstants.S_OK;
                        case (uint)VSConstants.VSStd2KCmdID.RIGHT: 
                            this.manager.Execute(this.view, EmacsCommandID.CharRight); 
                            return VSConstants.S_OK;
                        case (uint)VSConstants.VSStd2KCmdID.UP: 
                            this.manager.Execute(this.view, EmacsCommandID.LineUp);
                            return VSConstants.S_OK;
                        case (uint)VSConstants.VSStd2KCmdID.DOWN: 
                            this.manager.Execute(this.view, EmacsCommandID.LineDown); 
                            return VSConstants.S_OK;
                        // Handle cancelation keys
                        case (uint)VSConstants.VSStd2KCmdID.TYPECHAR:
                        case (uint)VSConstants.VSStd2KCmdID.BACKSPACE:
                        case (uint)VSConstants.VSStd2KCmdID.CANCEL:
                        case (uint)VSConstants.VSStd2KCmdID.TAB:
                            Deactivate();
                            break;
                    }
                }
                else if (pguidCmdGroup == typeof(VSConstants.VSStd97CmdID).GUID)
                {
                    switch (nCmdID)
                    {
                        // Handle cancelation keys
                        case (uint)VSConstants.VSStd97CmdID.Cut:
                        case (uint)VSConstants.VSStd97CmdID.Copy:
                            Deactivate(false);
                            break;
                        case (uint)VSConstants.VSStd97CmdID.Delete:
                            Deactivate();
                            break;
                    }
                }
                else if (pguidCmdGroup == typeof(EmacsCommandID).GUID)
                {
                    switch (nCmdID)
                    {
                        // Handle cancelation Emacs commands
                        case (int)EmacsCommandID.BreakLine:
                        case (int)EmacsCommandID.BreakLineIndent:
                        case (int)EmacsCommandID.DeleteToEndOfLine:
                        case (int)EmacsCommandID.Quit:
                            {
                                Deactivate();
                                break;
                            }
                    }
                }
            }

            return VSConstants.S_FALSE;
        }

        public void Deactivate(bool clearSelection = true)
        {
            if (clearSelection)
            {
                this.ClearSelection();
            }
            this.IsActive = false;
        }

        int IOleCommandTarget.QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            return VSConstants.S_FALSE;
        }

        public override void PostprocessMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e)
        {
            Deactivate(false);
            base.PostprocessMouseLeftButtonDown(e);
        }
    }
}