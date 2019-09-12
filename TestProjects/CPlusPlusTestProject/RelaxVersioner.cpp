// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.10.39.0, Do not edit.
// Generated date: Thu, 12 Sep 2019 08:01:05 GMT

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

[assembly: AssemblyVersionAttribute("0.10.39")];
[assembly: AssemblyFileVersionAttribute("2019.9.12.30534")];
[assembly: AssemblyInformationalVersionAttribute("954ec7a84d1ea91e84f18eb26345ea84d9ae7b62")];
[assembly: AssemblyVersionMetadataAttribute("Date","Thu, 12 Sep 2019 07:57:48 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","devel")];
[assembly: AssemblyVersionMetadataAttribute("Tags","")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Fixed rv platform monitor.")];
[assembly: AssemblyVersionMetadataAttribute("Build","")];
[assembly: AssemblyVersionMetadataAttribute("Generated","Thu, 12 Sep 2019 08:01:05 GMT")];

