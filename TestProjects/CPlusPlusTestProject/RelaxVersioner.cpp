// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.69.0, Do not edit.
// Generated date: Thu, 29 Aug 2019 14:09:21 GMT

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

[assembly: AssemblyVersionAttribute("0.9.69")];
[assembly: AssemblyFileVersionAttribute("2019.8.29.41591")];
[assembly: AssemblyInformationalVersionAttribute("e0347c5229d04dd38151dadb2fbe18ca26734528")];
[assembly: AssemblyVersionMetadataAttribute("Date","Thu, 29 Aug 2019 14:06:22 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.69")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Fixed contains runtime directory when building final result.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Thu, 29 Aug 2019 14:09:21 GMT")];

