// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.66.0
// Do not edit.
// Generated date: Wed, 28 Aug 2019 07:57:22 GMT

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

[assembly: AssemblyVersionAttribute("0.9.66")];
[assembly: AssemblyFileVersionAttribute("2019.8.28.30425")];
[assembly: AssemblyInformationalVersionAttribute("bb1f900d7ed0f50f382d842feb6543384ea18263")];
[assembly: AssemblyVersionMetadataAttribute("Build","Wed, 28 Aug 2019 07:54:10 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.66")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Fixed trimming prepeded directory separator.")];

