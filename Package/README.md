# ShipColors by darmuh
***
### Style your ship with your own personal colors!

- Mode "Generate Config" dynamically generates config items for materials throughout the ship for further customization.
	- Due to the dynamic nature of this mode, you may need to load the lobby multiple times to get some config items to show.
	- Also, working from an existing save file may detect scrap and other items within the ship. These can be filtered out in the setup config.
- Generate a Webpage to easily modify your config and update your settings.
	- See [here for example](https://darmuh.github.io/OpenLib/OpenLib/Website/Examples/ShipColors_Generated.cfg_generator.htm)
- Buttons added via LethalConfig!
	- Regenerate your config at any time
	- Generate a web page from your config at any time
	- Generated config items are added to the LethalConfig menu immediately
- Compatibility with darmuhsTerminalStuff will not generate Terminal customization config items when TerminalStuff customization is active.
- Any time you change a setting the customizations will **instantly** reload. 

**WARNING:** With the "Use Shared Textures" mode, this mod can change colors of materials not just on the ship. 
 - If you see odd colors on scrap or in the facility it's likely using the same shared texture.

### Compatibility
- This mod has built-in compatibility for [ShipWindows](https://thunderstore.io/c/lethal-company/p/TestAccount666/ShipWindows/), [LethalConfig](https://thunderstore.io/c/lethal-company/p/AinaVT/LethalConfig/), and [darmuhsTerminalStuff](https://thunderstore.io/c/lethal-company/p/darmuh/darmuhsTerminalStuff/)
- For mods like [OpenBodyCams](https://thunderstore.io/c/lethal-company/p/Zaggy1024/OpenBodyCams/) and [GeneralImprovements](https://thunderstore.io/c/lethal-company/p/ShaosilGaming/GeneralImprovements/), the default ban configuration items have added their respective screen objects/material names to avoid issues. [v0.2.3+]
- For all other mods, if you encounter issues please consider adding game objects associated with their mod to the GenBannedObjects list.

### API
- As of 0.3.0 there is now an API class for external mods to utilize.
- The API class has the following public methods:
	- InitCustomization: Calls the internal StartCustomizer method which is normally called at Terminal Start
		- If called after Terminal Start it will not create new configuration items and only read for changes from existing config items.
	- RegenConfigItems: Calls the RegenerateConfig method (the one used for the button in LethalConfig)
		- This will restart config generation and search the entire ship for objects to create config items for.
	- BanObject: Will ban the game object provided from generating color configuration items (will also prevent color changes)
	- UnbanObject: Will unban the game object provided from generating color configuration items and changing material colors
		- You'll only ever need to use this if you have banned your object at some point and want to unban it later.
	- AddObject: Will register your object to create a configuration item for color changes
		- This is used by my AutoParentEvent patch to register furniture/upgrade spawned objects.
		- If for whatever reason my patches are not detecting your modded object that you want to register, you can call this method to add it.
		- Once you've added all objects you wish to register, you will need to call RefreshLethalConfig
	- RefreshLethalConfig: Refresh the lethal config menu for shipcolors with any new configuration items since last refresh

<details>
<summary>Please see below for a helpful guide on configuring this mod in "Use Shared Textures" mode: (Credits to Endoxicom)</summary>

![ShipColorChanges by Endoxicom](https://github.com/darmuh/ShipColors/blob/master/shipcolorchanges.png?raw=true)

</details>

For detailed information on configuring this mod in "Generate Config" mode, please see the [ShipColors Generated Google Doc](https://docs.google.com/spreadsheets/d/1v-Oo7XO1jEryLq3D7EHcyXuz_aQYfUyyQ0-3vNQOoOo/edit?gid=0#gid=0) which is also maintained by Endoxicom

See below some early examples of this mod after configuration:
<details>
<summary>Vanilla Example (1)</summary>

![Image 1](https://github.com/darmuh/ShipColors/blob/master/Images/image1.jpg?raw=true)

</details>

<details>
<summary>Vanilla Example (2)</summary>

![Image 2](https://github.com/darmuh/ShipColors/blob/master/Images/image2.jpg?raw=true)

</details>

<details>
<summary>Christmas Ship by Lunxara</summary>

![xmas-lunxara](https://github.com/darmuh/ShipColors/blob/master/Images/xmas-lunxara.jpg?raw=true)

</details>

<details>
<summary>Lunxara Fancy Ship Example (1)</summary>

![lunxarafloor1](https://github.com/darmuh/ShipColors/blob/master/Images/lunxarafloor1.jpg?raw=true)

</details>

<details>
<summary>Lunxara Fancy Ship Example (2)</summary>

![lunxarafloor2](https://github.com/darmuh/ShipColors/blob/master/Images/lunxarafloor2.jpg?raw=true)

</details>