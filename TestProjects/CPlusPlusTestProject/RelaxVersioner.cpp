// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.25.0
// Do not edit.
// Generated date: Thu, 22 Aug 2019 08:15:22 GMT

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

[assembly: AssemblyVersionAttribute("0.9.25")];
[assembly: AssemblyFileVersionAttribute("2019.8.22.26855")];
[assembly: AssemblyInformationalVersionAttribute("67c23b70f290f07dfb6d40bdba7af85903038c2f")];
[assembly: AssemblyVersionMetadataAttribute("Build","Thu, 22 Aug 2019 05:55:11 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Validated test projects on current version package.")];

