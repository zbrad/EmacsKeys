# EmacsKeys
This project provides Emacs key bindings for Visual Studio users, through a Visual Studio Extension.

## History ##

This capability was available in [VS2008](https://msdn.microsoft.com/en-us/library/ms165528(VS.90).aspx), but was removed in VS2010 and VS2012.  

Emulation was released as a Visual Studio Extension called the "[Emacs Emulation](https://visualstudiogallery.msdn.microsoft.com/09dc58c4-6f47-413a-9176-742be7463f92 "Emacs Emulation")" extension.

Since the extension has not been supported by the Visual Studio Team since 2010, I was able to make the case for the source code to be made available publicly. 

This is a copy of the original VS2010 extension source code as provided by the Visual Studio Team, and is the initial check-in of this repository.

## Current Plans ##

A current limitation of the original version, is that it requires the copying of a "vsk" file, into the Visual Studio product folder, which requires running the extension installer as administrator.

To solve this, my next check-in will instead make use of the Visual Studio import/export bindings (vssettings), stored per user, with a specified set of Emacs key bindings, that can then be edited locally according to taste.




