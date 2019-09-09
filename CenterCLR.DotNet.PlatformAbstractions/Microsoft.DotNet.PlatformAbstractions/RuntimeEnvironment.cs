using Microsoft.DotNet.PlatformAbstractions.Native;
using System;

namespace Microsoft.DotNet.PlatformAbstractions
{
	public static class RuntimeEnvironment
	{
		private static readonly string OverrideEnvironmentVariableName = "DOTNET_RUNTIME_ID";

		public static Platform OperatingSystemPlatform
		{
			get;
		} = PlatformApis.GetOSPlatform();


		public static string OperatingSystemVersion
		{
			get;
		} = PlatformApis.GetOSVersion();


		public static string OperatingSystem
		{
			get;
		} = PlatformApis.GetOSName();


		public static string RuntimeArchitecture
		{
			get;
		} = GetArch();


		private static string GetArch()
		{
			if (!Environment.Is64BitProcess)
			{
				return "x86";
			}
			return "x64";
		}

		public static string GetRuntimeIdentifier()
		{
			return Environment.GetEnvironmentVariable(OverrideEnvironmentVariableName) ?? (GetRIDOS() + GetRIDVersion() + GetRIDArch());
		}

		private static string GetRIDArch()
		{
			return $"-{RuntimeArchitecture}";
		}

		private static string GetRIDVersion()
		{
			switch (OperatingSystemPlatform)
			{
			case Platform.Windows:
				return GetWindowsProductVersion();
			case Platform.Linux:
				if (string.IsNullOrEmpty(OperatingSystemVersion))
				{
					return string.Empty;
				}
				return $".{OperatingSystemVersion}";
			case Platform.Darwin:
				return $".{OperatingSystemVersion}";
			case Platform.FreeBSD:
				return $".{OperatingSystemVersion}";
			default:
				return string.Empty;
			}
		}

		private static string GetWindowsProductVersion()
		{
			Version version = Version.Parse(OperatingSystemVersion);
			if (version.Major == 6)
			{
				if (version.Minor == 1)
				{
					return "7";
				}
				if (version.Minor == 2)
				{
					return "8";
				}
				if (version.Minor == 3)
				{
					return "81";
				}
			}
			else if (version.Major >= 10)
			{
				return version.Major.ToString();
			}
			return string.Empty;
		}

		private static string GetRIDOS()
		{
			switch (OperatingSystemPlatform)
			{
			case Platform.Windows:
				return "win";
			case Platform.Linux:
				return OperatingSystem.ToLowerInvariant();
			case Platform.Darwin:
				return "osx";
			case Platform.FreeBSD:
				return "freebsd";
			default:
				return "unknown";
			}
		}
	}
}
