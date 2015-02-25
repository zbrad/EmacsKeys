using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Text;

namespace Microsoft.VisualStudio.Editor.EmacsEmulation.Utilities
{
    /// <summary>
    /// Monitors the changes of a buffer tracking changed content onto a string.
    /// </summary>
    class BufferMonitor : IDisposable
    {
        private bool _isDisposed;
        private StringBuilder _bufferChanges;
        private ITextBuffer _buffer;

        private BufferMonitor(ITextBuffer buffer)
        {
            _isDisposed = false;
            _buffer = buffer;
            _bufferChanges = new StringBuilder();

            _buffer.Changed += OnBufferChanged;
        }

        private void OnBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            foreach(ITextChange change in e.Changes)
            {
                _bufferChanges.Append(change.OldText);
            }
        }

        public static BufferMonitor Create(ITextBuffer buffer)
        {
            return new BufferMonitor(buffer);
        }

        public void Dispose()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("BufferMonitor");

            _isDisposed = true;
            _buffer.Changed -= OnBufferChanged;
        }

        public bool IsEmpty
        {
            get
            {
                return _bufferChanges.Length == 0;
            }
        }

        public string BufferChanges
        {
            get
            {
                return _bufferChanges.ToString();
            }
        }
    }
}
