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
Treatment.exe --help

  -n, --dry-run            (Default: false) Dry run

  -v, --verbose            (Default: false) Prints all messages to standard output.

  -s, --summary            (Default: true) Summary

  -h, --hold               (Default: false) Keeps console open when process successfully finished.

  -p, --search-provider    (Default: FileSystem) Set search provider to search for csproj files. Options: FileSystem, Everything

  -d, --directory          Required. Root directory to process the csproj files.

  --help                   Display this help screen.

  --version                Display version information.
```


## What is 'Everything'?
Found at voidtools.com [FAQ](https://www.voidtools.com/faq/#what_is_everything) 

> "Everything" is search engine that locates files and folders by filename instantly for Windows.
> 
> Unlike Windows search "Everything" initially displays every file and folder on your computer (hence the name "Everything").
> 
> You type in a search filter to limit what files and folders are displayed.

Requires Everything to be installed, but when used as search provider it increases performance.