using static OpenLib.ConfigManager.ConfigSetup;
using static OpenLib.Common.Misc;
using static ShipColors.ConfigManager.GeneratedConfig;
using BepInEx.Configuration;
using UnityEngine;
using System.Collections.Generic;
using ShipColors.ConfigManager;
using Color = UnityEngine.Color;
using System.Linq;

namespace ShipColors.Customizer
{
    internal class GeneratedCustomization
    {
        internal static List<CustomColorClass> materialToColor = [];
        internal static List<GameObject> ObjectsWithConfigItems = [];
        internal static List<GameObject> DoNotTouchList = [];
        internal static bool configGenerated = false;
        internal static void CreateAllConfigs()
        {
            if (configGenerated)
                return;

            //materialToColor.RemoveAll(x => x.gameObj == null);
            ObjectsWithConfigItems.RemoveAll(x => x.gameObject == null);

            List<int> acceptableLayers = OpenLib.Common.CommonStringStuff.GetNumberListFromStringList(OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenAcceptedLayers.Value, ','));
            List<string> bannedObjects = OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenBannedObjects.Value, ',');
            List<string> bannedMaterials = OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenBannedMaterials.Value, ',');
            List<string> permitListObjects = OpenLib.Common.CommonStringStuff.GetKeywordsPerConfigItem(ConfigSettings.GenPermitListObjects.Value, ',');

            bool addTerminal = true;
            if(Plugin.instance.darmuhsTerminalStuff)
                addTerminal = Compat.MyTerminalStuff.AddTerminalConfigs();

            GameObject topLevel = GameObject.Find("Environment/HangarShip");
            GameObject shipModels2b = GameObject.Find("Environment/HangarShip/ShipModels2b");
            if (topLevel == null)
            {
                Plugin.ERROR("Unable to find root ship object @ Environment/HangarShip");
                return;
            }

            if (TryGetChildObjects(topLevel, out List<GameObject> allObjects, shipModels2b))
            {
                Plugin.Spam($"allObjects Count - {allObjects.Count}");

                allObjects = [.. allObjects.OrderBy(x => x.name)];
                foreach (GameObject gameObject in allObjects)
                {
                    if (allObjects.Any(p => (p.transform == gameObject.transform.parent)))
                        continue; //if any object in the listing is this object's parent, skip

                    if (!IsValidObject(gameObject, bannedObjects, acceptableLayers, permitListObjects))
                        continue;

                    if (!addTerminal && gameObject.name.ToLower() == "terminal")
                        continue; //skip terminal, covered by darmuhsTerminalStuff

                    ProcessObjectFamily(gameObject, bannedMaterials, bannedObjects, acceptableLayers, permitListObjects);
                }
            }
            else
                Plugin.WARNING("Unable to get any objects!");

            if(OpenLib.Plugin.instance.LethalConfig)
                Compat.LethalConfigStuff.AddConfig(Generated);

            Plugin.Spam("Config has been generated");
            configGenerated = true;
        }

        internal static void ProcessObjectFamily(GameObject gameObject, List<string> bannedMaterials, List<string> bannedObjects, List<int> acceptableLayers, List<string> permitListObjects, string customSection = "", string customName = "")
        {
            if (!IsValidObject(gameObject, bannedObjects, acceptableLayers, permitListObjects))
                return;

            if (TryGetFamily(gameObject, out List<GameObject> familyTree))
            {
                //grandparent detected
                if (familyTree.Count >= 2)
                {
                    //Plugin.Spam($"sorting through {gameObject.name} familyTree");
                    int loopCount = 0;
                    //set i to the last object in the familytree
                    for (int i = familyTree.Count - 1; i > 0; i--)
                    {
                        //Plugin.Spam($"Loop# {loopCount}");
                        loopCount++;
                        int child = i;
                        int parent = child - 1;

                        //Plugin.Spam($"child: {child}\nparent: {parent}\ngrandparent: {grandparent}");
                        if (familyTree[child] == null)
                            continue;

                        if (parent >= 0)
                        {
                            //child next loop
                            ProcessObject(familyTree[child], bannedMaterials, bannedObjects, acceptableLayers, permitListObjects, customSection, customName, familyTree[parent], familyTree[0]);
                        }
                        else
                        {
                            //no more parents (toplevel)
                            ProcessObject(familyTree[child], bannedMaterials, bannedObjects, acceptableLayers, permitListObjects, customSection, customName, familyTree[0]);
                        }
                    }
                }
                else
                {
                    if (familyTree.Count == 1)
                        ProcessObject(familyTree[0], bannedMaterials, bannedObjects, acceptableLayers, permitListObjects, customSection, customName, familyTree[0]);
                    else
                        Plugin.WARNING("Unable to process familyTree??");
                }
            }
            else
            {
                Plugin.Spam($"{gameObject.name} has no familyTree, processing on it's own");
                ProcessObject(gameObject, bannedMaterials, bannedObjects, acceptableLayers, permitListObjects, customSection, customName);
            }
        }

        internal static bool ObjectComponentsAllowed(GameObject gameObject)
        {
            if (ConfigSettings.GenAcceptItems.Value && ConfigSettings.GenAcceptScrap.Value)
                return true;

            GrabbableObject item = gameObject.GetComponentInChildren<GrabbableObject>();

            if (item == null)
                return true;

            if (item.itemProperties.isScrap)
            {
                if (!ConfigSettings.GenAcceptScrap.Value)
                    return false;
                else
                    return true;
            }
            else
            {
                if (!ConfigSettings.GenAcceptItems.Value)
                    return false;
            }

            return true;

        }

        private static bool IsValidObject(GameObject gameObject, List<string> bannedObjects, List<int> acceptableLayers, List<string> permitListObjects)
        {
            if (bannedObjects.Any(x => gameObject.name.ToLower().Contains(x.ToLower())))
            {
                if(permitListObjects.Any(y => gameObject.name.ToLower() == y.ToLower()))
                    Plugin.Spam($"{gameObject.name} is explicitly permitted!");
                else
                {
                    Plugin.Spam($"{gameObject.name} is detected as a banned object!");
                    return false;
                }   
            }
                
            if (!acceptableLayers.Contains(gameObject.layer))
            {
                Plugin.Spam($"{gameObject.name} is detected as having a banned layer!");
                return false;
            }    

            if (ObjectsWithConfigItems.Contains(gameObject))
            {
                Plugin.Spam($"{gameObject.name} is detected as already having a config item!");
                return false;
            }

            if (DoNotTouchList.Contains(gameObject))
            {
                Plugin.WARNING($"{gameObject.name} has been designated as a banned object via the ShipColors API!");
                return false;
            }

            if (!ObjectComponentsAllowed(gameObject))
                return false;
                
            return true;
        }

        internal static void ProcessObject(GameObject gameObject, List<string> bannedMaterials, List<string> bannedObjects, List<int> acceptableLayers, List<string> permitListObjects, string customSection = "", string customName = "", GameObject parentObj = null, GameObject grandParent = null)
        {
            if(gameObject == null)
            {
                Plugin.ERROR("Provided NULL object at ProcessObject");
                return;
            }

            if (!IsValidObject(gameObject, bannedObjects, acceptableLayers, permitListObjects))
                return;

            //direct parent config items
            if (TryGetMeshRenderers(gameObject, out MeshRenderer[] mesh))
            {
                MakeConfigItems(bannedMaterials, mesh, gameObject, parentObj, grandParent, customSection, customName);
                ObjectsWithConfigItems.Add(gameObject);
            }
            else
                Plugin.Spam($"{gameObject.name} has no MeshRenderers!");
        }

        private static void MakeConfigItems(List<string> bannedMaterials, MeshRenderer[] meshes, GameObject gameObject, GameObject parent = null, GameObject grandparent = null, string customSection = "", string customName = "")
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
                    if (bannedMaterials.Any(x => material.name.ToLower().Contains(x.ToLower())))
                    {
                        Plugin.Spam($"{material.name} is banned!");
                        continue;
                    }

                    ConfigEntry<string> colorEntry;
                    ConfigEntry<float> colorFloatEntry = null!;
                    string color = ColorUtility.ToHtmlStringRGB(material.color);
                    if(parent != null && grandparent != null)
                    {
                        colorEntry = MakeString(Generated, $"{grandparent.name} Colors", $"{parent.name}/{gameObject.name}/{material.name} Color", "#" + color, $"Change color of material, {material.name} as part of object {gameObject.name}");
                        if(material.color.a < 1)
                            colorFloatEntry = MakeClampedFloat(Generated, $"{grandparent.name} Colors", $"{parent.name}/{gameObject.name}/{material.name} Alpha", material.color.a, $"Change alpha of material, {material.name} as part of object {gameObject.name} in {grandparent.name}", 0, 1);
                    }
                    else if(parent != null && grandparent == null)
                    {
                        colorEntry = MakeString(Generated, $"{parent.name} Colors", $"{gameObject.name}/{material.name} Color", "#" + color, $"Change color of material, {material.name} as part of object {gameObject.name}");
                        if (material.color.a < 1)
                            colorFloatEntry = MakeClampedFloat(Generated, $"{parent.name} Colors", $"{gameObject.name}/{material.name} Alpha", material.color.a, $"Change alpha of material, {material.name} as part of object {gameObject.name}", 0, 1);
                    }
                    else
                    {
                        string sectionName = $"{gameObject.name} Colors";
                        string itemName = $"{material.name}";

                        if (customSection.Length > 0)
                            sectionName = customSection;

                        if (customName.Length > 0)
                        {
                            itemName = $"{customName}";

                            if (meshRenderer.materials.Length > 1)
                                itemName = $"{customName} {material.name}";
                        }

                        colorEntry = MakeString(Generated, sectionName, itemName + " Color", "#" + color, $"Change color of material, {material.name} as part of object {gameObject.name}");
                        if (material.color.a < 1)
                            colorFloatEntry = MakeClampedFloat(Generated, sectionName, itemName + " Alpha", material.color.a, $"Change alpha of material, {material.name} as part of object {gameObject.name}", 0, 1);
                    }
                    
                    Color newColor = HexToColor(colorEntry.Value);
                    if(colorFloatEntry != null)
                        newColor.a = colorFloatEntry.Value;
                    material.color = newColor;
                    //Plugin.Spam($"Config item created for {material.name} in Section: {gameObject.name}");
                    //Plugin.Spam($"{material.name} set to color - {colorEntry.Value} with alpha {colorFloatEntry.Value}");
                    CustomColorClass CustomColorClass = new(colorEntry, colorFloatEntry, material, gameObject);
                    materialToColor.Add(CustomColorClass);
                }
            }
        }

        internal static void ReadCustomClassValues(ref List<CustomColorClass> config)
        {
            foreach(CustomColorClass item in config)
            {
                Color newColor = HexToColor(item.colorConfig.Value);
                if(item.alphaConfig != null)
                    newColor.a = item.alphaConfig.Value;
                item.material.color = newColor;
                Plugin.Spam($"set color for {item.material.color} to {item.colorConfig.Value}");
                if(item.alphaConfig != null)
                    Plugin.Spam($"with alpha {item.alphaConfig.Value}");
            }
        }

        internal static void UpdateGeneratedValue(ConfigEntry<string> valueUpdated)
        {
            Plugin.Spam($"Attempting to update value: {valueUpdated.Definition.Key}");
            CustomColorClass newConfig = materialToColor.Find(x => x.colorConfig == valueUpdated);

            if(newConfig != null)
            {
                if (newConfig.material == null)
                {
                    Plugin.WARNING("Unable to update null material!");
                    return;
                }

                Color newColor = HexToColor(valueUpdated.Value);
                if (newConfig.alphaConfig != null)
                    newColor.a = newConfig.alphaConfig.Value;
                newConfig.material.color = newColor;
                Plugin.Spam($"{newConfig.material.name} updated to {valueUpdated.Value}");
            }
            else            
                Plugin.WARNING($"Could not find {valueUpdated.Definition.Key} in material listing ({materialToColor.Count})");
        }

        internal static void UpdateGeneratedValue(ConfigEntry<float> valueUpdated)
        {
            Plugin.Spam($"Attempting to update value: {valueUpdated.Definition.Key}");

            CustomColorClass newConfig = materialToColor.Find(x => x.alphaConfig == valueUpdated);
            if(newConfig != null)
            {
                Color newColor = HexToColor(newConfig.colorConfig.Value);
                newColor.a = valueUpdated.Value;
                newConfig.material.color = newColor;
                Plugin.Spam($"{newConfig.material.name} updated to {valueUpdated.Value}");
            }
            else
                Plugin.WARNING($"Could not find {valueUpdated.Definition.Key} in material listing ({materialToColor.Count})");
            
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

        private static bool TryGetChildObjects(GameObject parent, out List<GameObject> allObjects, GameObject parent2 = null)
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
                if (parent2 != null)
                    if (parent2 == parent.transform.GetChild(i).gameObject)
                        continue;

                allObjects.Add(parent.transform.GetChild(i).gameObject);
                //Plugin.Spam($"Object found through child transform - {parent.transform.GetChild(i).gameObject.name}");
            }

            if(parent2 != null)
            {
                if(parent2.transform.childCount > 0)
                {
                    for (int i = 0; i < parent2.transform.childCount; i++)
                    {
                        allObjects.Add(parent2.transform.GetChild(i).gameObject);
                        //Plugin.Spam($"Object found through child transform - {parent.transform.GetChild(i).gameObject.name}");
                    }
                }
            }

            return true;

        }

        private static bool TryGetFamily(GameObject gameObject, out List<GameObject> familyTree)
        {
            familyTree = [];
            if(gameObject == null)
            {
                Plugin.WARNING($"A provided gameobject is NULL at TryGetFamily");
                return false;
            }

            if(gameObject.transform.childCount == 0)
                return false;

            List<Transform> allChildren = [.. gameObject.GetComponentsInChildren<Transform>()];
            familyTree.Add(gameObject);

            foreach(Transform child in allChildren)
            {
                if(child.gameObject.GetComponent<MeshRenderer>() != null && child.gameObject != null) //only add objects that have meshrenderer components
                    familyTree.Add(child.gameObject);
            }

            Plugin.Spam($"TryGetFamily got familyTree tree for {gameObject.name} with [ {familyTree.Count} ] members");

            return true;
        }
    }
    
    internal class CustomColorClass
    {
        internal ConfigEntry<string> colorConfig;
        internal ConfigEntry<float> alphaConfig;
        internal GameObject gameObj;
        internal Material material;

        internal CustomColorClass(ConfigEntry<string> stringEntry, ConfigEntry<float> floatEntry, Material mat, GameObject obj)
        {
            colorConfig = stringEntry;
            alphaConfig = floatEntry;
            material = mat;
            gameObj = obj;
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
