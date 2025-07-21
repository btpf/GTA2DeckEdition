*********************************************************************
GTA2 GXT Editor v3.0.1 build 63, 08.08 - 14.08.2010
*********************************************************************

Copyright (c) 2010, B-$hep (A.V).
Portions Copyright (c) 2004 Delfi.
---------------------------------------------------------------------
* What's new in 3.0.1

	+ "GoTo" dialog now only allows to enter numbers (0-9) and also
		allows to use backspace key to fix any typos.
		You can type in the line nr and hit the "Enter" key which
		does the same as "OK" button.
	+ "GoTo" dialog now displays error message if line nr 
		is out of range.
	+ When "GoTo" dialog appears, then editbox automatically gets
		the focus.
	+ Added line numbers.
	+ Added "View" menu. Which allows now hide / show TOOLBAR
		and Line numbers.
	+ Added possibility to change editor font and background color
	+ Added "Russian" language to the languages list.
		If r.gxt is opened then editor charset is set to Russian
		automatically.
		Unfortunately Russian r.gxt decoding is not working (yet).
		It doesn't display Russian text correctly even with Russian
		charset and font.
		Some custom decoding of GXT must be done specially for
		Russian version. Im working on it.		
	+ Toolbar, line numbers, color info, window position and size
		is all saved in INI file.
	+ "Edit -> Delete" is now only enabled if there is something selected
	+ Before opening new GXT or if closing program, user will be asked
		to save any changes made in file.
		This was available also in v3.0.0 but i forgot to mention it.
	+ Fixed ReadMe.txt displaying by F1. It throwed "Access Violation".
	+ Fixed "List Index Out of bounds" error that occurred when editor
		tried to read from MRU list but the file didn't exist..


* What's new in 3.0.0
	+ Added tips to toolbar buttons
	+ Added recent files
	+ Added XP manifest
	+ Added possibility to Import & Export GXT to & from textfiles
	+ Added possibility to "Go To" specific line in editor
	+ Added popup menu for editor, makes it a bit easier to use
	+ Added this improved and updated ReadMe.txt file
	+ Added displaying of currently opened GXT file in app title
	+ Added new About box
	+ "Edit" menu is now more like in Notepad
	+ Fixed all known memory leaks, removed faulty VCL components
	+ Some minor UI improvements
	+ Executable is packed with UPX with max compression
	+ Added version info to executable
	+ Last used GXT file (if exists) is now opened by default when launching
	+ Improved the "Syntax Check" window a bit. Double clicking on error
		will point to the correct line now and you can scroll the
		errorlist with mousewheel now
	+ Improved the work of different "Edit" menu items.
	+ Closing the errorlist window with "Close" button will set
		focus back to editor.
	+ F1 displays this ReadMe file

* Features:
	- Easy editing of GTA2 GXT files
	- Easy Import & Export from / to textfiles
	- Commandline support (you can drop the GXT file to editor shortcut)
	- Displays currently opened filename in window title

* Requirements:
	- GTA2
	- Windows 9x, 2000, XP, Vista or Win7

* Thanks and greetings to:
	- Sektor for the motivation to improve this editor
	- elypter for the textfile Export / Import idea

* Other info:

	This program uses:
 
	SynEdit version 2.00, http://SynEdit.SourceForge.net
	Toolbar97 version 1.8 by Jordan Russel, http://www.jrsoftware.org

	Program developed using Borland Delphi 5 Enterprise Edition.


	Do you have any comments, ideas, suggestions, questions,
	BUG REPORTS related to this software?

	MailTo: 

		plainsecrets [at] gmail.com

	Visit my website for updates about this software and my other GTA2 projects:

		http://www.gta2madness.co.cc