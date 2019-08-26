// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.34.0
// Do not edit.
// Generated date: Fri, 23 Aug 2019 09:50:28 GMT

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

[assembly: AssemblyVersionAttribute("0.9.34")];
[assembly: AssemblyFileVersionAttribute("2019.8.23.33377")];
[assembly: AssemblyInformationalVersionAttribute("c8b8294f4485edf7b133e1955cc102e912df594c")];
[assembly: AssemblyVersionMetadataAttribute("Build","Fri, 23 Aug 2019 09:32:35 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.34")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Fixed AddDllDirectory charset.")];

