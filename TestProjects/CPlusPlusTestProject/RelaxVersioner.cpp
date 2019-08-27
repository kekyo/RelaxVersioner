// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.43.0
// Do not edit.
// Generated date: Tue, 27 Aug 2019 00:43:02 GMT

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

[assembly: AssemblyVersionAttribute("0.9.43")];
[assembly: AssemblyFileVersionAttribute("2019.8.27.16853")];
[assembly: AssemblyInformationalVersionAttribute("08a9f5f29ce715a50221c2e7cf0ec77d2796c011")];
[assembly: AssemblyVersionMetadataAttribute("Build","Tue, 27 Aug 2019 00:21:46 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.43")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Fixed nuspec path.")];

