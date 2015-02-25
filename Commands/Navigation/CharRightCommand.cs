using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.ComponentModel.Composition;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Commands
{
    /// <summary>
    /// This command goes forward one character.  With a prefix arg, it goes forward that many characters.
    /// If the prefix arg is negative, this command goes backward that many characters.
    /// Otherwise, acts like visual studio character motion.
    /// 
    /// Keys: Ctrl+F | Right Arrow
    /// </summary>
    [EmacsCommand(EmacsCommandID.CharRight, CanBeRepeated = true, InverseCommand = EmacsCommandID.CharLeft)]
    internal class CharRightCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.EditorOperations.MoveToNextCharacter();
        }
    }
}
