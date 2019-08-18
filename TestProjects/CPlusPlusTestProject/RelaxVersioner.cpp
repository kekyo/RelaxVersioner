// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.1.0
// Do not edit.
// Generated date: Sun, 18 Aug 2019 13:23:36 GMT

#include "stdafx.h"

namespace System
{
	namespace Reflection
	{
		[System::AttributeUsage(System::AttributeTargets::Assembly, AllowMultiple = true, Inherited = false)]
		private ref class AssemblyVersionMetadataAttribute sealed : public System::Attribute
		{
		private:
			System::String^ key_;
			System::String^ value_;
		public:
			AssemblyVersionMetadataAttribute(System::String^ key, System::String^ value)
				: key_(key), value_(value) { }
			property System::String^ Key { System::String^ get() { return key_; } }
			property System::String^ Value { System::String^ get() { return value_; } }
		};
	}
}

using namespace System::Reflection;
using namespace System::Runtime::InteropServices;

[assembly: AssemblyVersionAttribute("0.9.1")];
[assembly: AssemblyFileVersionAttribute("2019.8.18.38636")];
[assembly: AssemblyInformationalVersionAttribute("3b6510b099971163f959c0fa69331417c4499758")];
[assembly: AssemblyVersionMetadataAttribute("Build","Sun, 18 Aug 2019 12:27:53 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.1")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Upgrading new MSBuild scripts (formaly .NET Core envs)")];

