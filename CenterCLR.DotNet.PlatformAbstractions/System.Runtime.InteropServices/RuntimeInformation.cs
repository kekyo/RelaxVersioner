using System.Linq;
using System.Reflection;

namespace System.Runtime.InteropServices
{
    public static class RuntimeInformation
    {
        private static string s_osDescription = null;

        private static object s_osLock = new object();

        private static object s_processLock = new object();

        private static Architecture? s_osArch = null;

        private static Architecture? s_processArch = null;

        private const string FrameworkName = ".NET Framework";

        private static string s_frameworkDescription;

        public static string OSDescription
        {
            get
            {
                if (s_osDescription == null)
                {
                    s_osDescription = global::Interop.NtDll.RtlGetVersion();
                }
                return s_osDescription;
            }
        }

        public static Architecture OSArchitecture
        {
            get
            {
                lock (s_osLock)
                {
                    if (!s_osArch.HasValue)
                    {
                        global::Interop.mincore.GetNativeSystemInfo(out global::Interop.mincore.SYSTEM_INFO lpSystemInfo);
                        switch (lpSystemInfo.wProcessorArchitecture)
                        {
                        case 12:
                            s_osArch = Architecture.Arm64;
                            break;
                        case 5:
                            s_osArch = Architecture.Arm;
                            break;
                        case 9:
                            s_osArch = Architecture.X64;
                            break;
                        case 0:
                            s_osArch = Architecture.X86;
                            break;
                        }
                    }
                }
                return s_osArch.Value;
            }
        }

        public static Architecture ProcessArchitecture
        {
            get
            {
                lock (s_processLock)
                {
                    if (!s_processArch.HasValue)
                    {
                        global::Interop.mincore.GetSystemInfo(out global::Interop.mincore.SYSTEM_INFO lpSystemInfo);
                        switch (lpSystemInfo.wProcessorArchitecture)
                        {
                        case 12:
                            s_processArch = Architecture.Arm64;
                            break;
                        case 5:
                            s_processArch = Architecture.Arm;
                            break;
                        case 9:
                            s_processArch = Architecture.X64;
                            break;
                        case 0:
                            s_processArch = Architecture.X86;
                            break;
                        }
                    }
                }
                return s_processArch.Value;
            }
        }

        public static string FrameworkDescription
        {
            get
            {
                if (s_frameworkDescription == null)
                {
                    AssemblyFileVersionAttribute assemblyFileVersionAttribute = (AssemblyFileVersionAttribute)typeof(object).Assembly.GetCustomAttributes(typeof(AssemblyFileVersionAttribute), true).First();
                    s_frameworkDescription = string.Format("{0} {1}", new object[2]
                    {
                        ".NET Framework",
                        assemblyFileVersionAttribute.Version
                    });
                }
                return s_frameworkDescription;
            }
        }

        public static bool IsOSPlatform(OSPlatform osPlatform)
        {
            return OSPlatform.Windows == osPlatform;
        }
    }
}
