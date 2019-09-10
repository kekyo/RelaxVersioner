// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.13.0, Do not edit.
// Generated date: Tue, 10 Sep 2019 04:30:33 GMT

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

[assembly: AssemblyVersionAttribute("0.10.14")];
[assembly: AssemblyFileVersionAttribute("2019.9.10.24280")];
[assembly: AssemblyInformationalVersionAttribute("143ba1ffd7f9ec74fd9a579d117f9e1bc71c3f92")];
[assembly: AssemblyVersionMetadataAttribute("Date","Tue, 10 Sep 2019 04:29:21 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Improved native library loader.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Tue, 10 Sep 2019 04:30:33 GMT")];

