﻿using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace AutoClickerMacro
{
    public partial class Form1 : Form
    {
        [DllImport("user32.dll", SetLastError = true)]
        static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);

        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;

        private bool _isClicking = false;
        private MouseHook _mouseHook;
        public Form1()
        {
            InitializeComponent();
            _mouseHook = new MouseHook();
            _mouseHook.MouseAction += new EventHandler<MouseEventArgs>(OnMouseAction);
            _mouseHook.Hook();
        }

        private void OnMouseAction(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && Control.ModifierKeys == Keys.Control)
            {
                if (!_isClicking)
                {
                    _isClicking = true;
                    StartClicking();
                }
            }
            else if (e.Button == MouseButtons.Right && Control.ModifierKeys == Keys.Control)
            {
                if (_isClicking)
                {
                    _isClicking = false;
                }
            }
        }

        private void StartClicking()
        {
            Thread clickThread = new Thread(() =>
            {
                while (_isClicking)
                {
                    DoMouseClick((uint)Cursor.Position.X, (uint)Cursor.Position.Y);
                    Thread.Sleep((int)numericUpDown1.Value); // 클릭 간격 (밀리초)
                }
            });
            clickThread.IsBackground = true;
            clickThread.Start();
        }

        private void DoMouseClick(uint x, uint y)
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _mouseHook.Unhook();
            base.OnFormClosing(e);
        }
    }
}
