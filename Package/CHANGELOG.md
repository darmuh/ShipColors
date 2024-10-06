# Change Log

All notable changes to this project will be documented in this file.
 
The format is based on [Keep a Changelog](http://keepachangelog.com/).

## [0.2.4]
 - Removed generation of useless alpha config items (material colors with alpha set to 1 by default)
 - Added new config item [GenPermitListObjects] to explicitly permit game objects to generate configs, even if the game object matches something in [GenBannedObjects]
 - Updated to latest version of OpenLib
 - Updated default configurations with new GenPermitListObjects in mind (suggest using these settings)

## [0.2.3]
 - Further improved configuration generation.
	- the funny log message appeared so now we are tracking an object's entire family tree
	- Emptied objects out of the ShipModels2b black-hole into multiple sections
	- Hopefully should be the last time this is reworked completely
 - Fixed banlist config options and they now allow partial matches to object/material names
 - Updated default configuration items
	- Screens added by GI and screens modified by OpenBodyCams should now be automatically ignored by default config items

## [0.2.2]
 - Improved configuration generation & sorting.
	- Will now check for game objects with children & grand children.
		- Hopefully no game objects need a deeper dive than that, if this happens you will see a fun log message.
	- Configuration sections are now labeled by the root parent object of the ship.
		- This results in some config item names being pretty long to display their full path :)
 - Moved config generation to TerminalStart (which is a bit later than before)
	- This will hopefully solve issues where certain objects were not added until config regen.

## [0.2.1]
 - Fixed darmuhsTerminalStuff soft compatibility
 - Fixed ShipWindows compatibility for Generated Config
 - Adjusted Config Generation to sort game objects with children into one singular section and to list each child's materials
	- This should fix cases where one of the storage closet's doors would not update their color
 - Added configuration items for more customization as to what generates in the config
	- GenAcceptedLayers: [Comma-separated listing] - This sets the acceptable layers to search for materials in each GameObject in the ship. 
		- If a GameObject has a layer not specified here it will be skipped.
	- GenBannedObjects: [Comma-separated listing] - This listing of game objects will be skipped and no config section will be generated.
		- Existing config section/items will be deleted on re-gen.
	- GenBannedMaterials: [Comma-separated listing] - This listing of materials will be skipped and no config item will be generated.
		- Existing config items will be deleted on re-gen.

## [0.2.0]
 - Added new mode that will generate config items for different textures located within game objects on the ship.
	- This is done dynamically and can be generated via a new button added in-game with LethalConfig.
 - Updated to OpenLib v0.2.0, with this update comes support for HTML Config Page generation.
	- A new button has been added in-game which will generate a webpage for another way to modify your configuration.
	- The html page is located as a file in your mod profile folder at Bepinex/config/webconfig
	- You can import your current config file or work from the defaults and then copy the code at the bottom of the page to paste in the new [ConfigCode] setting.
 - With OpenLib v0.2.0 and LethalConfigv1.4.3 the generated configs are added directly to the game after they are created.
	- Modifying any generated config item will see their corresponding material color update immediately.
 - Added the 3 other ship lights for further customization (they were originally being changed with their accompanying light 1/4,3/5,4/6)
 - The new generated configs will also grab modded prop's materials.
	- This has been tested only with ShipInventory's chute. If the modded prop's materials is not being grabbed you can try regenerating the config with the "regen config" button that is added via LethalConfig.
 
## [0.1.0]
 - Initial Release.