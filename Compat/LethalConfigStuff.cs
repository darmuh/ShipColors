using BepInEx.Configuration;
using LethalConfig;

namespace ShipColors.Compat
{
    internal class LethalConfigStuff
    {
        public static void AddConfig(ConfigFile configName)
        {
            if (!OpenLib.Compat.LethalConfigSoft.IsLethalConfigUpdated())
                return;

            Plugin.Spam($"Queuing file {configName.ConfigFilePath}");
            LethalConfigManager.QueueCustomConfigFileForLateAutoGeneration(configName);
            LethalConfigManager.RunLateAutoGeneration();
        }
    }
}
