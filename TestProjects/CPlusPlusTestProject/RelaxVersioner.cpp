// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.1.0, Do not edit.
// Generated date: Mon, 09 Sep 2019 02:06:02 GMT

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
[assembly: AssemblyFileVersionAttribute("2019.9.9.19571")];
[assembly: AssemblyInformationalVersionAttribute("a7af77094ce167418fdefe7a1f90b1e3aec140fc")];
[assembly: AssemblyVersionMetadataAttribute("Date","Mon, 09 Sep 2019 01:52:22 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Updated self host and test projects version.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Mon, 09 Sep 2019 02:06:02 GMT")];

