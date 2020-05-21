using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;

namespace ScreenToText
{
    class Program
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        static bool ctrlPressed = false;
        static bool ctrlSPressed = false;

        [STAThread]
        [Obsolete]
        public static void Main()
        {
            AppDomain.CurrentDomain.AppendPrivatePath(path: @"./libs");
            _hookID = SetHook(_proc);
            SnippingTool.AreaSelected += DoScreen;
            Application.Run();
            UnhookWindowsHookEx(_hookID);
        }

        static void DoScreen(object sender, EventArgs e)
        {
            Image bmp = SnippingTool.Image;
            if (bmp != null)
            {
                Console.WriteLine("Success: screenshot sent to clipboard!");
                Clipboard.SetImage(bmp);
                //ConvertImage.ConvertImageToTextIron();
                ConvertImage.ConvertImageToTextVS();
            }
            else
            {
                Console.WriteLine("Error: no screenshot taken!");
            }
        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam) // Defining a keyboard shortcut
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                if (vkCode == 162 || vkCode == 163) // 162 is Left Ctrl, 163 is Right Ctrl
                {
                    ctrlPressed = true;
                }
                else if (vkCode == 83 && ctrlPressed == true) // "S"
                {
                    ctrlPressed = false;
                    ctrlSPressed = true;
                }
                else if (vkCode == 68 && ctrlSPressed == true) // "D"
                {
                    ctrlPressed = false;
                    ctrlSPressed = false;
                    Console.WriteLine("\"CTRL + S + D\" is PRESSED!");
                    SnippingTool.Snip();
                }
                else
                {
                    ctrlPressed = false;
                    ctrlSPressed = false;
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}