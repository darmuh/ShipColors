using ShipColors.ConfigManager;
using ShipColors.Customizer;
using ShipColors.Events;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ShipColors
{
    public class API
    {
        public static void InitCustomization()
        {
            string YourPluginName = Assembly.GetCallingAssembly().GetName().Name;
            Plugin.Spam($"{YourPluginName} has called InitCustomization() from ShipColors.API!");
            Subscribers.StartCustomizer();
        }

        public static void RegenConfigItems()
        {
            string YourPluginName = Assembly.GetCallingAssembly().GetName().Name;
            Plugin.Spam($"{YourPluginName} has called RegenConfigItems() from ShipColors.API!");
            GeneratedConfig.RegenerateConfig();
        }

        public static void RefreshLethalConfig()
        {
            string YourPluginName = Assembly.GetCallingAssembly().GetName().Name;
            Plugin.Spam($"{YourPluginName} has called RefreshLethalConfigMenu() from ShipColors.API!");
            GeneratedConfig.RefreshLethalConfigMenu();
        }

        public static void BanObject(GameObject gameObject)
        {
            if (gameObject == null)
                return;

            string YourPluginName = Assembly.GetCallingAssembly().GetName().Name;
            Plugin.Spam($"{YourPluginName} has called BanObject() for GameObject [ {gameObject.name} ] from ShipColors.API!");

            if (GeneratedCustomization.DoNotTouchList.Contains(gameObject))
                Plugin.Log.LogInfo($"{gameObject.name} is already designated as BANNED!");
            else
            {
                GeneratedCustomization.DoNotTouchList.Add(gameObject);
                Plugin.Log.LogInfo($"{gameObject.name} is now designated as BANNED!");
            }
        }

        public static void UnbanObject(GameObject gameObject)
        {
            if (gameObject == null) 
                return;

            string YourPluginName = Assembly.GetCallingAssembly().GetName().Name;
            Plugin.Spam($"{YourPluginName} has called UnbanObject() for GameObject [ {gameObject.name} ] from ShipColors.API!");

            if (GeneratedCustomization.DoNotTouchList.Contains(gameObject))
            {
                GeneratedCustomization.DoNotTouchList.Remove(gameObject);
                Plugin.Log.LogInfo($"{gameObject.name} has been unbanned!");
            }
            else
                Plugin.Log.LogInfo($"{gameObject.name} is already unbanned!");

        }

        public static void AddObject(GameObject gameObject, string customSection = "", string customName = "")
        {
            if (gameObject == null) 
                return;

            string YourPluginName = Assembly.GetCallingAssembly().GetName().Name;
            Plugin.Spam($"{YourPluginName} has called AddObject() for GameObject [ {gameObject.name} ] from ShipColors.API!");

            if (ConfigSettings.ModeSetting.Value == "Use Shared Textures")
            {
                Plugin.WARNING($"Not adding modded object, ModeSetting is set to Use Shared Textures");
                return;
            }    

            if (GeneratedCustomization.ObjectsWithConfigItems.Contains(gameObject))
            {
                Plugin.MoreLogs($"Object: {gameObject.name} already accounted for!");
                return;
            }
            else
            {
                List<int> acceptableLayers = OpenLib.Common.CommonStringStuff.GetNumberListFromStringList(OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenAcceptedLayers.Value, ','));
                List<string> bannedObjects = OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenBannedObjects.Value, ',');
                List<string> bannedMaterials = OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenBannedMaterials.Value, ',');
                List<string> permitListObjects = OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenPermitListObjects.Value, ',');

                GeneratedCustomization.ProcessObjectFamily(gameObject, bannedMaterials, bannedObjects, acceptableLayers, permitListObjects, customSection, customName);
            }
        }

    }
}
