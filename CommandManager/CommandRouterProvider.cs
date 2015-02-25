using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using EnvDTE;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.TextManager.Interop;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    [Export]
    class CommandRouterProvider
    {
        [Import]
        private ICompletionBroker CompletionBroker = null;

        [Import(typeof(SVsServiceProvider))]
        private System.IServiceProvider ServiceProvider = null;

        [Import]
        private IVsEditorAdaptersFactoryService EditorAdapterFactoryService = null;

        public CommandRouter GetCommandRouter(ITextView view)
        {
            return view.Properties.GetOrCreateSingletonProperty<CommandRouter>(
                () => 
                    {
                        IOleCommandTarget nextCommandTarget = null;
                        IVsTextView textViewAdapter = EditorAdapterFactoryService.GetViewAdapter(view);
                        DTE dte = ServiceProvider.GetService<DTE>();

                        // create a new router and add it to the view's command chain
                        CommandRouter router = new CommandRouter(view, textViewAdapter as IOleCommandTarget, CompletionBroker, dte);

                        System.Runtime.InteropServices.Marshal.ThrowExceptionForHR(textViewAdapter.AddCommandFilter(router, out nextCommandTarget));

                        router.Next = nextCommandTarget;

                        return router;
                    });
        }
    }
}
