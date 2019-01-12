## Customizing RelaxVersioner

The assembly attributes that **Relax Versioner** generates are controlled via a `RelaxVersioner.rules` file.
The program will first look for it in your project directory. If it can't find it there, then it will look in your solution directory. If it's not there either then it will load `DefaultRuleSet.rules`, which is embedded into its assembly. 

The custom rules file is an XML file with `RelaxVersioner` as its root element, which must contain at least one `WriterRules` element.
These `WriterRules` elements determine what attributes RelaxVersioner will generate.

```xml
<?xml version="1.0" encoding="utf-8"?>
<RelaxVersioner version="1.0">
   <WriterRules>
      .
      .
      .
   </WriterRules>
   <WriterRules>
      .
      .
      .
   </WriterRules>
   .
   .
   .
</RelaxVersioner>
```

### The Make-up of WriterRules

The `WriterRules` element may have three kinds of child elements:

1. `Language` elements, which must be one of the following:
   * C#
   * F#
   * VB
   * C++/CLI
   * Wix
2. `Import` elements, which import a namespace given its value.
   
3. `Rule` elements, which define an assembly attribute to be written down.

Rule elements are explained more in detail in the [next page](More-On-Rules.md).

Although only one language is selected per build, a `WriterRules` element can have multiple languages in it, and each one will have the same rules and imports.
A good example of this is the default rule set:

```xml
<RelaxVersioner version="1.0">
    <WriterRules>
        <!-- Target languages -->
        <Language>C#</Language>
        <Language>F#</Language>
        <Language>VB</Language>
        <Language>C++/CLI</Language>
```

It doesn't matter wheather your building a **C#** project or a **C++/CLI** project. RelaxVersioner will generate the same assembly information from the same element in the same file.

### Extra Options

The behavior of RelaxVersioner can be further altered by editing your msbuild file. For instance, the default path of the generated source file is:

```xml
<RelaxVersionerOutputPath Condition="'$(RelaxVersionerOutputPath)' == 
''">$([System.IO.Path]::Combine('$(ProjectDir)','$(IntermediateOutputPath)',
'RelaxVersioner$(DefaultLanguageSourceExtension)'))</RelaxVersionerOutputPath>
```

You can define your own **RelaxVersionerOutputPath** in your project file if you wish, and RelaxVersioner will use that instead :)