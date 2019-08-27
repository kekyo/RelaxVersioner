// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.63.0
// Do not edit.
// Generated date: Tue, 27 Aug 2019 22:44:50 GMT

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

[assembly: AssemblyVersionAttribute("0.9.63")];
[assembly: AssemblyFileVersionAttribute("2019.8.28.13820")];
[assembly: AssemblyInformationalVersionAttribute("2b6a286c102e3190f342ef461db1010250f28c05")];
[assembly: AssemblyVersionMetadataAttribute("Build","Tue, 27 Aug 2019 22:40:40 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.63")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Fixed failure msbuild on linux environment because the mono loader interpreted dll.config file.")];

