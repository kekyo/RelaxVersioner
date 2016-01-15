# RelaxVersioner
![RelaxVersioner](https://raw.githubusercontent.com/kekyo/CenterCLR.RelaxVersioner/master/Images/CenterCLR.RelaxVersioner.128.png)
* English language https://github.com/kekyo/CenterCLR.RelaxVersioner

## これは何？
* RelaxVersionerは、軽量で非常に簡単に使用することが出来る、Gitベースの「自動バージョニング」ツールセットです。.NETベースのソースコードを対象としていて、ビルド時にアセンブリ属性を自動的に適用します。
* サポートしている言語は、.NETで標準的に使用される、C#・F#・VB.NET・C++/CLIです。
* ローカルのGitリポジトリから、自動的にタグ・ブランチの名称を取得し、アセンブリ属性に適用することが出来ます。
* Visual Studio/MSBuildの中間出力フォルダーを自動的に使用するため、Gitリポジトリ内を汚すことがありません。

## License
* Copyright (c) 2015 Kouji Matsui (@kekyo2)
* Under Apache v2

## 使い方
* NuGetで"CenterCLR.RelaxVersioner"を検索して導入してください。 https://www.nuget.org/packages/CenterCLR.RelaxVersioner/
* ビルドすると、自動的にアセンブリ属性が適用されます。ILSpy等で確認するか、一部の属性はエクスプローラーのプロパティから確認することが出来ます。

## TODO:
* 既知の問題 : Gitタグの検索方法に誤りがある。
* アセンブリ属性の適用方法などをカスタマイズ可能にする。
* ネイティブC++プロジェクトに対応させる。

## 履歴
* 0.5.30.0: 4言語で動作確認
* 0.5.0.0: Initial public commit. (Broken, still under construction)
