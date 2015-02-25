using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command rotates the Clipboard Ring and inserts the next item, replacing 
    /// the text previously inserted. 
    /// 
    /// Keys: Alt+Y
    /// </summary>
    [EmacsCommand(EmacsCommandID.PasteRotate, UndoName = "Paste")]
    internal class PasteRotateCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            ITextSelection selection = context.TextView.Selection;
            ClipboardRing ring = context.Manager.ClipboardRing;

            if (!ring.IsEmpty)
            {
                ITrackingSpan preInsertionSelectionSpan = null;

                if (!selection.IsEmpty)
                {
                    // Currently only support stream selection
                    if (selection.Mode == TextSelectionMode.Stream)
                    {
                        preInsertionSelectionSpan = context.TextView.TextSnapshot.CreateTrackingSpan(selection.StreamSelectionSpan.SnapshotSpan, SpanTrackingMode.EdgeInclusive);
                    }
                    else
                    {
                        selection.Clear();
                    }
                }
                else
                {
                    preInsertionSelectionSpan = context.TextView.TextSnapshot.CreateTrackingSpan(new Span(selection.AnchorPoint.Position, 0), SpanTrackingMode.EdgeInclusive);
                }
                
                context.EditorOperations.ReplaceSelection(ring.GetNext());

                // Select newly inserted text
                SnapshotSpan newSelectionRange = preInsertionSelectionSpan.GetSpan(context.TextView.TextSnapshot);
                context.EditorOperations.SelectAndMoveCaret(new VirtualSnapshotPoint(newSelectionRange.Start), new VirtualSnapshotPoint(newSelectionRange.End));
            }
        }
    }
}