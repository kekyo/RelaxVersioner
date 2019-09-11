// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.24.0, Do not edit.
// Generated date: Wed, 11 Sep 2019 01:03:24 GMT

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

[assembly: AssemblyVersionAttribute("0.10.24")];
[assembly: AssemblyFileVersionAttribute("2019.9.11.17931")];
[assembly: AssemblyInformationalVersionAttribute("fe45961c123be052c273aa22428ad684f33dab5e")];
[assembly: AssemblyVersionMetadataAttribute("Date","Wed, 11 Sep 2019 00:57:42 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Omitted net40/net45 platform because there caused conflicting version for MSBuild.Framework assemblies.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Wed, 11 Sep 2019 01:03:24 GMT")];

