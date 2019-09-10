using System;
using System.Runtime.InteropServices;

internal static class Interop
{
	internal class NtDll
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

		[DllImport("ntdll.dll")]
		private static extern int RtlGetVersion(out RTL_OSVERSIONINFOEX lpVersionInformation);

		internal static string RtlGetVersion()
		{
			RTL_OSVERSIONINFOEX lpVersionInformation = default(RTL_OSVERSIONINFOEX);
			lpVersionInformation.dwOSVersionInfoSize = (uint)Marshal.SizeOf((object)lpVersionInformation);
			if (RtlGetVersion(out lpVersionInformation) == 0)
			{
				return string.Format("{0} {1}.{2}.{3} {4}", "Microsoft Windows", lpVersionInformation.dwMajorVersion, lpVersionInformation.dwMinorVersion, lpVersionInformation.dwBuildNumber, lpVersionInformation.szCSDVersion);
			}
			return "Microsoft Windows";
		}
	}

	internal static class Libraries
	{
		internal const string Advapi32 = "advapi32.dll";

		internal const string BCrypt = "BCrypt.dll";

		internal const string Combase = "combase.dll";

		internal const string Console_L1 = "api-ms-win-core-console-l1-1-0.dll";

		internal const string Console_L2 = "api-ms-win-core-console-l2-1-0.dll";

		internal const string CoreFile_L1 = "api-ms-win-core-file-l1-1-0.dll";

		internal const string CoreFile_L1_2 = "api-ms-win-core-file-l1-2-0.dll";

		internal const string CoreFile_L2 = "api-ms-win-core-file-l2-1-0.dll";

		internal const string Crypt32 = "crypt32.dll";

		internal const string Debug = "api-ms-win-core-debug-l1-1-0.dll";

		internal const string Error_L1 = "api-ms-win-core-winrt-error-l1-1-0.dll";

		internal const string ErrorHandling = "api-ms-win-core-errorhandling-l1-1-0.dll";

		internal const string Eventing = "api-ms-win-eventing-provider-l1-1-0.dll";

		internal const string Handle = "api-ms-win-core-handle-l1-1-0.dll";

		internal const string Heap = "api-ms-win-core-heap-obsolete-l1-1-0.dll";

		internal const string Heap_L1 = "api-ms-win-core-heap-l1-1-0.dll";

		internal const string IO = "api-ms-win-core-io-l1-1-0.dll";

		internal const string IpHlpApi = "iphlpapi.dll";

		internal const string Kernel32 = "kernel32.dll";

		internal const string Kernel32_L1 = "api-ms-win-core-kernel32-legacy-l1-1-1.dll";

		internal const string Kernel32_L2 = "api-ms-win-core-kernel32-legacy-l1-1-0.dll";

		internal const string Keyboard = "ext-ms-win-ntuser-keyboard-l1-2-1.dll";

		internal const string LibraryLoader = "api-ms-win-core-libraryloader-l1-1-0.dll";

		internal const string Localization = "api-ms-win-core-localization-l1-2-0.dll";

		internal const string Memory_L1_0 = "api-ms-win-core-memory-l1-1-0.dll";

		internal const string Memory_L1_1 = "api-ms-win-core-memory-l1-1-1.dll";

		internal const string Memory_L1_2 = "api-ms-win-core-memory-l1-1-2.dll";

		internal const string Memory_L1_3 = "api-ms-win-core-memory-l1-1-3.dll";

		internal const string NCrypt = "ncrypt.dll";

		internal const string NtDll = "ntdll.dll";

		internal const string OleAut32 = "oleaut32.dll";

		internal const string Pipe = "api-ms-win-core-namedpipe-l1-1-0.dll";

		internal const string Pipe_L2 = "api-ms-win-core-namedpipe-l1-2-1.dll";

		internal const string ProcessEnvironment = "api-ms-win-core-processenvironment-l1-1-0.dll";

		internal const string ProcessThread_L1 = "api-ms-win-core-processthreads-l1-1-0.dll";

		internal const string ProcessThread_L1_1 = "api-ms-win-core-processthreads-l1-1-1.dll";

		internal const string ProcessThread_L1_2 = "api-ms-win-core-processthreads-l1-1-2.dll";

		internal const string ProcessTopology = "api-ms-win-core-processtopology-obsolete-l1-1-0.dll";

		internal const string Profile = "api-ms-win-core-profile-l1-1-0.dll";

		internal const string Psapi = "api-ms-win-core-psapi-l1-1-0.dll";

		internal const string Psapi_Obsolete = "api-ms-win-core-psapi-obsolete-l1-1-0.dll";

		internal const string Registry_L1 = "api-ms-win-core-registry-l1-1-0.dll";

		internal const string Registry_L2 = "api-ms-win-core-registry-l2-1-0.dll";

		internal const string RoBuffer = "api-ms-win-core-winrt-robuffer-l1-1-0.dll";

		internal const string SecurityBase = "api-ms-win-security-base-l1-1-0.dll";

		internal const string SecurityCpwl = "api-ms-win-security-cpwl-l1-1-0.dll";

		internal const string SecurityCryptoApi = "api-ms-win-security-cryptoapi-l1-1-0.dll";

		internal const string SecurityLsa = "api-ms-win-security-lsalookup-l2-1-0.dll";

		internal const string SecurityLsaPolicy = "api-ms-win-security-lsapolicy-l1-1-0.dll";

		internal const string SecurityProvider = "api-ms-win-security-provider-l1-1-0.dll";

		internal const string SecuritySddl = "api-ms-win-security-sddl-l1-1-0.dll";

		internal const string ServiceCore = "api-ms-win-service-core-l1-1-1.dll";

		internal const string ServiceMgmt_L1 = "api-ms-win-service-management-l1-1-0.dll";

		internal const string ServiceMgmt_L2 = "api-ms-win-service-management-l2-1-0.dll";

		internal const string ServiceWinSvc = "api-ms-win-service-winsvc-l1-1-0.dll";

		internal const string Sspi = "sspicli.dll";

		internal const string String_L1 = "api-ms-win-core-string-l1-1-0.dll";

		internal const string Synch = "api-ms-win-core-synch-l1-1-0.dll";

		internal const string SystemInfo_L1_1 = "api-ms-win-core-sysinfo-l1-1-0.dll";

		internal const string SystemInfo_L1_2 = "api-ms-win-core-sysinfo-l1-2-0.dll";

		internal const string ThreadPool = "api-ms-win-core-threadpool-l1-2-0.dll";

		internal const string User32 = "user32.dll";

		internal const string Util = "api-ms-win-core-util-l1-1-0.dll";

		internal const string Version = "api-ms-win-core-version-l1-1-0.dll";

		internal const string WinHttp = "winhttp.dll";

		internal const string Winsock = "Ws2_32.dll";

		internal const string Wow64 = "api-ms-win-core-wow64-l1-1-0.dll";

		internal const string Ws2_32 = "ws2_32.dll";

		internal const string Zlib = "clrcompression.dll";
	}

	internal class mincore
	{
		internal struct SYSTEM_INFO
		{
			internal ushort wProcessorArchitecture;

			internal ushort wReserved;

			internal int dwPageSize;

			internal IntPtr lpMinimumApplicationAddress;

			internal IntPtr lpMaximumApplicationAddress;

			internal IntPtr dwActiveProcessorMask;

			internal int dwNumberOfProcessors;

			internal int dwProcessorType;

			internal int dwAllocationGranularity;

			internal short wProcessorLevel;

			internal short wProcessorRevision;
		}

		internal enum ProcessorArchitecture : ushort
		{
			Processor_Architecture_INTEL = 0,
			Processor_Architecture_ARM = 5,
			Processor_Architecture_IA64 = 6,
			Processor_Architecture_AMD64 = 9,
			Processor_Architecture_ARM64 = 12,
			Processor_Architecture_UNKNOWN = ushort.MaxValue
		}

		[DllImport("api-ms-win-core-sysinfo-l1-2-0.dll")]
		internal static extern void GetNativeSystemInfo(out SYSTEM_INFO lpSystemInfo);

		[DllImport("api-ms-win-core-sysinfo-l1-1-0.dll")]
		internal static extern void GetSystemInfo(out SYSTEM_INFO lpSystemInfo);
	}
}
