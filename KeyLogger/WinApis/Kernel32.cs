using System;
using System.Runtime.InteropServices;

namespace KeyLogger.WinApis
{
	public class Kernel32
	{
		[DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
		public static extern IntPtr GetModuleHandle( string lpModuleName );

		[DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
		public static extern uint GetCurrentThreadId();

		[DllImport( "kernel32.dll" )]
		public static extern IntPtr GetCurrentProcess();

		[DllImport( "kernel32.dll" )]
		public static extern IntPtr GetConsoleWindow(); 
	}
}
