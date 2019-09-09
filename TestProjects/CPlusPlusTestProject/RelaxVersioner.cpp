// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.6.0, Do not edit.
// Generated date: Mon, 09 Sep 2019 12:37:54 GMT

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

[assembly: AssemblyVersionAttribute("0.10.7")];
[assembly: AssemblyFileVersionAttribute("2019.9.9.38896")];
[assembly: AssemblyInformationalVersionAttribute("29492797c17157db7d8f10fcc3d436ffc2480545")];
[assembly: AssemblyVersionMetadataAttribute("Date","Mon, 09 Sep 2019 12:36:32 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Updated readme")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Mon, 09 Sep 2019 12:37:54 GMT")];

