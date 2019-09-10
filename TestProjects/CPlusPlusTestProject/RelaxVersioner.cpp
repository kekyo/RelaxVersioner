// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.17.0, Do not edit.
// Generated date: Tue, 10 Sep 2019 08:29:40 GMT

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

[assembly: AssemblyVersionAttribute("0.10.17")];
[assembly: AssemblyFileVersionAttribute("2019.9.10.29774")];
[assembly: AssemblyInformationalVersionAttribute("5432af855399c7896c246fc9be943b54c1ce72d8")];
[assembly: AssemblyVersionMetadataAttribute("Date","Tue, 10 Sep 2019 07:32:29 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Rewrote fork analysis.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Tue, 10 Sep 2019 08:29:40 GMT")];

