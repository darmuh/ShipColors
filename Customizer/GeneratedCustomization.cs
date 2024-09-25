using static OpenLib.ConfigManager.ConfigSetup;
using static OpenLib.Common.Misc;
using static ShipColors.ConfigManager.GeneratedConfig;
using BepInEx.Configuration;
using UnityEngine;
using System.Collections.Generic;
using ShipColors.ConfigManager;
using Color = UnityEngine.Color;
using System.Linq;
using UnityEngine.InputSystem.HID;

namespace ShipColors.Customizer
{
    internal class GeneratedCustomization
    {
        internal static List<CustomColorClass> materialToColor = [];
        internal static List<GameObject> ObjectsWithConfigItems = [];
        internal static bool configGenerated = false;
        internal static void CreateAllConfigs()
        {
            if (configGenerated)
                return;

            materialToColor.Clear();
            ObjectsWithConfigItems.Clear();
            List<int> acceptableLayers = OpenLib.Common.CommonStringStuff.GetNumberListFromStringList(OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenAcceptedLayers.Value, ','));
            List<string> bannedObjects = OpenLib.Common.CommonStringStuff.GetListToLower(OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenBannedObjects.Value, ','));
            List<string> bannedMaterials = OpenLib.Common.CommonStringStuff.GetListToLower(OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenBannedMaterials.Value, ','));

            bool addTerminal = false;
            if(Plugin.instance.darmuhsTerminalStuff)
                addTerminal = Compat.MyTerminalStuff.AddTerminalConfigs();

            GameObject topLevel = GameObject.Find("Environment/HangarShip");
            if (topLevel == null)
            {
                Plugin.ERROR("Unable to find root ship object @ Environment/HangarShip");
                return;
            }

            if (TryGetChildObjects(topLevel, out List<GameObject> allObjects))
            {
                Plugin.Spam($"allObjects Count - {allObjects.Count}");
                allObjects = [.. allObjects.OrderBy(x => x.name)];
                foreach (GameObject gameObject in allObjects)
                {
                    if (bannedObjects.Contains(gameObject.name.ToLower()))
                        continue; //game object name is blocked

                    if (!acceptableLayers.Contains(gameObject.layer))
                        continue; //game object layer is being filtered out

                    if (!addTerminal && gameObject.name.ToLower() == "terminal")
                        continue; //skip terminal, covered by darmuhsTerminalStuff

                    if(ObjectsWithConfigItems.Contains(gameObject))
                        continue; //this object already has a config item created

                    if (allObjects.Any(p => p.transform == gameObject.transform.parent))
                        continue; //this object has a parent object in the list, skip for now

                    if (IsNotRootGameObject(gameObject, out List<GameObject> children, out Dictionary<GameObject,GameObject> family))
                    {
                        //family.Add(grandchild, child);
                        if (family.Count > 0)
                        {
                            //process grandchildren
                            foreach (KeyValuePair<GameObject,GameObject> member in family)
                            {
                                if (bannedObjects.Contains(member.Key.name.ToLower()) || ObjectsWithConfigItems.Contains(member.Key))
                                    continue;

                                if (!acceptableLayers.Contains(member.Key.layer))
                                    continue;

                                //grandchildren config item
                                if (TryGetAllMeshRenderer(member.Key, out MeshRenderer[] meshes))
                                {
                                    MakeConfigItems(bannedMaterials, meshes, member.Key, member.Value, gameObject);
                                    ObjectsWithConfigItems.Add(member.Key);
                                }

                                if (bannedObjects.Contains(member.Value.name.ToLower()) || ObjectsWithConfigItems.Contains(member.Value))
                                    continue;

                                if (!acceptableLayers.Contains(member.Value.layer))
                                    continue;

                                //child parent config item
                                if (TryGetMeshRenderers(member.Value, out MeshRenderer[] mesh))
                                {
                                    MakeConfigItems(bannedMaterials, mesh, member.Value, gameObject);
                                    ObjectsWithConfigItems.Add(member.Value);
                                }
                            }

                            ProcessChildren(children, bannedMaterials, bannedObjects, acceptableLayers, gameObject);

                            //main parent config item
                            if (TryGetMeshRenderers(gameObject, out MeshRenderer[] any))
                            {
                                MakeConfigItems(bannedMaterials, any, gameObject);
                                ObjectsWithConfigItems.Add(gameObject);
                            }
                        }
                        else
                        {
                            ProcessChildren(children, bannedMaterials, bannedObjects, acceptableLayers, gameObject);

                            //parent config item
                            if (TryGetMeshRenderers(gameObject, out MeshRenderer[] mesh))
                            {
                                MakeConfigItems(bannedMaterials, mesh, gameObject);
                                ObjectsWithConfigItems.Add(gameObject);
                            }
                        }
                    }
                    else
                    {
                        if (TryGetAllMeshRenderer(gameObject, out MeshRenderer[] meshes))
                        {
                            //objects without children
                            MakeConfigItems(bannedMaterials, meshes, gameObject);
                            ObjectsWithConfigItems.Add(gameObject);
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

        private static void ProcessChildren(List<GameObject> children, List<string> bannedMaterials, List<string> bannedObjects, List<int> acceptableLayers, GameObject gameObject)
        {
            foreach (GameObject child in children)
            {
                if (bannedObjects.Contains(child.name.ToLower()) || ObjectsWithConfigItems.Contains(child))
                    continue;

                if (!acceptableLayers.Contains(child.layer))
                    continue;

                //children config item
                if (TryGetAllMeshRenderer(child, out MeshRenderer[] meshes))
                {
                    MakeConfigItems(bannedMaterials, meshes, child, gameObject);
                    ObjectsWithConfigItems.Add(child);
                }
            }
        }

        private static bool IsNotRootGameObject(GameObject thisObject, out List<GameObject> children, out Dictionary<GameObject,GameObject> family)
        {
            children = [];
            family = [];

            Plugin.Spam($"Checking IsNotRootGameObject {thisObject.name}");

            if (thisObject == null)
            {
                Plugin.ERROR("NULL OBJECT SENT TO IsNotRootGameObject");
                return false;
            }
            
            //this object has no children to return
            if (thisObject.transform.childCount == 0)
                return false;

            if (TryGetChildObjects(thisObject, out children))
            {
                foreach (GameObject child in children)
                {
                    if (child.transform.childCount == 0)
                        continue;
                    else
                    {
                        if (TryGetChildObjects(child, out List<GameObject> grandChildren))
                        {
                            foreach(GameObject grandchild in grandChildren)
                            {
                                family.Add(grandchild, child);
                            }
                            Plugin.Spam($"Grandchildren [ {grandChildren.Count} ] found in {thisObject.name}/{child.name}");
                        }
                    }
                }

                return true;
            }
            else
                return false;


        }

        private static void MakeConfigItems(List<string> bannedMaterials, MeshRenderer[] meshes, GameObject gameObject, GameObject parent = null, GameObject grandparent = null)
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
                    if(parent != null && grandparent != null)
                    {
                        colorEntry = MakeString(Generated, $"{grandparent.name} Colors", $"{parent.name}/{gameObject.name}/{material.name} Color", "#" + color, $"Change color of material, {material.name} as part of object {gameObject.name}");
                        colorFloatEntry = MakeClampedFloat(Generated, $"{grandparent.name} Colors", $"{parent.name}/{gameObject.name}/{material.name} Alpha", material.color.a, $"Change alpha of material, {material.name} as part of object {gameObject.name} in {grandparent.name}", 0, 1);
                    }
                    else if(parent != null && grandparent == null)
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
                    //Plugin.Spam($"Config item created for {material.name} in Section: {gameObject.name}");
                    //Plugin.Spam($"{material.name} set to color - {colorEntry.Value} with alpha {colorFloatEntry.Value}");
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

        private static bool TryGetMeshRenderers(GameObject gameObject, out MeshRenderer[] mesh)
        {
            if (gameObject == null)
            {
                mesh = null;
                Plugin.WARNING($"GameObject provided is null @TryGetMeshRenderer");
                return false;
            }

            mesh = gameObject.GetComponents<MeshRenderer>();
            if (mesh == null)
                return false;
            else
                return true;
        }

        private static bool TryGetChildObjects(GameObject parent, out List<GameObject> allObjects)
        {
            allObjects = null;
            if (parent == null)
            {
                Plugin.WARNING($"parent object is NULL at TryGetChildObjects");
                return false;
            }

            if (parent.transform.childCount == 0)
                return false;

            allObjects = [];
            for(int i = 0; i < parent.transform.childCount; i++)
            {
                allObjects.Add(parent.transform.GetChild(i).gameObject);
                //Plugin.Spam($"Object found through child transform - {parent.transform.GetChild(i).gameObject.name}");
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
