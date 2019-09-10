using System;
using System.IO;

namespace Microsoft.DotNet.PlatformAbstractions
{
	public static class ApplicationEnvironment
	{
		public static string ApplicationBasePath
		{
			get;
		} = GetApplicationBasePath();


		private static string GetApplicationBasePath()
		{
			string path = ((string)AppDomain.CurrentDomain.GetData("APP_CONTEXT_BASE_DIRECTORY")) ?? AppDomain.CurrentDomain.BaseDirectory;
			return Path.GetFullPath(path);
		}
	}
}
