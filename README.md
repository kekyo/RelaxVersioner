# RelaxVersioner
![RelaxVersioner](https://raw.githubusercontent.com/kekyo/CenterCLR.RelaxVersioner/master/Images/CenterCLR.RelaxVersioner.128.png)
* ![Japanese language](https://raw.githubusercontent.com/kekyo/CenterCLR.RelaxVersioner/master/Images/Japanese.256.png) https://github.com/kekyo/CenterCLR.RelaxVersioner/blob/master/README.ja.md

## What is this?
* RelaxVersioner is Very easy-usage, Git-based, auto-generate version informations in .NET source code. (Assembly attribute based)
* Target language/projects: C#, F#, VB.NET, and C++/CLI
* Auto collect version information from local Git repository tags/branch name.
* Place source code location which isn't obstructive for Git. (ex: obj/Debug)

## License
* Copyright (c) 2015 Kouji Matsui (@kekyo2)
* Under Apache v2

## How to use
* Search NuGet repository: "CenterCLR.RelaxVersioner", and install. https://www.nuget.org/packages/CenterCLR.RelaxVersioner/
* After installed, build project. Auto-apply version informations into assembly attributes. Some attributes are looking for directory Windows Explorer property page.

## TODO:
* Known problem : Not valid Git tag search algorithm.
* Custom attribute/format ruleset.

## History
* 0.5.30.0: Checked on 4 languages.
* 0.5.0.0: Initial public commit. (Broken, still under construction)
