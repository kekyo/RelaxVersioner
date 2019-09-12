// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.49.0, Do not edit.
// Generated date: Thu, 12 Sep 2019 13:24:16 GMT

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

[assembly: AssemblyVersionAttribute("0.10.51")];
[assembly: AssemblyFileVersionAttribute("2019.9.12.40281")];
[assembly: AssemblyInformationalVersionAttribute("5a5996cd4223b091d4e1347f9bacf3ae64849fcf")];
[assembly: AssemblyVersionMetadataAttribute("Date","Thu, 12 Sep 2019 13:22:42 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","wip")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Thu, 12 Sep 2019 13:24:16 GMT")];

