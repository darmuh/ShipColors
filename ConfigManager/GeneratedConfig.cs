using BepInEx;
using BepInEx.Configuration;
using ShipColors.Customizer;
using System.IO;
using OpenLib.ConfigManager;

namespace ShipColors.ConfigManager
{
    public static class GeneratedConfig
    {
        public static ConfigFile Generated = new(Path.Combine(Paths.ConfigPath, $"{Plugin.PluginInfo.PLUGIN_NAME}_Generated.cfg"), true);

        public static void GenerateWebpage()
        {
            WebHelper.WebConfig(Generated);
        }

        public static void RegenerateConfig()
        {
            if (!GeneratedCustomization.configGenerated)
                return;

            GeneratedCustomization.configGenerated = false;
            GeneratedCustomization.CreateAllConfigs();
        }

        public static void ClearOrphans()
        {
            ConfigSetup.RemoveOrphanedEntries(Generated);
        }

        public static void RefreshLethalConfigMenu()
        {
            if (OpenLib.Plugin.instance.LethalConfig)
                Compat.LethalConfigStuff.AddConfig(Generated);
        }

        public static void ReadConfigCode()
        {
            if (ConfigSettings.ConfigCode.Value == string.Empty)
                return;

            if (!GeneratedCustomization.configGenerated)
                return;

            WebHelper.ReadCompressedConfig(ref ConfigSettings.ConfigCode, Generated);
            GeneratedCustomization.ReadCustomClassValues(ref GeneratedCustomization.materialToColor);
        }

        internal static void OnSettingChanged(object sender, SettingChangedEventArgs settingChangedArg)
        {
            Plugin.Spam("Generated config setting changed!");

            if (StartOfRound.Instance != null)
            {
                if (settingChangedArg.ChangedSetting.BoxedValue.GetType() == typeof(string))
                {
                    Plugin.Spam($"String detected, checking listing ({GeneratedCustomization.materialToColor.Count})");
                    ConfigEntry<string> settingChanged = settingChangedArg.ChangedSetting as ConfigEntry<string>;
                    CustomColorClass item = GeneratedCustomization.materialToColor.Find(x => x.colorConfig == settingChanged);
                    if(item != null)
                    {
                        GeneratedCustomization.UpdateGeneratedValue(settingChanged);
                        return;
                    }

                    Plugin.Log.LogInfo($"Unable to find an existing setting for {settingChanged.Definition.Key}");
                }
                else if (settingChangedArg.ChangedSetting.BoxedValue.GetType() == typeof(float))
                {
                    Plugin.Spam($"float detected, checking listing ({GeneratedCustomization.materialToColor.Count})");
                    ConfigEntry<float> settingChanged = settingChangedArg.ChangedSetting as ConfigEntry<float>;
                    CustomColorClass item = GeneratedCustomization.materialToColor.Find(x => x.alphaConfig == settingChanged);
                    if (item != null)
                    {
                        GeneratedCustomization.UpdateGeneratedValue(settingChanged);
                        return;
                    }

                    Plugin.Log.LogInfo($"Unable to find an existing setting for {settingChangedArg.ChangedSetting.Definition.Key}");
                    return;
                }
                else
                    Plugin.WARNING($"Setting {settingChangedArg.ChangedSetting.Definition.Key} does not match float or string! Type is {settingChangedArg.ChangedSetting.BoxedValue.GetType()}");
            }
        }
    }
}
