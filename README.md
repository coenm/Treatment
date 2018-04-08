# Treatment

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

  list-providers    List installed search providers to be used when fixing csproject files.

  fix               Fix csproj files where hintpath of packages is fixed

  help              Display more information on a specific command.

  version           Display version information.


Treatment.exe help fix

  -d, --directory          Root directory to process the csproj files.

  -n, --dry-run            (Default: false) File changes are not written to disk, only listed in the console.

  -s, --summary            (Default: false) Prints a summary at the end of fixing the csproj files.

  -p, --search-provider    (Default: FileSystem) Set search provider to search for csproj files. To list the search providers, use the verb 'list-providers'.

  -v, --verbose            (Default: false) Prints more information about the current process to the console.

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

- [Appveyor](https://www.appveyor.com/);
- [GitVersion](https://gitversion.readthedocs.io/en/latest/);
- Input directory validation improvement;
- Core without Console.WriteLine (almost done)
- ~~Console.WriteLine abstraction?;~~
- Improve tests;
- Improve performance by processing multiple csproj files in parallel;
- Improve functionality by other fixes in the csproj files (app.config settings).
- Replace `ListSearchProvidersCommand` command with a query and let the `Treatment.Console` project decide how to display the result.

## Failed experiments

- Tried [Pose](https://www.nuget.org/packages/Pose) to shim `Console` for testing without success. Mabye usable for testing the `ConsoleAdapter`
