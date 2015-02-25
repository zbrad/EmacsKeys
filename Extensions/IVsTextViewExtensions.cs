using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
	internal static class IVsTextViewExtensions
	{
		internal static IVsWindowFrame GetWindowFrame(this IVsTextView textView)
		{			
			IObjectWithSite objWithSite = (IObjectWithSite)textView;
			var serviceProviderGuid = typeof(Microsoft.VisualStudio.OLE.Interop.IServiceProvider).GUID;

			IntPtr serviceProviderPointer;
			objWithSite.GetSite(ref serviceProviderGuid, out serviceProviderPointer);
			var serviceProvider = (Microsoft.VisualStudio.OLE.Interop.IServiceProvider)Marshal.GetObjectForIUnknown(serviceProviderPointer);

			Marshal.Release(serviceProviderPointer);

			serviceProviderGuid = typeof(SVsWindowFrame).GUID;
			var interfaceGuid = typeof(IVsWindowFrame).GUID;
			IntPtr windowFramePointer;
			serviceProvider.QueryService(ref serviceProviderGuid, ref interfaceGuid, out windowFramePointer);

			var windowFrame = (IVsWindowFrame)Marshal.GetObjectForIUnknown(windowFramePointer);

			Marshal.Release(windowFramePointer);

			return windowFrame;
		}

        internal static int GetStartPositionAfterLines(this ITextView textView, ITextViewLine line, int currentLineDifference)
        {
            var lineNumber = textView.TextBuffer.GetLineNumber(line.Start) + currentLineDifference;
            if (lineNumber >= textView.TextBuffer.CurrentSnapshot.LineCount)
            {
                lineNumber = textView.TextBuffer.CurrentSnapshot.LineCount - 1;
            }

            var currentLine = textView.TextSnapshot.GetLineFromLineNumber(lineNumber);

            return currentLine.Start.Position;
        }

        internal static void PositionCaretOnLine(this ITextView textView, int viewLine)
        {
            var newPosition = textView.GetStartPositionAfterLines(textView.TextViewLines.FirstVisibleLine, viewLine);
            if (newPosition >= textView.TextSnapshot.Length)
            {
                newPosition = textView.TextSnapshot.Length -1;
            }

            textView.Caret.MoveTo(new SnapshotPoint(textView.TextSnapshot, newPosition));
            textView.Caret.EnsureVisible();
        }


	}
}
