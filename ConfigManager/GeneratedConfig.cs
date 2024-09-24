using BepInEx;
using BepInEx.Configuration;
using ShipColors.Customizer;
using System.IO;
using OpenLib.ConfigManager;
using ShipColors.Events;

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
                    foreach (CustomColorClass item in GeneratedCustomization.materialToColor)
                    {
                        if (item.colorConfig == settingChanged)
                        {
                            GeneratedCustomization.UpdateGeneratedValue(settingChanged);
                            return;
                        }
                    }
                    Plugin.WARNING($"Unable to find {settingChanged.Definition.Key}");
                }
                else if (settingChangedArg.ChangedSetting.BoxedValue.GetType() == typeof(float))
                {
                    Plugin.Spam($"float detected, checking listing ({GeneratedCustomization.materialToColor.Count})");
                    ConfigEntry<float> settingChanged = settingChangedArg.ChangedSetting as ConfigEntry<float>;
                    foreach (CustomColorClass item in GeneratedCustomization.materialToColor)
                    {
                        if (item.alphaConfig == settingChanged)
                        {
                            GeneratedCustomization.UpdateGeneratedValue(settingChanged);
                            return;
                        }
                    }

                    Plugin.WARNING($"Unable to find {settingChangedArg.ChangedSetting.Definition.Key}");
                    return;
                }
                else
                    Plugin.WARNING($"Setting change does not match float or string! Type is {settingChangedArg.ChangedSetting.BoxedValue.GetType()}");
            }
            else
                Plugin.Spam("Not in-game or mode is not using generated config");
        }
    }
}
