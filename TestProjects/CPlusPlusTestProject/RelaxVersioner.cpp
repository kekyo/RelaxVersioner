// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.0.0, Do not edit.
// Generated date: Mon, 09 Sep 2019 01:26:37 GMT

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

[assembly: AssemblyVersionAttribute("0.10.1")];
[assembly: AssemblyFileVersionAttribute("2019.9.9.18794")];
[assembly: AssemblyInformationalVersionAttribute("b1a0a116a72a770ee8109fc0d3e61bf543d281f4")];
[assembly: AssemblyVersionMetadataAttribute("Date","Mon, 09 Sep 2019 01:26:29 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Updated self host and test projects version.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Mon, 09 Sep 2019 01:26:37 GMT")];

