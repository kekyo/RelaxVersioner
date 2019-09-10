// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.19.0, Do not edit.
// Generated date: Tue, 10 Sep 2019 13:22:45 GMT

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

[assembly: AssemblyVersionAttribute("0.10.19")];
[assembly: AssemblyFileVersionAttribute("2019.9.10.40187")];
[assembly: AssemblyInformationalVersionAttribute("b26b6c8ffa35f88b29375465c24c0d3558663acf")];
[assembly: AssemblyVersionMetadataAttribute("Date","Tue, 10 Sep 2019 13:19:34 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Fixed using uninitialized repository.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Tue, 10 Sep 2019 13:22:45 GMT")];

