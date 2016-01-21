# RelaxVersioner
![RelaxVersioner](https://raw.githubusercontent.com/kekyo/CenterCLR.RelaxVersioner/master/Images/CenterCLR.RelaxVersioner.128.png)
* English language https://github.com/kekyo/CenterCLR.RelaxVersioner

## これは何？
* RelaxVersionerは、軽量で非常に簡単に使用することが出来る、Gitベースの「自動バージョニング」ツールセットです。.NETベースのソースコードを対象としていて、ビルド時にアセンブリ属性を自動的に適用します。
* サポートしている言語は、.NETで標準的に使用される、C#・F#・VB.NET・C++/CLIです。
* ローカルのGitリポジトリから、自動的にタグ・ブランチの名称を取得し、アセンブリ属性に適用することが出来ます。
* Visual Studio/MSBuildの中間出力フォルダーを自動的に使用するため、Gitリポジトリ内を汚すことがありません。

## 出力されるコードの例:

### For C#:
``` csharp
[assembly: System.Reflection.AssemblyVersionAttribute("0.5.30")]
[assembly: System.Reflection.AssemblyFileVersionAttribute("2016.1.15.41306")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Branch","master")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Message","Fixed tab")]
```

### For F#:
``` fsharp
namespace global
    [<assembly: System.Reflection.AssemblyVersionAttribute("0.5.30")>]
    [<assembly: System.Reflection.AssemblyFileVersionAttribute("2016.1.15.41306")>]
    [<assembly: System.Reflection.AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>]
    [<assembly: System.Reflection.AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")>]
    [<assembly: System.Reflection.AssemblyMetadataAttribute("Branch","master")>]
    [<assembly: System.Reflection.AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")>]
    [<assembly: System.Reflection.AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")>]
    [<assembly: System.Reflection.AssemblyMetadataAttribute("Message","Fixed tab")>]
    do()
```

### For VB.NET:
``` visualbasic
Imports System.Reflection
<Assembly: System.Reflection.AssemblyVersionAttribute("0.5.30")>
<Assembly: System.Reflection.AssemblyFileVersionAttribute("2016.1.15.41306")>
<Assembly: System.Reflection.AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Branch","master")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Message","Fixed tab")>
```

### For C++/CLI:
``` cpp
[assembly: System::Reflection::AssemblyVersionAttribute("0.5.30")];
[assembly: System::Reflection::AssemblyFileVersionAttribute("2016.1.15.41306")];
[assembly: System::Reflection::AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Branch","master")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Message","Fixed tab")];

```

## 使い方
* NuGetで"CenterCLR.RelaxVersioner"を検索して導入してください。 https://www.nuget.org/packages/CenterCLR.RelaxVersioner/
* あらかじめ、AssemblyInfo.cs等に定義されている、デフォルトの"AssemblyVersion"と"AssemblyFileVersion"属性をコメントアウトして下さい（ビルド時に重複定義エラーが発生します）。これらはカスタムルールを用いて定義を除外するのであれば、引き続き使用する事もできます。
* ビルドすると、自動的にアセンブリ属性が適用されます。ILSpy等で確認するか、一部の属性はエクスプローラーのプロパティから確認することが出来ます。
* プロジェクトフォルダ、又はソリューションフォルダに"RelaxVersioner.rules"ファイルを配置することで、カスタムルールを定義出来ます。まだドキュメントを用意できていないので、サンプルとして以下の定義例を参照してください。

## カスタムルールファイルの例:
``` xml
<?xml version="1.0" encoding="utf-8"?>
<RelaxVersioner version="1.0">
	<Rules>
		<!-- Target languages -->
		<Language>C#</Language>
		<Language>F#</Language>
		<Language>VB</Language>
		<Language>C++/CLI</Language>
		<!--
			"gitLabel" is extract numerical-notate version string [1.2.3.4] from git repository tags/branches traverse start HEAD.
			If not found, fallback to "safeVersion".
		-->
		<Rule name="System.Reflection.AssemblyVersionAttribute">{gitLabel}</Rule>
		<!--
			"safeVersion" is extract committed date (with commmiter) from git repository HEAD.
			"safeVersion" specialized from "committer.When".
			(The format is safe-numerical-notate version string [2016.2.14.12345]. (Last number is 2sec prec.))
		-->
		<Rule name="System.Reflection.AssemblyFileVersionAttribute">{safeVersion}</Rule>
		<!--
			"commitId" is extract commit id from git repository HEAD.
			"commitId" alias to "commit.Sha".
		-->
		<Rule name="System.Reflection.AssemblyInformationalVersionAttribute">{commitId}</Rule>
		<!--
			"key" is only used "AssemblyMetadataAttribute".
			If you use "AssemblyMetadataAttribute" and platform version == "v4.0",
			cannot use mscorlib implementation.
			Will auto define pseudo "AssemblyMetadataAttribute" class.
		-->
		<!--
			"committer.When" or you can use another choice "author.When".
			"author" and "committer" can use property "Name", "Email", and "When".
		-->
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Build">{committer.When:R}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Branch">{branch.Name}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Author">{author}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Committer">{committer}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Message">{commit.MessageShort}</Rule>
	</Rules>
</RelaxVersioner>
```

## TODO:
* 既知の問題 : Gitタグの検索方法に誤りがある。
* 除外ルールのサポート。
* ネイティブC++プロジェクトに対応させる。

## License
* Copyright (c) 2015 Kouji Matsui (@kekyo2)
* Under Apache v2

## 履歴
* 0.7.1: カスタムルールセットファイルに対応
* 0.5.30: 4言語で動作確認
* 0.5.0: Initial public commit. (Broken, still under construction)
