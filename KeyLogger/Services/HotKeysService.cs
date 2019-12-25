using KeyLogger.Enums;
using System;
using System.Windows.Forms;
using static KeyLogger.WinApis.User32;

namespace KeyLogger.Services
{
	internal static class HotKeysService
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="key"></param>
		/// <param name="handle"></param>
		/// <returns>Key hash code</returns>
		internal static int SetHotKey(Keys key, IntPtr handle)
		{
			int modifiers = 0;
			if ((key & Keys.Alt) == Keys.Alt)
				modifiers |= (int)CombineKeys.MOD_ALT;
			if ((key & Keys.Control) == Keys.Control)
				modifiers |= (int)CombineKeys.MOD_CONTROL;
			if ((key & Keys.Shift) == Keys.Shift)
				modifiers |= (int)CombineKeys.MOD_SHIFT;
			Keys keys = key & ~Keys.Control & ~Keys.Shift & ~Keys.Alt;
			var keyId = key.GetHashCode();
			RegisterHotKey(handle, keyId, modifiers, (int)keys);
			return keyId;			
		}
	}
}
