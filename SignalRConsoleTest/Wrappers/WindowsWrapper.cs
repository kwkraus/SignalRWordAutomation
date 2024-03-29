﻿using Microsoft.Office.Interop.Word;
using System;
using System.Runtime.InteropServices;

namespace SignalRConsoleTest.Wrappers
{
    public class WindowWrapper : IDisposable
    {
        private Window _win;

        public WindowWrapper(Window win)
        {
            _win = win;
        }

        public WdWindowState WindowState
        {
            get { return _win.WindowState; }
            set { _win.WindowState = value; }
        }


        public void Dispose()
        {
            if (_win != null)
            {
                Marshal.ReleaseComObject(_win);
                _win = null;
            }
        }

        public void SetFocus()
        {
            _win.SetFocus();
        }

        public void Activate()
        {
            _win.Activate();
        }
    }
}