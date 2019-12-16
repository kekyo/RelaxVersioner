// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.1.0.5.0, Do not edit.
// Generated date: Mon, 16 Dec 2019 14:32:24 GMT

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

[assembly: AssemblyVersionAttribute("1.0.5")];
[assembly: AssemblyFileVersionAttribute("2019.12.16.42088")];
[assembly: AssemblyInformationalVersionAttribute("9f8576565136b647cd94438e267f07b93e886ed7")];
[assembly: AssemblyVersionMetadataAttribute("Date","Mon, 16 Dec 2019 14:22:56 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","master")];
[assembly: AssemblyVersionMetadataAttribute("Tags","1.0.5")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Merge branch 'devel'")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Mon, 16 Dec 2019 14:32:24 GMT")];

