using BepInEx.Configuration;
using static OpenLib.ConfigManager.ConfigSetup;

namespace ShipColors.ConfigManager
{
    public static class ConfigSettings
    {
        //MAIN
        public static ConfigEntry<bool> ExtensiveLogging { get; internal set; }
        public static ConfigEntry<bool> DeveloperLogging { get; internal set; }

        //Mode
        public static ConfigEntry<string> ModeSetting { get; internal set; }
        public static ConfigEntry<string> ConfigCode;

        //LIGHTS
        public static ConfigEntry<bool> SetShipLights { get; internal set; }
        public static ConfigEntry<string> ShipLight_1 { get; internal set; }
        public static ConfigEntry<string> ShipLight_2 { get; internal set; }
        public static ConfigEntry<string> ShipLight_3 { get; internal set; }
        public static ConfigEntry<string> ShipLight_4 { get; internal set; }
        public static ConfigEntry<string> ShipLight_5 { get; internal set; }
        public static ConfigEntry<string> ShipLight_6 { get; internal set; }

        //Use Shared Materials
        public static ConfigEntry<string> Mat_TerminalTex { get; internal set; }
        public static ConfigEntry<string> Mat_ScreenOff { get; internal set; }
        public static ConfigEntry<string> Mat_Charger {  get; internal set; }
        public static ConfigEntry<string> Mat_DarkSteel { get; internal set; }
        public static ConfigEntry<string> Mat_ElevatorSteel { get; internal set; }
        public static ConfigEntry<string> Mat_BlackRubber { get; internal set; }
        public static ConfigEntry<string> Mat_DeskBottom { get; internal set; }
        public static ConfigEntry<string> Mat_ControlPanel { get; internal set; }
        public static ConfigEntry<string> Mat_ShipHull { get; internal set; }
        public static ConfigEntry<string> Mat_ShipRoomMetal { get; internal set; }
        public static ConfigEntry<string> Mat_ShipFloor { get; internal set; }
        public static ConfigEntry<string> Mat_BunkBeds { get; internal set; }
        public static ConfigEntry<string> Mat_LockerCabinet { get; internal set; }
        public static ConfigEntry<string> Mat_DoorGenerator { get; internal set; }
        public static ConfigEntry<string> Mat_DoorControlPanel { get; internal set; }
        public static ConfigEntry<string> Mat_ShipDoors { get; internal set; }
        public static ConfigEntry<string> Mat_ShipDoors2 { get; internal set; }


        //Terminal Customization
        public static ConfigEntry<bool> TerminalCustomization { get; internal set; }
        public static ConfigEntry<string> TerminalColor { get; internal set; }
        public static ConfigEntry<string> TerminalButtonsColor { get; internal set; }
        public static ConfigEntry<string> TerminalKeyboardColor { get; internal set; }
        public static ConfigEntry<string> TerminalTextColor { get; internal set; }
        public static ConfigEntry<string> TerminalMoneyColor { get; internal set; }
        public static ConfigEntry<string> TerminalMoneyBGColor { get; internal set; }
        public static ConfigEntry<float> TerminalMoneyBGAlpha { get; internal set; }
        public static ConfigEntry<string> TerminalCaretColor { get; internal set; }
        public static ConfigEntry<string> TerminalScrollbarColor { get; internal set; }
        public static ConfigEntry<string> TerminalScrollBGColor { get; internal set; }
        public static ConfigEntry<string> TerminalLightColor { get; internal set; }
        public static ConfigEntry<bool> TerminalCustomBG { get; internal set; }
        public static ConfigEntry<string> TerminalCustomBGColor { get; internal set; }
        public static ConfigEntry<float> TerminalCustomBGAlpha { get; internal set; }

        public static void BindConfigSettings()
        {
            Plugin.Log.LogInfo("Binding configuration settings");

            //MAIN
            ExtensiveLogging = MakeBool(Plugin.instance.Config, "Debug", "ExtensiveLogging", false, "Enable or Disable extensive logging for this mod.");
            DeveloperLogging = MakeBool(Plugin.instance.Config, "Debug", "DeveloperLogging", false, "Enable or Disable developer logging for this mod. (this will fill your log file FAST)");

            ModeSetting = MakeClampedString(Plugin.instance.Config, "Setup", "ModeSetting", "Generate Config", "Determine whether to generate a config for each material or to use Global Shared Textures", new AcceptableValueList<string>("Use Shared Textures", "Generate Config"));
            ConfigCode = MakeString(Plugin.instance.Config, "Setup", "ConfigCode", "", "Paste your config code from the configuration website here to automatically apply your changes!");

            //LIGHTS
            SetShipLights = MakeBool(Plugin.instance.Config, "Ship Lights", "SetShipLights", true, "Enable or Disable changing ship light colors section");
            ShipLight_1 = MakeString(Plugin.instance.Config, "Ship Lights", "ShipLight_1", "#FFFFFF", "This changes the color of the first ship light");
            ShipLight_2 = MakeString(Plugin.instance.Config, "Ship Lights", "ShipLight_2", "#FFFFFF", "This changes the color of the first ship light");
            ShipLight_3 = MakeString(Plugin.instance.Config, "Ship Lights", "ShipLight_3", "#FFFFFF", "This changes the color of the first ship light");
            ShipLight_4 = MakeString(Plugin.instance.Config, "Ship Lights", "ShipLight_4", "#FFFFFF", "This changes the color of the first ship light");
            ShipLight_5 = MakeString(Plugin.instance.Config, "Ship Lights", "ShipLight_5", "#FFFFFF", "This changes the color of the first ship light");
            ShipLight_6 = MakeString(Plugin.instance.Config, "Ship Lights", "ShipLight_6", "#FFFFFF", "This changes the color of the first ship light");

            //Global Shared Textures
            Mat_TerminalTex = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_TerminalTex", "#CFCFCF", "This changes the color of the physical terminal texture (re-used by other electronics)");
            Mat_Charger = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_Charger", "#5C5840", "This changes the color of the main texture of the charging station");
            Mat_DarkSteel = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_DarkSteel", "#494949", "This changes the color of the DarkSteel material that is used throughout the game");
            Mat_ElevatorSteel = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_ElevatorSteel", "#FFFFFF", "This changes the color of the ElevatorSteel material that is used throughout the game");
            Mat_BlackRubber = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_BlackRubber", "#2E2E2E", "This changes the color of the BlackRubber material that is used throughout the game");
            Mat_ScreenOff = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_ScreenOff", "#000000", "This changes the color of the ScreenOff material that is used throughout the game");
            Mat_DeskBottom = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_DeskBottom", "#545250", "This changes the color of the Desk Bottom texture by the ship monitors and everywhere else it is re-used");
            Mat_ControlPanel = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_ControlPanel", "#6C6862", "This changes the color of the ControlPanel shared material that is used throughout the game");
            Mat_ShipFloor = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_ShipFloor", "#C8BDB8", "This changes the color of the ShipFloor shared material that is used throughout the game");
            Mat_ShipHull = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_ShipHull", "#FFFFFF", "This changes the color of the ShipHull shared material that is used throughout the game");
            Mat_ShipRoomMetal = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_ShipRoomMetal", "#D6C9B5", "This changes the color of the ShipRoomMetal shared material that is used throughout the game");
            Mat_BunkBeds = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_BunkBeds", "#CFCFCF", "This changes the color of the BunkBeds shared material that is used throughout the game");
            Mat_LockerCabinet = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_LockerCabinet", "#7D2426", "This changes the color of the LockerCabinet shared material that is used throughout the game");
            Mat_ShipDoors = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_ShipDoors", "#71665E", "This changes the color of the ShipDoors shared material that is used throughout the game");
            Mat_ShipDoors2 = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_ShipDoors2", "#6F645D", "This changes the color of the ShipDoors2 shared material that is used throughout the game");
            Mat_DoorGenerator = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_DoorGenerator", "#FFECD0", "This changes the color of the DoorGenerator shared material that is used throughout the game");
            Mat_DoorControlPanel = MakeString(Plugin.instance.Config, "Global Shared Textures", "Mat_DoorControlPanel", "#373732", "This changes the color of the DoorControlPanel shared material that is used throughout the game");
                
            RemoveOrphanedEntries(Plugin.instance.Config);
        }
    }
}