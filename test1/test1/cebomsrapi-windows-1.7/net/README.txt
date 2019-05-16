General:
========

The .NET API is build for "Any CPU". It internally selects the correct
native layer at runtime.

There is a pre-build version in sub-directory "prebuilt".

Windows:
========

Visual Studio / C#:
--------------------
Tested with: Microsoft Visual Studio 2013 and 2015

Open solution specific to your Visual Studio version 
(build-vs2010/cebomsrnet.sln, build-vs2012/cebomsrnet.sln, build-vs2012/cebomsrnet.sln
or build-vs2015/cebomsrnet.sln) and build the project. Debug and
Release builds can be selected.

The .NET API is part of this project.

Linux/Mac OS X:
===============

Monodevelop / C#
----------------

Open the solution file build-vs2010/cebomsrnet.sln from Monodevelop.

Important: Please edit SystemConnector.cs and change the follwoing lines:

        private const string DLLx86 = "cebomsr-1.7-x86.dll";        
        private const string DLLx64 = "cebomsr-1.7-x86_64.dll";
		
into this:

        private const string DLLx86 = "cebomsr-1.7";        
        private const string DLLx64 = "cebomsr-1.7";		
		
Otherwise, this does not run on Linux. Rebuilt cebomsrnet-1.7.dll and don't
use the prebuilt version.
