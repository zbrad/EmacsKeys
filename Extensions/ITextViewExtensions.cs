using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor.EmacsEmulation.Utilities;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    internal static class ITextViewExtensions
    {
        private static string KillStringObjectID = "KillStringObjectID";

        internal static SnapshotPoint GetCaretPosition(this ITextView view)
        {
            return view.Caret.Position.BufferPosition;
        }

        // A kill string stores the set of cut text in a view until it's pushed down to the clipboard. For instance,
        // if the user performs 4 kill word commands, the text for all those 4 commands is accumulated and then
        // pushed to the clipboard when the user performs a non kill command (for instance moving the caret)
        #region Kill String Management

        internal static string GetKillString(this ITextView view)
        {
            if (view.Properties.ContainsProperty(KillStringObjectID))
            {
                return view.Properties.GetProperty<StringBuilder>(KillStringObjectID).ToString();
            }
            else
            {
                return null;
            }
        }

        internal static void ResetKillString(this ITextView view)
        {
            view.Properties.RemoveProperty(KillStringObjectID);
        }

        internal static void AppendKillString(this ITextView view, string value)
        {
            view.Properties.GetOrCreateSingletonProperty<StringBuilder>(KillStringObjectID, () => new StringBuilder()).Append(value);
        }

        internal static void FlushKillSring(this ITextView view, ClipboardRing clipboardRing)
        {
            string accumulatedKillString = view.GetKillString();

            if (!string.IsNullOrEmpty(accumulatedKillString))
            {
                clipboardRing.CopyToClipboard(accumulatedKillString);
                clipboardRing.Add(accumulatedKillString);

                view.ResetKillString();
            }
        }

        #endregion
    }
}
