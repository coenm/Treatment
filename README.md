<h1 align="center">Treatment</h1>
<div align="center">
  
[![Build status](https://ci.appveyor.com/api/projects/status/41u98m32ih1758kq/branch/develop?svg=true)](https://ci.appveyor.com/project/coenm/treatment/branch/develop) 

</div>

Specific command line tool for updating `*.csproj` files.

## What does it do?

Replace lines like:
```
<HintPath>..\..\..\..\..\..\Packages\A.BC.1.0.149\lib\net45\A.BC.dll</HintPath>
```

to 
```
<HintPath>$(PackagesDir)\A.BC.1.0.149\lib\net45\A.BC.dll</HintPath>
```
in `*.csproj` files within the given root directory (see `--directory` argument.)

## How to ask?

```
Treatment.exe help

  fix-app-config    Remove new app.config files and fix csproj file

  list-providers    List installed search, and version control providers to be used

  fix               Fix csproj files where hintpath of packages is fixed

  help              Display more information on a specific command.

  version           Display version information.


Treatment.exe help fix-app-config

  -h, --hold                   (Default: false) Keeps console open when process successfully finished.

  -d, --directory              Root directory to process the csproj files.

  -n, --dry-run                (Default: false) File changes are not written to disk, only listed in the console.

  -v, --verbose                (Default: 0) Verbosity level ranging from 0 (disabled) to 3 (max).

  -p, --search-provider        (Default: FileSystem) Set search provider to search for csproj files. To list the search
                               providers, use the 'list-providers' command.

  --versioncontrol-provider    Set version control provider

  -s, --summary                (Default: false) Prints a summary at the end of fixing the csproj files.

  --help                       Display this help screen.

  --version                    Display version information.


Treatment.exe help fix

  -d, --directory          Root directory to process the csproj files.

  -n, --dry-run            (Default: false) File changes are not written to disk, only listed in the console.

  -s, --summary            (Default: false) Prints a summary at the end of fixing the csproj files.

  -p, --search-provider    (Default: FileSystem) Set search provider to search for csproj files. To list the search providers, use the 'list-providers' command.

  -v, --verbose            (Default: 0) Verbosity level ranging from 0 (disabled) to 3 (max).

  -h, --hold               (Default: false) Keeps console open when process successfully finished.

  --help                   Display this help screen.

  --version                Display version information.


Treatment.exe help list-providers

  -h, --hold    (Default: false) Keeps console open when process successfully finished.

  --help        Display this help screen.

  --version     Display version information.
```

## What is 'Everything'?

Found at voidtools.com [FAQ](https://www.voidtools.com/faq/#what_is_everything)

> "Everything" is search engine that locates files and folders by filename instantly for Windows.
>
> Unlike Windows search "Everything" initially displays every file and folder on your computer (hence the name "Everything").
>
> You type in a search filter to limit what files and folders are displayed.

Requires Everything to be installed, but when used as search provider it increases performance.

## Why 'so much code' for such a simple application?

Yes. The application has little functionality which probably can be coded in a single class with a few methods that is still pretty clean. This project is also about experimenting with common patterns, principles, frameworks etc. etc.

## Decisions

- Based on the commandline arguments, a command is constructed.
- Dependency injection (only constructor injection) using [SimpleInjector](https://www.nuget.org/packages/SimpleInjector/) library. This framework also supports a plugin to register it's implementations.
- Use commands and command handlers for the two commands the application supports. Command handlers can be decorated to enable crosscuttingconcerns (i.e. Execution time measurement, Logging and (superficial) Command validation).
These (superficial) validations are done using the [FluentValidation](https://www.nuget.org/packages/FluentValidation/) library.
- Suffix methods returning Tasks with `Async`.
- Test projects are named with suffix `.Tests`
- No abstractions for logging.

## TODO

- [x] [GitFlow](http://nvie.com/posts/a-successful-git-branching-model/);
- [x] [Appveyor](https://www.appveyor.com/);
- [x] [GitVersion](https://gitversion.readthedocs.io/en/latest/) for automatic versioning; 
- [ ] Input directory validation improvement;
- [x] Core without Console.WriteLine;
- [x] Console.WriteLine abstraction?;
- [x] Experiment with [Pose](https://www.nuget.org/packages/Pose) to shim `Console`.
- [ ] Improve functionality by other fixes in the csproj files (app.config settings).
- [x] Experiment with [ApprovalTests](https://github.com/approvals/ApprovalTests.Net) to test WPF views.
- [ ] TestAutomation
- [ ] CodeCoverage
