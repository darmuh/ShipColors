﻿using OpenLib.Events;
using OpenLib.Common;
using ShipColors.ConfigManager;
using ShipColors.Customizer;
using System.Reflection;
using System;


namespace ShipColors.Events
{
    public class Subscribers
    {
        public static void Subscribe()
        {
            EventManager.TerminalAwake.AddListener(OnTerminalAwake);
            EventManager.TerminalStart.AddListener(OnStart);
            EventManager.TerminalDisable.AddListener(OnTerminalDisable);
            EventManager.GameNetworkManagerStart.AddListener(OnGameLoad);
        }

        public static void OnGameLoad()
        {
            if(StartGame.SoftCompatibility("darmuh.TerminalStuff", ref Plugin.instance.darmuhsTerminalStuff))
            {
                Plugin.MoreLogs("leaving terminal customization to darmuhsTerminalStuff (generated config will skip terminal object)");
            }
            if(StartGame.SoftCompatibility("BMX.LobbyCompatibility", ref Plugin.instance.LobbyCompat))
            {
                Version version = Assembly.GetExecutingAssembly().GetName().Version;
                OpenLib.Compat.BMX_LobbyCompat.SetBMXCompat(false, version);
            }
            if (StartGame.SoftCompatibility("TestAccount666.ShipWindows", ref Plugin.instance.ShipWindows))
                Plugin.Spam("Will rely on ShipWindows to call customization");

            if (OpenLib.Plugin.instance.LethalConfig)
            {
                OpenLib.Compat.LethalConfigSoft.AddButton("Setup", "Generate Webpage", "Press this to generate a webpage in the Bepinex/Config/Webconfig folder from this mod's generated config!\nYou can then use this webpage to modify your config and paste a config code to apply in-game", "Generate Webpage", GeneratedConfig.GenerateWebpage);
                OpenLib.Compat.LethalConfigSoft.AddButton("Setup", "Regen Config", "Press this to regenerate the generated config when [Mode Setting] is set to Generate Config.", "Regen Config", GeneratedConfig.RegenerateConfig);
            }
            else
                Plugin.MoreLogs("LethalConfig is not detected by OpenLib");
            
        }

        public static void OnTerminalAwake(Terminal instance)
        {
            Plugin.instance.Terminal = instance;
            Plugin.MoreLogs($"Setting Plugin.instance.Terminal");
        }

        internal static void StartCustomizer()
        {
            if (ConfigSettings.SetShipLights.Value)
            {
                CustomShipLights.SetShipLights();
            }

            if (ConfigSettings.ModeSetting.Value == "Use Shared Textures")
            {
                GlobalSharedCustomization.UseSharedTextures();
                Plugin.Spam("Only setting shared texture values");
                return;
            }
            else if(ConfigSettings.ModeSetting.Value == "Generate Config")
            {
                if (!GeneratedCustomization.configGenerated)
                {
                    GeneratedCustomization.CreateAllConfigs();
                    GeneratedConfig.Generated.SettingChanged += GeneratedConfig.OnSettingChanged;
                    GeneratedConfig.ReadConfigCode();
                }
                    
                else
                    GeneratedCustomization.ReadCustomClassValues(ref GeneratedCustomization.materialToColor);
            }
            
            //TerminalCustomizations.TerminalStuff();
        }

        public static void OnTerminalDisable()
        {
            GeneratedCustomization.materialToColor.Clear();
            GeneratedCustomization.configGenerated = false;
        }

        public static void OnStart()
        {
            if (Plugin.instance.ShipWindows)
                return;

            Plugin.Log.LogInfo("patching customizations now!");
            StartCustomizer();
        }

    }
}
