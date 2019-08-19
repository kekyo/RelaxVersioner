// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.14.0
// Do not edit.
// Generated date: Mon, 19 Aug 2019 22:04:03 GMT

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

[assembly: AssemblyVersionAttribute("0.9.14")];
[assembly: AssemblyFileVersionAttribute("2019.8.20.1145")];
[assembly: AssemblyInformationalVersionAttribute("b0034dc22a52b6da981f249c2cd715da1c8a6501")];
[assembly: AssemblyVersionMetadataAttribute("Build","Mon, 19 Aug 2019 15:38:11 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.14")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Added developmentdependency attribute.")];

