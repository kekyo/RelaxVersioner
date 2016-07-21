# RelaxVersioner
![RelaxVersioner](https://raw.githubusercontent.com/kekyo/CenterCLR.RelaxVersioner/master/Images/CenterCLR.RelaxVersioner.128.png)
* English language https://github.com/kekyo/CenterCLR.RelaxVersioner

# Status
* NuGet Package: [![NuGet RelaxVersioner](https://img.shields.io/nuget/v/CenterCLR.RelaxVersioner.svg?style=flat)](https://www.nuget.org/packages/CenterCLR.RelaxVersioner)
* Continuous integration: [![AppVeyor RelaxVersioner](https://img.shields.io/appveyor/ci/kekyo/centerclr-relaxversioner.svg?style=flat)](https://ci.appveyor.com/project/kekyo/centerclr-relaxversioner)
* Gitter: [![Gitter RelaxVersioner](https://img.shields.io/gitter/room/kekyo/CenterCLR.RelaxVersioner.svg?style=flat)](https://gitter.im/kekyo/CenterCLR.RelaxVersioner)

## これは何？
* RelaxVersionerは、軽量で非常に簡単に使用することが出来る、Gitベースの「自動バージョニング」ツールセットです。.NETベースのソースコードを対象としていて、ビルド時にアセンブリ属性を自動的に適用します。
* RelaxVersionerを使うと、Gitのタグ・ブランチ・コミットメッセージだけを使って、バージョン管理が出来ます。つまり、追加のツール操作が不要なため、Gitを知ってさえいれば学習コストがほとんどなく、CI環境にも容易に対応できます。
* サポートしている言語は、.NETで標準的に使用される、C#・F#・VB.NET・C++/CLI、そしてWix（wixprojとwxs）です。
  * Wixは現在作業中です。
* ローカルのGitリポジトリから、自動的にタグ・ブランチの名称を取得し、アセンブリ属性に適用することが出来ます。
* AssemblyInfo.csファイルを直接変更しません。RelaxVersionerはテンポラリファイルに定義を出力し、それをコンパイルさせます。
* Visual Studio/MSBuildの中間出力フォルダーを自動的に使用するため、Gitリポジトリ内を汚すことがありません。
* カスタムルールセットを定義することで、出力する属性と内容をカスタマイズすることが出来ます。

## 出力されるコードの例:

### For C#:

``` csharp
[assembly: System.Reflection.AssemblyVersionAttribute("0.5.30.0")]
[assembly: System.Reflection.AssemblyFileVersionAttribute("2016.1.15.41306")]
[assembly: System.Reflection.AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Branch","master")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Tags","0.5.30")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")]
[assembly: System.Reflection.AssemblyMetadataAttribute("Message","Fixed tab")]
```

### For F#:

``` fsharp
namespace global
	[<assembly: System.Reflection.AssemblyVersionAttribute("0.5.30.0")>]
	[<assembly: System.Reflection.AssemblyFileVersionAttribute("2016.1.15.41306")>]
	[<assembly: System.Reflection.AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Branch","master")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Tags","0.5.30")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")>]
	[<assembly: System.Reflection.AssemblyMetadataAttribute("Message","Fixed tab")>]
	do()
```

### For VB.NET:

``` visualbasic
Imports System.Reflection
<Assembly: System.Reflection.AssemblyVersionAttribute("0.5.30.0")>
<Assembly: System.Reflection.AssemblyFileVersionAttribute("2016.1.15.41306")>
<Assembly: System.Reflection.AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Branch","master")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Tags","0.5.30")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")>
<Assembly: System.Reflection.AssemblyMetadataAttribute("Message","Fixed tab")>
```

### For C++/CLI:

``` cpp
[assembly: System::Reflection::AssemblyVersionAttribute("0.5.30.0")];
[assembly: System::Reflection::AssemblyFileVersionAttribute("2016.1.15.41306")];
[assembly: System::Reflection::AssemblyInformationalVersionAttribute("a05ab9fc87b22234596f4ddd43136e9e526ebb90")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Build","Fri, 15 Jan 2016 13:56:53 GMT")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Branch","master")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Tags","0.5.30")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Author","Kouji Matsui <k@kekyo.net>")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Committer","Kouji Matsui <k@kekyo.net>")];
[assembly: System::Reflection::AssemblyMetadataAttribute("Message","Fixed tab")];
```

## 使い方
* NuGetで"RelaxVersioner"を検索して導入してください。 https://www.nuget.org/packages/CenterCLR.RelaxVersioner/
* あらかじめ、AssemblyInfo.cs等に定義されている、デフォルトの"AssemblyVersion"と"AssemblyFileVersion"属性をコメントアウトして下さい（ビルド時に重複定義エラーが発生します）。
  * これらはカスタムルールを用いて定義を除外するのであれば、引き続き使用する事もできます。
* ビルドすると、自動的にアセンブリ属性が適用されます。ILSpy等で確認するか、一部の属性はエクスプローラーのプロパティから確認することが出来ます。
* プロジェクトフォルダ、又はソリューションフォルダに"RelaxVersioner.rules"ファイルを配置することで、カスタムルールを定義出来ます。まだドキュメントを用意できていないので、サンプルとして以下の定義例を参照してください。

## 開発運用の例
1. C#・F#などでプロジェクトを新規に作ります。
2. NuGetで"RelaxVersioner"を検索して、プロジェクトに追加します。
3. AssemblyInfo.csなどに定義されている、デフォルトの"AssemblyVersion"と"AssemblyFileVersion"属性をコメントアウトします。
4. この状態でビルドするだけで、バージョンが適用されたバイナリが生成されます。
  * デフォルトでは、AssemblyVersionが"0.0.1.0"、AssemblyFileVersionがビルド時の日時を2秒精度でバージョン化した値（例:"2016.05.12.11523"）となります。
	* また、AssemblyMetadataに、ローカルGitリポジトリから得られる情報が埋め込まれます（Author・ブランチ・タグなど）。しかし、この例ではまだgit initしてないので"Unknown"として埋め込まれます。
5. ソリューションフォルダでgit initして適当にコミットしてください。
6. この状態でビルドすると、Author・ブランチやコミットメッセージが埋め込まれます。
7. 現在のコミットにタグをつけてください。例えば"0.5.4"のようなバージョン表記です。これでビルドすれば、このバージョン番号が自動的にAssemblyVersionに反映されるようになります。
8. 全て良ければ、リモートリポジトリにpushして完了です。
9. 以後、コードを変更してリリースの準備が出来たら、新たにタグをつければそれがAssemblyVersionに反映されるので、ビルドしてバイナリをリリースします。

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
			"author" and "committer" can use property "Name", "Email", and "When". (Derived from libgit2sharp)
		-->
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Build">{committer.When:R}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Branch">{branch.Name}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Tags">{tags}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Author">{author}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Committer">{committer}</Rule>
		<Rule name="System.Reflection.AssemblyMetadataAttribute" key="Message">{commit.MessageShort}</Rule>
	</Rules>
</RelaxVersioner>
```

## TODO:
* 除外ルールのサポート
* ネイティブC++プロジェクトに対応させる
* NuGetを出力するプロジェクトに対応させる (nuproj?, nubuild? and/or other projects?)
* テンプレート出力をサポートする
* フォールバックルールを指定可能にする
* Mono環境のサポート (そして *nix 環境でのCIのサポート / 詳しい方のPRウェルカム)

## License
* Copyright (c) 2015 Kouji Matsui (@kekyo2)
* Under Apache v2

## 履歴
* 0.8.3:
  * Wixプロジェクトのサポート
* 0.8.2:
  * Mono環境のサポート (未完了・詳しい方のPRウェルカム)
* 0.8.1:
  * ルールセットの "Rules" を "WriterRules" に変更 (Breaking change)
  * "gitLabel" のフォールバック値を "safeVersion" から、固定の "0.0.0.0" に変更
  * ビルド時に、適用されたバージョンを出力するようにした
  * ソースコード上のタブを修正
* 0.7.18:
  * PCLプロジェクトで、AssemblyMetadataAttributeが見つからない問題を修正
* 0.7.17:
  * TargetPathに対応するフォルダが存在しない環境（CI等）で例外が発生していた問題を修正
* 0.7.16:
  * libgit2sharpとNamingFormatterを更新
* 0.7.14:
  * SemVerで使用される、プレフィックス・ポストフィックスを除外してバージョン番号をパースするように変更
  * Gitリポジトリのトラバース方法を変更し、TagsについてはHEADコミットのみを参照するように変更
* 0.7.13:
  * コミットメッセージなどにソースコード非互換の文字列が含まれている場合にビルドが失敗するのを修正。例: Dirty\String"Test"
* 0.7.12:
  * まだコミットがないリポジトリを使用した場合にクラッシュする問題を修正
* 0.7.11:
  * {branches} と {tags} フォーマットを追加
* 0.7.10:
  * NamingFormatterを使うように変更
  * System.Version.Parseを使うように変更
* 0.7.7:
  * 最新のタグの検索漏れを修正
* 0.7.6:
  * 古い .NET Framework (2.0-3.5) に対応
* 0.7.5:
  * プロジェクトフォルダからルートフォルダに向かってgitリポジトリを検索するように変更
  * gitリポジトリが見つからないか、コミットが存在しないような場合でも、ダミーの属性を生成するようにしてエラーを回避した
* 0.7.1: カスタムルールセットファイルに対応
* 0.5.30: 4言語で動作確認
* 0.5.0: Initial public commit. (Broken, still under construction)
