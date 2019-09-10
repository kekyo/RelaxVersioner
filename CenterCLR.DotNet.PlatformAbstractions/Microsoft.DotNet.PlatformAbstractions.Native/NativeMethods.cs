using System;
using System.Runtime.InteropServices;

namespace Microsoft.DotNet.PlatformAbstractions.Native
{
	internal static class NativeMethods
	{
		public static class Darwin
		{
			private const int CTL_KERN = 1;

			private const int KERN_OSRELEASE = 2;

			public unsafe static string GetKernelRelease()
			{
				int* ptr = stackalloc int[2];
				*ptr = 1;
				ptr[1] = 2;
				byte* ptr2 = stackalloc byte[32];
				uint* ptr3 = stackalloc uint[1];
				*ptr3 = 32u;
				try
				{
					if (sysctl(ptr, 2u, ptr2, ptr3, IntPtr.Zero, 0u) == 0 && *ptr3 < 32)
					{
						return Marshal.PtrToStringAnsi((IntPtr)(void*)ptr2, (int)(*ptr3));
					}
				}
				catch (Exception inner)
				{
					throw new PlatformNotSupportedException("Error reading Darwin Kernel Version", inner);
				}
				throw new PlatformNotSupportedException("Unknown error reading Darwin Kernel Version");
			}

			[DllImport("libc")]
			private unsafe static extern int sysctl(int* name, uint namelen, byte* oldp, uint* oldlenp, IntPtr newp, uint newlen);
		}

		public static class Unix
		{
			public unsafe static string GetUname()
			{
				byte* value = stackalloc byte[2048];
				try
				{
					if (uname((IntPtr)(void*)value) == 0)
					{
						return Marshal.PtrToStringAnsi((IntPtr)(void*)value);
					}
				}
				catch (Exception inner)
				{
					throw new PlatformNotSupportedException("Error reading Unix name", inner);
				}
				throw new PlatformNotSupportedException("Unknown error reading Unix name");
			}

			[DllImport("libc")]
			private static extern int uname(IntPtr utsname);
		}

		public static class Windows
		{
			internal struct RTL_OSVERSIONINFOEX
			{
				internal uint dwOSVersionInfoSize;

				internal uint dwMajorVersion;

				internal uint dwMinorVersion;

				internal uint dwBuildNumber;

				internal uint dwPlatformId;

				[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
				internal string szCSDVersion;
			}

			[DllImport("ntdll")]
			private static extern int RtlGetVersion(out RTL_OSVERSIONINFOEX lpVersionInformation);

			internal static string RtlGetVersion()
			{
				RTL_OSVERSIONINFOEX lpVersionInformation = default(RTL_OSVERSIONINFOEX);
				lpVersionInformation.dwOSVersionInfoSize = (uint)Marshal.SizeOf((object)lpVersionInformation);
				if (RtlGetVersion(out lpVersionInformation) == 0)
				{
					return $"{lpVersionInformation.dwMajorVersion}.{lpVersionInformation.dwMinorVersion}.{lpVersionInformation.dwBuildNumber}";
				}
				return null;
			}
		}
	}
}
