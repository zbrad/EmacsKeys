using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    internal class EmacsCommandContext
    {
        internal EmacsCommandContext(
            EmacsCommandsManager manager,
            ITextStructureNavigatorSelectorService textStructureNavigatorSelectorService,
            IEditorOperations editorOperations,
            ITextView view,
            CommandRouter commandRouter)
        {
            this.Manager = manager;
            this.EditorOperations = editorOperations;
            this.TextView = view;
            this.CommandRouter = commandRouter;

            this.TextStructureNavigator = textStructureNavigatorSelectorService.GetTextStructureNavigator(view.TextBuffer);
            this.MarkSession = MarkSession.GetSession(view);
        }

        internal ITextStructureNavigator TextStructureNavigator { get; private set; }
        internal IEditorOperations EditorOperations { get; private set; }
        internal ITextView TextView { get; private set; }
        internal ITextBuffer TextBuffer { get { return this.TextView.TextBuffer; } }
        internal EmacsCommandsManager Manager { get; private set; }
        internal CommandRouter CommandRouter { get; private set; }
        internal int? UniversalArgument { get { return this.Manager.UniversalArgument; } }               
        internal MarkSession MarkSession { get; private set; }
    }
}
