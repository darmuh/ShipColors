using BepInEx.Configuration;
using ShipColors.ConfigManager;
using static OpenLib.Common.Misc;

using UnityEngine;

namespace ShipColors.Customizer
{
    internal class CustomShipLights
    {

        internal static void SetShipLights()
        {
            SetLightColor("Environment/HangarShip/ShipElectricLights/Area Light (3)", ConfigSettings.ShipLight_1);
            SetLightColor("Environment/HangarShip/ShipElectricLights/Area Light (9)", ConfigSettings.ShipLight_4);
            
            SetLightColor("Environment/HangarShip/ShipElectricLights/Area Light (4)", ConfigSettings.ShipLight_2); 
            SetLightColor("Environment/HangarShip/ShipElectricLights/Area Light (8)", ConfigSettings.ShipLight_5);

            SetLightColor("Environment/HangarShip/ShipElectricLights/Area Light (5)", ConfigSettings.ShipLight_3);
            SetLightColor("Environment/HangarShip/ShipElectricLights/Area Light (7)", ConfigSettings.ShipLight_6);
        }

        internal static void SetLightColor(string GameObjectFind, ConfigEntry<string> setting)
        {
            if(GameObject.Find(GameObjectFind) != null)
            {
                if(GameObject.Find(GameObjectFind).GetComponent<Light>() != null)
                {
                    GameObject.Find(GameObjectFind).GetComponent<Light>().color = HexToColor(setting.Value);
                    Plugin.Spam($"{setting.Definition.Key} has been set for light at path {GameObjectFind}");
                }
            }
        }
    }
}
