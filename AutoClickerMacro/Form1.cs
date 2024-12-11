using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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
            numericUpDown1.Minimum = 1; // 최소 1분
            numericUpDown1.Maximum = 3600; // 최대 60시간 (밀리초)
            numericUpDown1.Value = 5; // 기본값 5분
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

        private async void button1_Click(object sender, EventArgs e)
        {
            // 설정된 시간 대기
            int delayInMinutes = (int)numericUpDown1.Value;
            int delayInMilliseconds = delayInMinutes * 60 * 1000; // 분을 밀리초로 변환
            await Task.Delay(delayInMilliseconds); // 대기

            // 마우스 클릭 이벤트 발생
            PerformMouseClick();

            // 프로그램 종료
            Application.Exit();
        }
        private void PerformMouseClick()
        {
            // 현재 마우스 위치 가져오기
            uint x = (uint)Cursor.Position.X;
            uint y = (uint)Cursor.Position.Y;

            // 마우스 클릭 시뮬레이션
            mouse_event(MOUSEEVENTF_LEFTDOWN | MOUSEEVENTF_LEFTUP, x, y, 0, 0);
        }
    }
}
