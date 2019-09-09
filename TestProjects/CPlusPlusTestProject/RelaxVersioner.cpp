// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.3.0, Do not edit.
// Generated date: Mon, 09 Sep 2019 09:28:16 GMT

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

[assembly: AssemblyVersionAttribute("0.10.3")];
[assembly: AssemblyFileVersionAttribute("2019.9.9.33123")];
[assembly: AssemblyInformationalVersionAttribute("bee7599a575a676ed0e158f3c80c22f055cbe610")];
[assembly: AssemblyVersionMetadataAttribute("Date","Mon, 09 Sep 2019 09:24:06 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Improved fork analysis.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Mon, 09 Sep 2019 09:28:16 GMT")];

