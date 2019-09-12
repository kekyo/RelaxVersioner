// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.1.0.0.0, Do not edit.
// Generated date: Thu, 12 Sep 2019 13:54:47 GMT

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

[assembly: AssemblyVersionAttribute("1.0.0")];
[assembly: AssemblyFileVersionAttribute("2019.9.12.40881")];
[assembly: AssemblyInformationalVersionAttribute("967c229a68a251fade829e789284cbc5848b1398")];
[assembly: AssemblyVersionMetadataAttribute("Date","Thu, 12 Sep 2019 13:42:42 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","master")];
[assembly: AssemblyVersionMetadataAttribute("Tags","1.0.0")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Merge branch 'devel'")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Thu, 12 Sep 2019 13:54:47 GMT")];

