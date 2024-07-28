using OpenLib.Events;
using OpenLib.Common;
using ShipColors.ConfigManager;
using ShipColors.Customizer;


namespace ShipColors.Events
{
    public class Subscribers
    {
        public static void Subscribe()
        {
            EventManager.TerminalAwake.AddListener(OnTerminalAwake);
            EventManager.StartOfRoundAwake.AddListener(OnStart);
            EventManager.TerminalDisable.AddListener(OnTerminalDisable);
            EventManager.GameNetworkManagerStart.AddListener(OnGameLoad);
        }

        public static void OnGameLoad()
        {
            if(StartGame.SoftCompatibility("ShipColors", "terminalstuff", ref Plugin.instance.darmuhsTerminalStuff))
            {
                Plugin.MoreLogs("leaving terminal customization to darmuhsTerminalStuff");
            }
            if(StartGame.SoftCompatibility("ShipColors", "bmx", ref Plugin.instance.LobbyCompat))
            {
                Compat.BMX_LobbyCompat.SetCompat(false);
            }
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

            if (ConfigSettings.UseSharedMaterials.Value)
            {
                GlobalSharedCustomization.UseSharedTextures();
                Plugin.Spam("Only setting shared texture values");
                return;
            }
            
            //TerminalCustomizations.TerminalStuff();
        }

        public static void OnTerminalDisable()
        {
            //nothing needed here
        }

        public static void OnStart()
        {
            Plugin.Log.LogInfo("patching customizations now!");
            StartCustomizer();
        }

    }
}
