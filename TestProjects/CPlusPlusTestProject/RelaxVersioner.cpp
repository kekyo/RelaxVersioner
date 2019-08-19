// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.1.0
// Do not edit.
// Generated date: Mon, 19 Aug 2019 02:11:53 GMT

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

[assembly: AssemblyVersionAttribute("0.9.4")];
[assembly: AssemblyFileVersionAttribute("2019.8.19.19534")];
[assembly: AssemblyInformationalVersionAttribute("f9f26da013074e51e792c60a0ef1d3b40f647c09")];
[assembly: AssemblyVersionMetadataAttribute("Build","Mon, 19 Aug 2019 01:51:09 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.4")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Adjusted asset flags.")];

