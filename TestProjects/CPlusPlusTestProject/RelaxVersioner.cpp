// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.20.0, Do not edit.
// Generated date: Tue, 10 Sep 2019 14:00:15 GMT

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

[assembly: AssemblyVersionAttribute("0.10.20")];
[assembly: AssemblyFileVersionAttribute("2019.9.10.40366")];
[assembly: AssemblyInformationalVersionAttribute("c111ff1efab5e2b77fce15bd306161b6eca7f7bd")];
[assembly: AssemblyVersionMetadataAttribute("Date","Tue, 10 Sep 2019 13:25:32 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Updated self host and test projects version.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Tue, 10 Sep 2019 14:00:15 GMT")];

