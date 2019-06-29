# EOE_DebugMenuMod
![alt text](https://github.com/Tanner555/EOE_DebugMenuMod/blob/master/DebugMenuPics/Debug-Compressed/DebugMenuWithActivateTeleportMenuBtn.PNG)

## Important Update - This Mod Will No Longer Work
Yes, this mod will no longer work with an updated version of Edge of Eternity. That's because the developers made the decision to upgrade their Unity build from Mono to IL2CPP. EOE traditionally depended on c# reflection techniques to load mod scripts into the game's c# assembly. This functionality really only works will with Mono, because of its flexible and traditional JIT compiler methods. IL2CPP is the process of converting the game's c# intermediate language into high performance c++ code. This allow's Edge of Eternity to bypass the limitations of c# (mostly), and generally allows the game code to run faster with less stuttering (performance improvements are CPU bound). Considering the EOE development team's priority to ship a finished game over improving their modding tools, this will be an official sunset for the very first Scripting mod to join the steam workshop. I know many of you saw the mod tools in the official announcement months before release and got excited. I sure did. But it was a decent effort to say the least. 

Because of this complication, I have removed this mod from the Steam Workshop and will no longer be supporting it.

## Description

A Simple UI That Adds Debugging Functionality Into Edge of Eternity. It's still in early progress, I plan to add more features in the future. 

To Toggle the Debug Menu, Press the 'K' Key. 

## Installation Guide (This will no longer work)
Installing the Mod Is Quite Easy. 
* You'll Need To Visit The Releases Page Here: [EOE Debug Menu Releases](https://github.com/Tanner555/EOE_DebugMenuMod/releases).
* Download the Latest Version, titled 'eoe_debug_menu_mod', and contains the zip. 
* Unzip the downloaded file into the SAME FOLDER. For instance, the zip could say Extract To "eoe_debug_menu_mod/". 
That way the folder structure will remain the same.
* Go into your Edge of Eternity Root Game Folder. You can do this by right clicking on EOE on steam, click properties, 
Go Into Local Files, and then click Browse Local Files...
* In the EOE root game file, navigate to "Edge Of Eternity/EdgeOfEternity_Data/StreamingAssets/Mods".
* Move the Folder you unzipped (It should have the same name as the zipped file) into the Mods Folder.
* You should now see the mod in the Mod Menu when you launch the game.
