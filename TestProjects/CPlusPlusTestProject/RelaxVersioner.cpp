// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.25.0
// Do not edit.
// Generated date: Thu, 22 Aug 2019 05:43:56 GMT

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
[assembly: AssemblyFileVersionAttribute("2019.8.22.24969")];
[assembly: AssemblyInformationalVersionAttribute("cba8f3d89af07ca06156836c934732d6bd9f9971")];
[assembly: AssemblyVersionMetadataAttribute("Build","Thu, 22 Aug 2019 04:52:19 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.25")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Preparing self host for versioning.")];

