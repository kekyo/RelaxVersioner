// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.41.0
// Do not edit.
// Generated date: Mon, 26 Aug 2019 02:30:20 GMT

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

[assembly: AssemblyVersionAttribute("0.9.41")];
[assembly: AssemblyFileVersionAttribute("2019.8.26.20466")];
[assembly: AssemblyInformationalVersionAttribute("da76025c7b117f158e6b49ae4891f779abbf4da4")];
[assembly: AssemblyVersionMetadataAttribute("Build","Mon, 26 Aug 2019 02:22:12 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.41")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","wip")];

