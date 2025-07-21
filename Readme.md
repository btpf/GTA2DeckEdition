# GTA 2 - Deck Edition

Written 7/7/25

This mod pack of GTA 2 is contains quality of life updates and crash fixes that make this game enjoyable on both the Steam Deck and Windows 11 out of the box. Additionally, some of the best community maps made have been translated, included, and showcased in the map manager.



## What's Included

- Custom Made Map/Mod Manager
- Curated community maps (with some mission supported ones showcased)
  - I threw the german and russian maps into Gemini 2.5 flash for english translation so they no longer crash the game and are playable. Translation correction PRs are welcome since it wasn't a perfect operation.
- GTA 2 Widescreen fix, Quicksave function
- Front End Fix - This fixes the main menu glitch (Showing in green), and adds a psx styled menu, a seriously good quality of life update. This mod depends on the widescreen fix and supports GTA 9.6 only.
  - Compiled with two unmerged PRs
    - https://github.com/gennariarmando/gta2-frontend-fix/pull/8
      https://github.com/gennariarmando/gta2-frontend-fix/pull/9
- GTA 2 Radar - This adds a gta 3 styled minimap
  - Compiled with latest update: https://github.com/gennariarmando/gta2-radar/pull/3
  - Note: This mod can be toggled from the mod manager
- GInput2 - controller support
- GTA 2 Power Manager + Launcher Batch file
  - This mod loader dll (dinput.dll) version breaks the gta 2 manager, so i created a script to launch the manager by temporarily renaming dinput.dll to something else. 
- All Stages & Bonus Stages unlocked in first save slot


---

## Installation

Simply extract the release into the root directory of your GTA 9.6 folder and launch `GTA 2 Launcher.exe`

### Steam Deck Install

Easiest way: Add the `GTA 2 Launcher.exe` as a non steam game and launch with the the latest prefix
Better way: Add the `GTA 2 Launcher.exe` to a wine prefix using bottles or other utility, and then add to steam.

See "Wine/Proton Settings" Section for necessary DLL overrides

## Steam Deck Notes

In order to get this game running on the Steam Deck, I have made the following change:

### Resolution & Window Settings

Under `\ufiles\settings.xml`

```
	<display>
		<ScreenWidth value="1280" />
		<ScreenHeight value="800" />
		<ScreenType value="borderless windowed" />
	</display>
```

Its important to match the resolution to the device and make the screenType borderless windowed otherwise the game will crash on launch.

### Wine/Proton Settings

Open Steam Library, go to Properties of Grand Theft Auto 2 and in "Compatibility" tab, in **COMMAND LINE ARGUMENTS** add following line: `WINEDLLOVERRIDES="dinput=n,b" %command%`

I highly recommend using proton through an application like bottles, and making the same override changes. This is because (As i understand it), you can then launch the mod loader, standalone game, and power manager under the same prefix with the same save and settings. If using bottles, just add the string `dinput` to the overrides section. 



## Adding Maps To Mod Manager

In order to add maps, first locate `/MapMods` in the root directory.

- create a mod folder with the name of the mod
- Inside of the mod folder, create a readme.txt, and include a thumbnail named `thumb.png` or `thumb.jpg` etc...

- Inside of the mod folder, create a `data` folder. This folder will override the data folder in the root of the installation, so place the map files accordingly here.



## Developer Notes

This is what I used to create this mod pack

Install Order:
1. (SKIP INSTALLING THIS) Ultimate-ASI-Loader = https://github.com/ThirteenAG/Ultimate-ASI-Loader
    1b. Legacy dinput asiloader (Skip this) = https://gtaforums.com/topic/989464-frontendfix-for-gta2/page/2/#findComment-1072449906
    1c. It seems i found an official version that works here: https://github.com/ThirteenAG/Ultimate-ASI-Loader/releases/tag/v4.68
2. (SKIP DLL FILE) Widescreen-Fix = https://github.com/ThirteenAG/WidescreenFixesPack/releases/tag/gta2
3. FontendFix = https://github.com/gennariarmando/gta2-frontend-fix
4. gta2 Radar = https://github.com/gennariarmando/gta2-radar/releases/tag/v0.100
5. GInput for GTA2 = https://gtaforums.com/topic/988318-ginput-for-gta2/
6. Power Manager = https://gamebanana.com/tools/6106
7. Start Manager.bat = My own tool, made to get around the dll issues
8. GTA 2 Launcher.ahk = Compiled to a 32 bit exe to make the folder structure a little cleaner



The mod manager was created intentionally with .NET Framework 4.8 since this version is most compatible with wine/proton. 

Once compiled, the mod manager must go into a "Mod Manager" folder
