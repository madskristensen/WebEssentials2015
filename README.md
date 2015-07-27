# [Web Essentials](http://vswebessentials.com) for Visual Studio 2015

[![Build status](https://ci.appveyor.com/api/projects/status/635d910flwbi7sh9?svg=true)](https://ci.appveyor.com/project/madskristensen/webessentials2015)

Web Essentials extends Visual Studio with lots of new features that web developers have been missing for many years. 

If you ever write CSS, HTML, JavaScript, CoffeeScript, LESS or Sass, then you will find many useful features that make your life as a developer easier. 

This is for all Web developers using Visual Studio.

To get the latest nightly build, follow [these instructions](http://vswebessentials.com/download#nightly).

### LESS/Sass/CoffeeScript compilation and bundling
The compilers have been removed from Web Essentials and a new extension
has been created specifically for compiling web resources. Download
[Web Compiler](https://visualstudiogallery.msdn.microsoft.com/3b329021-cd7a-4a01-86fc-714c2d05bb6c)

For bundling and minification, use
[Bundler &amp; Minifier](https://visualstudiogallery.msdn.microsoft.com/9ec27da7-e24b-4d56-8064-fd7e88ac1c40)

## Getting started
To contribute to this project, you'll need to do a few things first:

 1. Fork the project on GitHub
 1. Clone it to your computer
 1. Install the [Visual Studio 2015 SDK](http://www.visualstudio.com/en-us/downloads/visual-studio-2015-downloads-vs#d-vs-sdk).
 1. Open the solution in VS2015.

To install your local fork into your main VS instance, you will first need to open `Source.extension.vsixmanifest` and bump the version number to make it overwrite the (presumably) already-installed production copy. (alternatively, just uninstall Web Essentials from within VS first)

You can then build the project, then double-click the VSIX file from the bin folder to install it in Visual Studio.


## Useful Links
 - [Getting started with Visual Studio extension development](http://blog.slaks.net/2013-10-18/extending-visual-studio-part-1-getting-started/)
 - [About Web Essentials features](http://blogs.msdn.com/b/mvpawardprogram/archive/2013/11/05/making-web-development-wonderful-again-with-web-essentials.aspx)
 - [Inside the Visual Studio editor](http://msdn.microsoft.com/en-us/library/vstudio/dd885240.aspx)
 - [Extending the Visual Studio editor](http://msdn.microsoft.com/en-us/library/vstudio/dd885244.aspx)
