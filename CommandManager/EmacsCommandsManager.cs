using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using EnvDTE;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Editor.EmacsEmulation.Commands;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using System.IO;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Editor.EmacsEmulation.Utilities;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    /// <summary>
    /// Manages and executes emacs commands
    /// </summary>
    [Export]
    internal class EmacsCommandsManager : IPartImportsSatisfiedNotification
    {
        internal const string EmacsVskFile = "Emacs.vsk";
        internal const string ClipboardDataTag = "Emacs Emulator Clipboard Tag";

        [Import(typeof(SVsServiceProvider))]
        public System.IServiceProvider ServiceProvider { get; private set; }

        [ImportMany]
        IEnumerable<Lazy<EmacsCommand, IEmacsCommandMetadata>> Commands { get; set; }

        [Import]
        IEditorOperationsFactoryService EditorOperationsFactoryService { get; set; }

        [Import]
        public IVsEditorAdaptersFactoryService EditorAdaptersFactoryService { get; set; }

        [Import]
        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService { get; set; }

        [Import]
        public ITextUndoHistoryRegistry TextUndoHistoryRegistry { get; set; }

        [Import]
        public ISmartIndentationService SmartIndentationService { get; set; }

        [Import]
        CommandRouterProvider CommandRouterProvider { get; set; }

        public IComponentModel ComponentModel { get; private set; }

        public ClipboardRing ClipboardRing { get; private set; }

        public bool IsEnabled { get; set; }

        public void OnImportsSatisfied()
        {
            this.ComponentModel = this.ServiceProvider.GetService<SComponentModel, IComponentModel>();
        }

        public EmacsCommandsManager()
        {
            this.ClipboardRing = new ClipboardRing();
        }

        /// <summary>
        /// Gets or sets how many times the next command will be executed
        /// </summary>
        internal int? UniversalArgument { get; set; }

        /// <summary>
        /// Returns the universal argument value if it was specified. Otherwise it returns the default value.
        /// </summary>
        /// <param name="defaultValue">The default value</param>
        /// <returns></returns>
        internal int GetUniversalArgumentOrDefault(int defaultValue = 0)
        {
            if (!this.UniversalArgument.HasValue)
                return defaultValue;

            return this.UniversalArgument.Value;
        }

        public void Execute(ITextView view, EmacsCommandID commandId, bool evaluateUniversalArgument = true)
        {
            var commandMetadata = GetCommandMetadata(commandId);

            if (commandMetadata != null)
                this.Execute(view, commandMetadata, evaluateUniversalArgument);
        }

        /// <summary>
        /// Executes the emacs command
        /// </summary>
        /// <param name="view"></param>
        /// <param name="metadata"></param>
        public void Execute(ITextView view, IEmacsCommandMetadata metadata, bool evaluateUniversalArgument = true)
        {
            using (BufferMonitor bufferMonitor = BufferMonitor.Create(view.TextBuffer))
            {
                try
                {
                    EmacsCommand command = CreateCommand(metadata);
                    EmacsCommand inverseCommand = null;
                    IEmacsCommandMetadata inverseCommandMetadata = null;

                    var context = new EmacsCommandContext(
                        this, this.TextStructureNavigatorSelectorService,
                        this.EditorOperationsFactoryService.GetEditorOperations(view),
                        view, CommandRouterProvider.GetCommandRouter(view));

                    if (command != null)
                    {
                        var history = this.TextUndoHistoryRegistry.GetHistory(context.TextBuffer);
                        using (var transaction = CreateTransaction(metadata, history))
                        {
                            var repeatCount = 1;
                            bool shouldExecuteInverse = false;

                            if (evaluateUniversalArgument)
                            {
                                // Check if we should execute the inverse logic of the command by checking if the universal argument is lower than 0
                                shouldExecuteInverse = GetUniversalArgumentOrDefault() < 0;
                                if (shouldExecuteInverse)
                                {
                                    // Search the inverse command using the metadata
                                    inverseCommandMetadata = GetCommandMetadata(metadata.InverseCommand);
                                    inverseCommand = CreateCommand(inverseCommandMetadata);
                                }

                                // If the command specifies that can be repeated use the universal argument as the counter, otherwise execute the command only once.
                                if (metadata.CanBeRepeated)
                                    repeatCount = Math.Abs(GetUniversalArgumentOrDefault(1));
                            }

                            for (; repeatCount > 0; repeatCount--)
                            {
                                if (shouldExecuteInverse)
                                {
                                    // Execute the inverse logic
                                    if (inverseCommand != null)
                                        inverseCommand.Execute(context);
                                    else
                                        command.ExecuteInverse(context);
                                }
                                else
                                {
                                    // Execute the normal logic
                                    command.Execute(context);
                                }
                            }

                            // TODO: Check command error and rollback the transaction     
                            if (transaction != null)
                            {
                                transaction.Complete();
                            }

                            // If the command executed was a kill command, then we want to append the text that was cut
                            // to any currently text cut for the view.
                            // If the command is not a kill command and we have had some cut commands executed previously
                            // that accumulated deleted text, then we want to put the data on the clipboard and reset
                            // the accumulated text.
                            if (metadata.IsKillCommand)
                            {
                                view.AppendKillString(bufferMonitor.BufferChanges);
                            }
                            else
                            {
                                view.FlushKillSring(this.ClipboardRing);
                            }

                            this.LastExecutedCommand = shouldExecuteInverse ? inverseCommandMetadata : metadata;
                        }
                    }
                }
                catch (NoOperationException)
                {
                    // Do nothing
                }
            }
        }

        private static ITextUndoTransaction CreateTransaction(IEmacsCommandMetadata metadata, ITextUndoHistory history)
        {
            if (string.IsNullOrEmpty(metadata.UndoName))
            {
                return null;
            }
            return history.CreateTransaction(metadata.UndoName);
        }

        private EmacsCommand CreateCommand(IEmacsCommandMetadata metadata)
        {
            if (metadata == null)
                return null;

            var command = this.Commands.FirstOrDefault(export => export.Metadata.Command == metadata.Command && export.Metadata.CommandGroup == metadata.CommandGroup);

            if (command != null)
                return command.Value;

            return null;
        }

        internal IEmacsCommandMetadata GetCommandMetadata(EmacsCommandID command)
        {
            return GetCommandMetadata((int)command, typeof(EmacsCommandID).GUID);
        }

        internal IEmacsCommandMetadata GetCommandMetadata(int commandId, Guid commandGroup)
        {
            return this.Commands
                .Select(lazy => lazy.Metadata)
                .FirstOrDefault(metadata => metadata.Command == commandId && new Guid(metadata.CommandGroup) == commandGroup);
        }

        internal void ClearStatus()
        {
            UpdateStatus(string.Empty);
        }

        internal void UpdateStatus(string text, bool append = false)
        {
            var dte = this.ServiceProvider.GetService<DTE>();
            if (dte != null && dte.StatusBar != null)
            {
                if (append)
                {
                    dte.StatusBar.Text += text;
                }
                else
                {
                    if (string.IsNullOrEmpty(text))
                        dte.StatusBar.Clear();
                    else
                        dte.StatusBar.Text = text;
                }
            }
        }

        internal bool IsEmacsVskInstalled
        {
            get { return File.Exists(Path.Combine(EmacsInstallationPath, EmacsVskFile)); }
        }

        internal string EmacsInstallationPath
        {
            get { return GetVsInstallPath(); }
        }

        string GetVsInstallPath()
        {
            var reg = this.ServiceProvider.GetService<SLocalRegistry, ILocalRegistry2>();

            string root = null;
            reg.GetLocalRegistryRoot(out root);

            using (var key = Registry.LocalMachine.OpenSubKey(root))
            {
                var installDir = key.GetValue("InstallDir") as string;

                return Path.GetDirectoryName(installDir);
            }
        }        

        internal void CheckEmacsVskSelected()
        {
            try
            {
                if (this.IsEmacsVskInstalled)
                {
                    var reg = this.ServiceProvider.GetService<SLocalRegistry, ILocalRegistry2>();

                    string root = null;
                    reg.GetLocalRegistryRoot(out root);

                    using (var vsKey = Registry.CurrentUser.OpenSubKey(root))
                    {
                        if (vsKey != null)
                        {
                            using (var keyboardKey = vsKey.OpenSubKey("Keyboard"))
                            {
                                if (keyboardKey != null)
                                {
                                    var schemeName = keyboardKey.GetValue("SchemeName") as string;

                                    this.IsEnabled = !string.IsNullOrEmpty(schemeName) && string.Equals("Emacs.vsk", Path.GetFileName(schemeName), StringComparison.InvariantCultureIgnoreCase);
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                this.IsEnabled = false;
            }
        }

        public IEmacsCommandMetadata LastExecutedCommand { get; set; }

        public bool AfterSearch { get; set; }

        public MarkSession GetOrCreateMarkSession(ITextView view)
        {
            return view.Properties.GetOrCreateSingletonProperty<MarkSession>(() => new MarkSession(view, this));
        }

        public UniversalArgumentSession GetOrCreateUniversalArgumentSession(ITextView view)
        {
            return view.Properties.GetOrCreateSingletonProperty<UniversalArgumentSession>(() => new UniversalArgumentSession(view, this));
        }
    }
}