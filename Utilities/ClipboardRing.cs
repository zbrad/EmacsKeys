using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation
{
    /// <summary>
    /// A class that keeps track of elements put by Emacs emulators on the clipboard. We're abandoning Visual Studio's clipboard
    /// ring because it doesn't let you count the number of items in it (and there's no way to know that you've already looped
    /// around) and because it only adds items to itself when a Cut or Copy command was executed in the editor.
    /// </summary>
    class ClipboardRing
    {
        const int CHAIN_SIZE = 10;

        /// <summary>
        /// Tracking data put in Emacs' clipboard chain. A null value denotes
        /// delegating/querying the VS clipboard ring.
        /// </summary>
        string[] _dataItems;

        /// <summary>
        /// Pointer to the current item in the chain.
        /// </summary>
        int _currentItem;

        /// <summary>
        /// Pointer to the next insertion in the chain.
        /// </summary>
        int _nextInsertionIndex;

        public ClipboardRing()
        {
            _dataItems = new string[CHAIN_SIZE];
            
            // create an item denoting delegation of work/queries to the VS clipboard ring
            _dataItems[0] = null;

            _currentItem = 0;
            _nextInsertionIndex = 0;
        }

        public bool IsEmpty
        {
            get
            {
                return _currentItem == 0 && _dataItems[_currentItem] == null;
            }
        }

        public void Reset()
        {
            _currentItem = 0;
        }

        public string GetNext()
        {
            string nextDataItem = _dataItems[_currentItem];

            _currentItem = this.IncreaseIndex(_currentItem);

            // Wrap to the start if we need to loop in a situation where
            // the ring is not completely full
            if (!this.IsEmpty && _dataItems[_currentItem] == null)
                _currentItem = 0;

            return nextDataItem;
        }

        public void Add(string data)
        {
            _dataItems[_nextInsertionIndex] = data;

            _nextInsertionIndex = this.IncreaseIndex(_nextInsertionIndex);
        }

        public void CopyToClipboard(string data)
        {
            System.Windows.DataObject dataObject = new System.Windows.DataObject();

            // put textual data
            dataObject.SetText(data, System.Windows.TextDataFormat.UnicodeText);
            dataObject.SetText(data, System.Windows.TextDataFormat.Text);

            System.Windows.Clipboard.SetDataObject(dataObject, false);
        }

        private int IncreaseIndex(int originalIndex)
        {
            int result = originalIndex + 1;

            // wrap?
            if (result == _dataItems.Length)
                result = 0;

            return result;
        }
    }
}
