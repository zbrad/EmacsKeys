using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Utilities;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Editor.EmacsEmulation.Commands;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.ComponentModelHost;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    [Export(typeof(IVsTextViewCreationListener))]
    [Export(typeof(IMouseProcessorProvider))]
    [ContentType("text")]
    [Name("Emacs Emulation MouseProcessor")]
    [Order(Before = "default")]
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal class EmacsFactory : IVsTextViewCreationListener, IMouseProcessorProvider
    {
        [Import]
        EmacsCommandsManager Manager { get; set; }

        [Import]
        IVsEditorAdaptersFactoryService EditorAdaptersFactory { get; set; }

        [Import]
        CommandRouterProvider CommandRouterProvider { get; set; }

        public void VsTextViewCreated(Microsoft.VisualStudio.TextManager.Interop.IVsTextView textViewAdapter)
        {
            var view = this.EditorAdaptersFactory.GetWpfTextView(textViewAdapter);
            view.Options.OptionChanged += OnOptionsChanged;

            IOleCommandTarget nextCommandTarget;

            if (view.Roles.Contains(PredefinedTextViewRoles.PrimaryDocument))
            {
                CommandRouter commandRouter = CommandRouterProvider.GetCommandRouter(view);

                // Register internal handlers with the command router

                // Register the filter to execute emacs commands
                commandRouter.AddCommandTarget(new EmacsCommandsFilter(view, this.Manager, commandRouter));

                // Register the mark session
                commandRouter.AddCommandTarget(this.Manager.GetOrCreateMarkSession(view));

                // Register the universal argument session
                commandRouter.AddCommandTarget(this.Manager.GetOrCreateUniversalArgumentSession(view));
            }
            else
            {
                // Register the filter to execute emacs commands
                var commandFilter = new InteractiveRoleWorkAroundFilter(view, this.Manager);
                textViewAdapter.AddCommandFilter(commandFilter, out nextCommandTarget);
                commandFilter.NextCommandTarget = nextCommandTarget;
            }
        }

        void OnOptionsChanged(object sender, EditorOptionChangedEventArgs e)
        {
            this.Manager.CheckEmacsVskSelected();
        }

        public IMouseProcessor GetAssociatedProcessor(IWpfTextView wpfTextView)
        {
            return this.Manager.GetOrCreateMarkSession(wpfTextView);
        }
    }
}