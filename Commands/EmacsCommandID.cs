using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    [Guid("9a95f3af-f86a-4aa2-80e6-012bf65dbbc3")]
    public enum EmacsCommandID
    {
        CharLeft = 0x1,
        CharRight = 0x2,
        LineUp = 0x3,
        LineDown = 0x4,
        LineEnd = 0x5,
        LineStart = 0x6,
        DocumentEnd = 0x7,
        DocumentStart = 0x8,
        WordPrevious = 0x9,
        WordNext = 0xA,
        GoToLine = 0xB,      
        ScrollPageUp = 0xC,
        ScrollPageDown = 0xD,
        ScrollLineCenter = 0xE,
        ScrollLineTop = 0x11,
        SplitVertical = 0x12,
        OtherWindow = 0x13,
        CloseOtherWindow = 0x14,
        TopOfWindow = 0xF,
        BottomOfWindow = 0x10,

        BreakLine = 0x15,
        BreakLineIndent = 0x16,
        LineOpen = 0x17,
        CharTranspose = 0x18,
        WordTranspose = 0x19,
        WordCapitalize = 0x1F,
        WordUppercase = 0x1D,
        WordLowercase = 0x1E,
        WordDeleteToEnd = 0x20,
        WordDeleteToStart = 0x21,
        DeleteToEndOfLine = 0x22,
        PasteRotate = 0x25,
        DeleteSelection = 0x2A,

        FileOpen = 0x2B,
        FileSave = 0x2C,
        FileSaveAs = 0x2D,
        FileSaveDirty = 0x2E,

        IncrementalSearch = 0x2F,
        IncrementalSearchBackwards = 0x30,

        SetMark = 0x27,
        PopMark = 0x28,
        SwapPointAndMark = 0x29,

        FindReplace = 0x31,
        Quit = 0x33,
        UniversalArgument = 0x34,
        ExtendedCommand = 0x35,

        QuotedInsert = 0x3A,
		ActivateRegion = 0x3B
    }
}
