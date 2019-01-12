## More On Rules

Every Rule must have a `name` attribute which specifies its name in the source.
For example, having

```xml
<Import>System.Reflection</Import>
<Rule name="AssemblyVersion">5.30</Rule>
```

In your `WriterRules` will generate

```csharp
using System.Reflection;

[assembly: AssemblyVersion(@"5.30")]
```

in **RelaxVersioner.cs**, assuming you're building a C# project.
Don't worry about the **@** sign; it's specific to C# / F# and just tells the compiler to ignore any otherwise special characters in a string.

A `Rule` element may also have a `key` attribute, but that's just for making your own custom meta-data:

```xml
<Import>System.Reflection</Import>
<Rule name="AssemblyVersionMetadata" key="Branch">Nacho Libre</Rule>
<Rule name="AssemblyVersionMetadata" key="Committer">Jack Black</Rule>
<Rule name="AssemblyVersionMetadata" key="Message">Change song to Encarnación<Rule>
```
```csharp
using System.Reflection;

[assembly: AssemblyVersionMetadata(@"Branch",@"Nacho Libre")]
[assembly: AssemblyVersionMetadata(@"Committer",@"Jack Black")]
[assembly: AssemblyVersionMetadata(@"Message",@"Change song to Encarnación")]
```

There is no such thing as `AssemblyVersionMetadata` in the .NET Framework, so RelaxVersioner will generate the necessary code in the source file.

## Key Values

On top of all of this, Rules can also have certain **key values** as part of their value.
For example, in  `<Rule name="AssemblyVersionMetadata">{committer}</Rule>`, the value `{committer}` will be replaced with the author of the last commit.
This kind of magic is the result of 
[Naming Formatter](https://github.com/kekyo/CenterCLR.NamingFormatter)
and
[libgit2sharp](https://github.com/libgit2/libgit2sharp).

Documentation on all of the different key values is currently a work in progress.