// This is auto-generated version information attributes by CenterCLR.RelaxVersioner.0.9.65.0
// Do not edit.
// Generated date: Wed, 28 Aug 2019 07:43:26 GMT

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

[assembly: AssemblyVersionAttribute("0.9.65")];
[assembly: AssemblyFileVersionAttribute("2019.8.28.30009")];
[assembly: AssemblyInformationalVersionAttribute("2eff2ea913a0ed639121bfa9946e72b43ad15dd8")];
[assembly: AssemblyVersionMetadataAttribute("Build","Wed, 28 Aug 2019 07:40:19 GMT")];
[assembly: AssemblyVersionMetadataAttribute("Branch","")];
[assembly: AssemblyVersionMetadataAttribute("Tags","0.9.65")];
[assembly: AssemblyVersionMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: AssemblyVersionMetadataAttribute("Message","Improved probing repository.")];

