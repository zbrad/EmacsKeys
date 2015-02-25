using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Formatting;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    internal static class ITextBufferExtensions
    {
        internal static ITextSnapshotLine GetContainingLine(this ITextBuffer textBuffer, int position)
        {
            return textBuffer.CurrentSnapshot.Lines.FirstOrDefault(l => l.Start <= position && l.End >= position);
        }

        internal static int GetLineNumber(this ITextBuffer textBuffer, SnapshotPoint position)
        {
            return textBuffer.GetContainingLine(position.Position).LineNumber;
        }

    }
}
