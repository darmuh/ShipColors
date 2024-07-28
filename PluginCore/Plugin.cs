﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using ShipColors.ConfigManager;
using ShipColors.Events;
using Steamworks.Data;
using System.Reflection;
using UnityEngine;


namespace ShipColors
{
    [BepInPlugin("darmuh.ShipColors", "ShipColors", (PluginInfo.PLUGIN_VERSION))]
    [BepInDependency("darmuh.OpenLib", "0.1.6")]


    public class Plugin : BaseUnityPlugin
    {
        public static Plugin instance;
        public static class PluginInfo
        {
            public const string PLUGIN_GUID = "darmuh.ShipColors";
            public const string PLUGIN_NAME = "ShipColors";
            public const string PLUGIN_VERSION = "0.1.0";
        }

        internal static ManualLogSource Log;

        //Compatibility
        public bool LobbyCompat = false;
        public bool darmuhsTerminalStuff = false;
        public Terminal Terminal;


        private void Awake()
        {
            instance = this;
            Log = base.Logger;
            Log.LogInfo((object)$"{PluginInfo.PLUGIN_NAME} is loading with version {PluginInfo.PLUGIN_VERSION}!");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            ConfigSettings.BindConfigSettings();
            Config.SettingChanged += OnSettingChanged;
            Subscribers.Subscribe();
            Log.LogInfo($"{PluginInfo.PLUGIN_NAME} load complete!");
        }

        internal void OnSettingChanged(object sender, SettingChangedEventArgs settingChangedArg)
        {
            Spam("CONFIG SETTING CHANGE EVENT");
            if (settingChangedArg.ChangedSetting == null)
                return;

            if (StartOfRound.Instance != null)
                Subscribers.StartCustomizer(); //refresh customizations

        }

         internal static void MoreLogs(string message)
        {
            if (ConfigSettings.ExtensiveLogging.Value)
                Log.LogInfo(message);
            else
                return;
        }

        internal static void Spam(string message)
        {
            if (ConfigSettings.DeveloperLogging.Value)
                Log.LogDebug(message);
            else
                return;
        }

        internal static void ERROR(string message)
        {
            Log.LogError(message);
        }

        internal static void WARNING(string message)
        {
            Log.LogWarning(message);
        }
    }

}