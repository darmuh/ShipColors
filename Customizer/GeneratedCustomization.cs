using static OpenLib.ConfigManager.ConfigSetup;
using static OpenLib.Common.Misc;
using static ShipColors.ConfigManager.GeneratedConfig;
using BepInEx.Configuration;
using UnityEngine;
using System.Collections.Generic;
using ShipColors.ConfigManager;

namespace ShipColors.Customizer
{
    internal class GeneratedCustomization
    {
        internal static List<CustomColorClass> materialToColor = [];
        internal static bool configGenerated = false;
        internal static void CreateAllConfigs()
        {
            if (configGenerated)
                return;

            materialToColor.Clear();
            List<int> acceptableLayers = OpenLib.Common.CommonStringStuff.GetNumberListFromStringList(OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenAcceptedLayers.Value, ','));
            List<string> bannedObjects = OpenLib.Common.CommonStringStuff.GetListToLower(OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenBannedObjects.Value, ','));
            List<string> bannedMaterials = OpenLib.Common.CommonStringStuff.GetListToLower(OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenBannedMaterials.Value, ','));

            bool addTerminal = false;
            if(Plugin.instance.darmuhsTerminalStuff)
                addTerminal = Compat.MyTerminalStuff.AddTerminalConfigs();

            if (TryGetAllGameObjects(out List<GameObject> allObjects))
            {
                foreach (GameObject gameObject in allObjects)
                {
                    if (bannedObjects.Contains(gameObject.name.ToLower()))
                        continue;

                    if (!acceptableLayers.Contains(gameObject.layer))
                        continue;

                    if (!addTerminal && gameObject.name.ToLower() == "terminal")
                        continue;

                    if(TryGetChildObjects(gameObject, out List<GameObject> children))
                    {
                        foreach(GameObject child in children)
                        {
                            if (TryGetAllMeshRenderer(child, out MeshRenderer[] meshes))
                                MakeConfigItems(bannedMaterials, meshes, child, gameObject);
                        }
                    }
                    else
                    {
                        if (TryGetAllMeshRenderer(gameObject, out MeshRenderer[] meshes))
                        {
                            //Plugin.Spam("Attempting to create meshrenderer config item");
                            MakeConfigItems(bannedMaterials, meshes, gameObject);
                        }
                    }

                    
                }
            }
            else
                Plugin.WARNING("Unable to get any objects!");

            if(OpenLib.Plugin.instance.LethalConfig)
                Compat.LethalConfigStuff.AddConfig(Generated);
            Plugin.Spam("Config has been generated");
            configGenerated = true;
        }

        private static void MakeConfigItems(List<string> bannedMaterials, MeshRenderer[] meshes, GameObject gameObject, GameObject parent = null)
        {
            foreach (MeshRenderer meshRenderer in meshes)
            {
                //Plugin.Spam($"meshRenderer: {meshRenderer.name} / materials count: {meshRenderer.materials.Length}");
                if (meshRenderer.materials.Length < 1)
                {
                    Plugin.WARNING($"No materials in {meshRenderer.name}");
                    continue;
                }

                foreach (Material material in meshRenderer.materials)
                {
                    if (bannedMaterials.Contains(material.name.ToLower()))
                        continue;

                    ConfigEntry<string> colorEntry;
                    ConfigEntry<float> colorFloatEntry;
                    string color = ColorUtility.ToHtmlStringRGB(material.color);
                    if(parent != null)
                    {
                        colorEntry = MakeString(Generated, $"{parent.name} Colors", $"{gameObject.name}/{material.name} Color", "#" + color, $"Change color of material, {material.name} as part of object {gameObject.name}");
                        colorFloatEntry = MakeClampedFloat(Generated, $"{parent.name} Colors", $"{gameObject.name}/{material.name} Alpha", material.color.a, $"Change alpha of material, {material.name} as part of object {gameObject.name}", 0, 1);
                    }
                    else
                    {
                        colorEntry = MakeString(Generated, $"{gameObject.name} Colors", $"{material.name} Color", "#" + color, $"Change color of material, {material.name} as part of object {gameObject.name}");
                        colorFloatEntry = MakeClampedFloat(Generated, $"{gameObject.name} Colors", $"{material.name} Alpha", material.color.a, $"Change alpha of material, {material.name} as part of object {gameObject.name}", 0, 1);
                    }
                    
                    Color newColor = HexToColor(colorEntry.Value);
                    newColor.a = colorFloatEntry.Value;
                    material.color = newColor;
                    Plugin.Spam($"Config item created for {material.name} in Section: {gameObject.name}");
                    Plugin.Spam($"{material.name} set to color - {colorEntry.Value} with alpha {colorFloatEntry.Value}");
                    CustomColorClass CustomColorClass = new(colorEntry, colorFloatEntry, material);
                    materialToColor.Add(CustomColorClass);
                }
            }
        }

        internal static void ReadCustomClassValues(ref List<CustomColorClass> config)
        {
            foreach(CustomColorClass item in config)
            {
                Color newColor = HexToColor(item.colorConfig.Value);
                newColor.a = item.alphaConfig.Value;
                item.material.color = newColor;
                Plugin.Spam($"set color for {item.material.color} to {item.colorConfig.Value} with alpha {item.alphaConfig.Value}");
            }
        }

        internal static void UpdateGeneratedValue(ConfigEntry<string> valueUpdated)
        {
            Plugin.Spam($"Attempting to update value: {valueUpdated.Definition.Key}");
            foreach(CustomColorClass item in materialToColor)
            {
                if(item.TryGetItem(valueUpdated, out CustomColorClass newConfig))
                {
                    Color newColor = HexToColor(valueUpdated.Value);
                    newColor.a = newConfig.alphaConfig.Value;
                    newConfig.material.color = newColor;
                    Plugin.Spam($"{newConfig.material.name} updated to {valueUpdated.Value}");
                    return;
                }
            }
            Plugin.WARNING($"Could not find {valueUpdated.Definition.Key} in material listing ({materialToColor.Count})");
        }

        internal static void UpdateGeneratedValue(ConfigEntry<float> valueUpdated)
        {
            Plugin.Spam($"Attempting to update value: {valueUpdated.Definition.Key}");
            foreach (CustomColorClass item in materialToColor)
            {
                if (item.TryGetItem(valueUpdated, out CustomColorClass newConfig))
                {
                    Color newColor = HexToColor(newConfig.colorConfig.Value);
                    newColor.a = valueUpdated.Value;
                    newConfig.material.color = newColor;
                    Plugin.Spam($"{newConfig.material.name} updated to {valueUpdated.Value}");
                    return;
                }
            }
            Plugin.WARNING($"Could not find {valueUpdated.Definition.Key} in material listing ({materialToColor.Count})");
        }

        private static bool TryGetAllMeshRenderer(GameObject gameObject, out MeshRenderer[] meshes)
        {
            if (gameObject == null)
            {
                meshes = null;
                Plugin.WARNING($"GameObject provided is null @TryGetAllMeshRenderer");
                return false;
            }

            meshes = gameObject.GetComponentsInChildren<MeshRenderer>();
            if (meshes == null)
                return false;
            else
                return true;
        }

        private static bool TryGetAllGameObjects(out List<GameObject> gameObjects)
        {
            GameObject parent = GameObject.Find("Environment/HangarShip");
            gameObjects = null;
            if(parent == null)
            {
                Plugin.WARNING($"Unable to find object at path Environment/HangarShip");
                return false;
            }

            if (TryGetChildObjects(parent, out gameObjects))
                return true;
            else
                return false;
            
        }

        private static bool TryGetChildObjects(GameObject parent, out List<GameObject> allObjects)
        {
            allObjects = null;
            if (parent == null)
            {
                Plugin.WARNING($"Unable to find object at path Environment/HangarShip");
                return false;
            }
            Plugin.Spam($"GetChildCount: {parent.transform.childCount}");

            if (parent.transform.childCount == 0)
                return false;

            allObjects = [];
            for(int i = 0; i < parent.transform.childCount; i++)
            {
                allObjects.Add(parent.transform.GetChild(i).gameObject);
                Plugin.Spam($"Object found through child transform - {parent.transform.GetChild(i).gameObject.name}");
            }

            return true;

        }
    }
    
    internal class CustomColorClass
    {
        internal ConfigEntry<string> colorConfig;
        internal ConfigEntry<float> alphaConfig;
        internal Material material;

        internal CustomColorClass(ConfigEntry<string> stringEntry, ConfigEntry<float> floatEntry, Material mat)
        {
            this.colorConfig = stringEntry;
            this.alphaConfig = floatEntry;
            this.material = mat;
        }

        internal bool TryGetItem(ConfigEntry<string> configEntry, out CustomColorClass item)
        {
            if(configEntry == colorConfig)
            {
                item = this;
                return true;
            }
            item = null;
            return false;
        }
        internal bool TryGetItem(ConfigEntry<float> configEntry, out CustomColorClass item)
        {
            if (configEntry == alphaConfig)
            {
                item = this;
                return true;
            }
            item = null;
            return false;
        }
    }
}
