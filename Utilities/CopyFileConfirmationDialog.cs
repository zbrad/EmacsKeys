using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    public partial class CopyFileConfirmationDialog : Form
    {
        public CopyFileConfirmationDialog()
        {
            InitializeComponent();

            btnOk.FlatStyle = FlatStyle.System;
            NativeMethods.SendMessage((int)btnOk.Handle, 0x160C, 0, 1);
        }
    }
}
