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
    /// This command goes backward one character.  With a prefix arg, it goes back that many characters.
    /// If the prefix arg is negative, this command goes forward that many characters.
    /// Otherwise, acts like Visual Studio char motion
    /// 
    /// Keys: Ctrl+B | Left Arrow
    /// </summary>
    [EmacsCommand(EmacsCommandID.CharLeft, CanBeRepeated = true, InverseCommand = EmacsCommandID.CharRight)]
    internal class CharLeftCommand : EmacsCommand
    {
        internal override void Execute(EmacsCommandContext context)
        {
            context.EditorOperations.MoveToPreviousCharacter();
        }        
    }
}