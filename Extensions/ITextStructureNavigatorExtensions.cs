using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    internal static class ITextStructureNavigatorExtensions
    {
        internal static SnapshotSpan? GetPreviousWord(this ITextStructureNavigator navigator, ITextView view)
        {
            return navigator.GetPreviousWord(view.GetCaretPosition());
        }

        internal static SnapshotSpan? GetPreviousWord(this ITextStructureNavigator navigator, SnapshotPoint position)
        {
            var word = new TextExtent(new SnapshotSpan(position, 0), false);
            while(!word.IsSignificant && word.Span.Start.Position > 0)
            {
                word = navigator.GetExtentOfWord(new SnapshotPoint(word.Span.Snapshot, word.Span.Start.Position - 1));
            }

            return word.IsSignificant ? new SnapshotSpan?(word.Span) : null;
        }

        internal static SnapshotSpan? GetNextWord(this ITextStructureNavigator navigator, ITextView view)
        {
            return navigator.GetNextWord(view.GetCaretPosition());
        }

        internal static SnapshotSpan? GetNextWord(this ITextStructureNavigator navigator, SnapshotPoint position)
        {
            var word = navigator.GetExtentOfWord(position);
            while (!word.IsSignificant && !word.Span.IsEmpty)
            {
                word = navigator.GetExtentOfWord(word.Span.End);
            }

            return word.IsSignificant ? new SnapshotSpan?(word.Span) : null;
        }
    }
}