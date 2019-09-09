using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Microsoft.DotNet.PlatformAbstractions.Native
{
	internal static class PlatformApis
	{
		private class DistroInfo
		{
			public string Id;

			public string VersionId;
		}

		private static readonly Lazy<Platform> _platform = new Lazy<Platform>(DetermineOSPlatform);

		private static readonly Lazy<DistroInfo> _distroInfo = new Lazy<DistroInfo>(LoadDistroInfo);

		public static string GetOSName()
		{
			switch (GetOSPlatform())
			{
			case Platform.Windows:
				return "Windows";
			case Platform.Linux:
				return GetDistroId() ?? "Linux";
			case Platform.Darwin:
				return "Mac OS X";
			case Platform.FreeBSD:
				return "FreeBSD";
			default:
				return "Unknown";
			}
		}

		public static string GetOSVersion()
		{
			switch (GetOSPlatform())
			{
			case Platform.Windows:
				return NativeMethods.Windows.RtlGetVersion() ?? string.Empty;
			case Platform.Linux:
				return GetDistroVersionId() ?? string.Empty;
			case Platform.Darwin:
				return GetDarwinVersion() ?? string.Empty;
			case Platform.FreeBSD:
				return GetFreeBSDVersion() ?? string.Empty;
			default:
				return string.Empty;
			}
		}

		private static string GetDarwinVersion()
		{
			string kernelRelease = NativeMethods.Darwin.GetKernelRelease();
			if (!Version.TryParse(kernelRelease, out Version result) || result.Major < 5)
			{
				return "10.0";
			}
			return $"10.{result.Major - 4}";
		}

		private static string GetFreeBSDVersion()
		{
			string oSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription;
			try
			{
				return System.Runtime.InteropServices.RuntimeInformation.OSDescription.Split()[1].Split('.')[0];
			}
			catch
			{
			}
			return string.Empty;
		}

		public static Platform GetOSPlatform()
		{
			return _platform.Value;
		}

		private static string GetDistroId()
		{
			return _distroInfo.Value?.Id;
		}

		private static string GetDistroVersionId()
		{
			return _distroInfo.Value?.VersionId;
		}

		private static DistroInfo LoadDistroInfo()
		{
			DistroInfo distroInfo = null;
			if (File.Exists("/etc/os-release"))
			{
				string[] array = File.ReadAllLines("/etc/os-release");
				distroInfo = new DistroInfo();
				string[] array2 = array;
				foreach (string text in array2)
				{
					if (text.StartsWith("ID=", StringComparison.Ordinal))
					{
						distroInfo.Id = text.Substring(3).Trim('"', '\'');
					}
					else if (text.StartsWith("VERSION_ID=", StringComparison.Ordinal))
					{
						distroInfo.VersionId = text.Substring(11).Trim('"', '\'');
					}
				}
			}
			else if (File.Exists("/etc/redhat-release"))
			{
				string[] array3 = File.ReadAllLines("/etc/redhat-release");
				if (array3.Length >= 1)
				{
					string text2 = array3[0];
					if (text2.StartsWith("Red Hat Enterprise Linux Server release 6.") || text2.StartsWith("CentOS release 6."))
					{
						distroInfo = new DistroInfo();
						distroInfo.Id = "rhel";
						distroInfo.VersionId = "6";
					}
				}
			}
			if (distroInfo != null)
			{
				distroInfo = NormalizeDistroInfo(distroInfo);
			}
			return distroInfo;
		}

		private static DistroInfo NormalizeDistroInfo(DistroInfo distroInfo)
		{
			int num = distroInfo.VersionId?.IndexOf('.') ?? (-1);
			if (num != -1 && distroInfo.Id == "alpine")
			{
				num = distroInfo.VersionId.IndexOf('.', num + 1);
			}
			if (num != -1 && (distroInfo.Id == "rhel" || distroInfo.Id == "alpine"))
			{
				distroInfo.VersionId = distroInfo.VersionId.Substring(0, num);
			}
			return distroInfo;
		}

		private static Platform DetermineOSPlatform()
		{
			int platform = (int)Environment.OSVersion.Platform;
			if (platform != 4 && platform != 6 && platform != 128)
			{
				return Platform.Windows;
			}
			try
			{
				string uname = NativeMethods.Unix.GetUname();
				if (string.Equals(uname, "Darwin", StringComparison.OrdinalIgnoreCase))
				{
					return Platform.Darwin;
				}
				if (string.Equals(uname, "Linux", StringComparison.OrdinalIgnoreCase))
				{
					return Platform.Linux;
				}
				if (string.Equals(uname, "FreeBSD", StringComparison.OrdinalIgnoreCase))
				{
					return Platform.FreeBSD;
				}
			}
			catch
			{
			}
			return Platform.Unknown;
		}
	}
}
