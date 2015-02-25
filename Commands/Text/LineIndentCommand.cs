using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Editor.OptionsExtensionMethods;
using System.Globalization;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// Recompute the indentation for the current line, delete the indentation on the line, and re-indent it.  
    /// When this command terminates, the caret is between the same two characters it was between when the command started.  
    /// However, if it was in the indentation, then the caret moves to be after the newly inserted indentation.  
    /// The indentation inserted is language context dependent (smart).
    /// If there's a multi line selection, then no-op.
    /// 
    /// Keys: Tab
    /// </summary>
    [EmacsCommand(VSConstants.VSStd2KCmdID.TAB, UndoName = "Indent")]
    internal class LineIndentCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            ITextSelection selection = context.TextView.Selection;
            bool trackCaret = true;
            bool markSessionActive = context.MarkSession.IsActive;

            // Return immediately if the buffer is read-only.
            if (context.TextBuffer.IsReadOnly(selection.Start.Position.GetContainingLine().Extent))
            {
                return;
            }

            // If there's not a multi-line selection, then clear it and setup for line indentation
            if (!selection.IsEmpty)
            {
                if (selection.Mode == TextSelectionMode.Box)
                {
                    return;
                }
                else
                {
                    VirtualSnapshotSpan selectionSpan = selection.StreamSelectionSpan;

                    if (selectionSpan.Start.Position.GetContainingLine().LineNumber != selectionSpan.End.Position.GetContainingLine().LineNumber)
                    {
                        return;
                    }

                    selection.Clear();

                    // Since there was a selection on the line before the format, we are not obligated to place the caret at a specific place
                    // after the format operation is done
                    trackCaret = false;
                }
            }

            // Strip any existing whitespace to setup the line for formatting
            this.StripWhiteSpace(context.TextView.GetCaretPosition().GetContainingLine());

            int? indentation = context.Manager.SmartIndentationService.GetDesiredIndentation(context.TextView, context.TextView.GetCaretPosition().GetContainingLine());

            if (indentation.HasValue)
            {
                // Insert the desired indentation level
                context.TextBuffer.Insert(context.TextView.GetCaretPosition().GetContainingLine().Start, new string(' ', indentation.Value));

                // Finally, are any tab/spaces conversions necessary?
                if (!context.TextView.Options.IsConvertTabsToSpacesEnabled())
                {
                    context.EditorOperations.ConvertSpacesToTabs();
                }
            }
            else
            {
                // We couldn't find any indentation level for the line, try executing the format command as the last resort

                // Remember caret position
                int caretOffsetFromStart = 0;

                if (trackCaret)
                {
                    CaretPosition positionBeforeChange = context.TextView.Caret.Position;
                    context.EditorOperations.MoveToStartOfLineAfterWhiteSpace(false);
                    caretOffsetFromStart = positionBeforeChange.BufferPosition.Position - context.TextView.GetCaretPosition();
                }

                // Format
                context.EditorOperations.SelectAndMoveCaret(
                    new VirtualSnapshotPoint(context.TextView.GetCaretPosition().GetContainingLine().Start, 0),
                    new VirtualSnapshotPoint(context.TextView.GetCaretPosition().GetContainingLine().End, 0));

                context.CommandRouter.ExecuteDTECommand("Edit.FormatSelection");

                // Move to beginning of newly indented line after format operation is done
                context.EditorOperations.MoveToStartOfLineAfterWhiteSpace(false);

                // Restore the position of the caret
                if (caretOffsetFromStart > 0)
                {
                    context.EditorOperations.MoveCaret(context.TextView.Caret.Position.BufferPosition + caretOffsetFromStart, false);
                }
            }

            // Ensure we restore the state of the mark session after the formatting operation (changing selection activates
            // the mark session automatically and we change the selection during our formatting operation).
            if (!markSessionActive)
            {
                context.MarkSession.Deactivate();
            }
        }

        /// <summary>
        /// Removes white space from both ends of a line.
        /// </summary>
        private void StripWhiteSpace(ITextSnapshotLine line)
        {
            ITextSnapshot snapshot = line.Snapshot;
            ITextBuffer buffer = snapshot.TextBuffer;

            int forwardIterator;
            int backwardIterator;

            // Detect spaces at the beginning
            forwardIterator = line.Start.Position;
            while (forwardIterator < line.End.Position && IsSpaceCharacter(snapshot[forwardIterator]))
            {
                ++forwardIterator;
            }

            // Detect spaces at the end
            backwardIterator = line.End.Position - 1;
            while (backwardIterator > forwardIterator && IsSpaceCharacter(snapshot[backwardIterator]))
            {
                --backwardIterator;
            }

            if ((backwardIterator != line.End.Position - 1) || (forwardIterator != line.Start.Position))
            {
                using (ITextEdit edit = buffer.CreateEdit())
                {
                    edit.Delete(Span.FromBounds(backwardIterator + 1, line.End.Position));
                    edit.Delete(Span.FromBounds(line.Start.Position, forwardIterator));

                    edit.Apply();
                }
            }
        }

        /// <summary>
        /// Copied from EditorOperations. Custom definition for Orcas parity.
        /// </summary>
        private static bool IsSpaceCharacter(char c)
        {
            return c == '\t' ||
                   (int)c == 0x200B ||
                   char.GetUnicodeCategory(c) == UnicodeCategory.SpaceSeparator;
        }
    }
}