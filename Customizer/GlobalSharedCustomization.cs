using BepInEx.Configuration;
using ShipColors.ConfigManager;
using static OpenLib.Common.Misc;

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace ShipColors.Customizer
{
    internal class GlobalSharedCustomization
    {
        internal static void UseSharedTextures()
        {
            LogColorBeforeChange(StartOfRound.Instance.mapScreen.offScreenMat.color, ConfigSettings.Mat_ScreenOff);
            LogColorBeforeChange(StartOfRound.Instance.mapScreen.mesh.sharedMaterial.color, ConfigSettings.Mat_TerminalTex);

            StartOfRound.Instance.mapScreen.offScreenMat.color = HexToColor(ConfigSettings.Mat_ScreenOff.Value);
            Plugin.Spam("Mat_ScreenOff set");
            StartOfRound.Instance.mapScreen.mesh.sharedMaterial.color = HexToColor(ConfigSettings.Mat_TerminalTex.Value);
            Plugin.Spam("Mat_TerminalTex set");
            ChargingStation();
            ControlPanels();
            ShipInside();
            Bunkbeds();
            LockerCabinet();
            HangarDoorStuff();

        }

        private static void ShipInside()
        {
            if(TryGetMeshRenderer("Environment/HangarShip/ShipInside", out MeshRenderer shipInside))
            {
                Plugin.Spam($"This should be 5 - {shipInside.sharedMaterials.Length}");
                LogColorBeforeChange(shipInside.sharedMaterials[0].color, ConfigSettings.Mat_ShipHull);
                LogColorBeforeChange(shipInside.sharedMaterials[1].color, ConfigSettings.Mat_ShipRoomMetal);
                LogColorBeforeChange(shipInside.sharedMaterials[2].color, ConfigSettings.Mat_ShipFloor);
                shipInside.sharedMaterials[0].color = HexToColor(ConfigSettings.Mat_ShipHull.Value); //shiphull
                shipInside.sharedMaterials[1].color = HexToColor(ConfigSettings.Mat_ShipRoomMetal.Value); //shiproommetal
                shipInside.sharedMaterials[2].color = HexToColor(ConfigSettings.Mat_ShipFloor.Value); //shipfloor
            }
        }

        private static void Bunkbeds()
        {
            //also affects file cabinet
            SetSharedMaterial("Environment/HangarShip/Bunkbeds", ConfigSettings.Mat_BunkBeds);
        }

        private static void LockerCabinet()
        {
            //StorageCloset
            SetSharedMaterial("Environment/HangarShip/StorageCloset", ConfigSettings.Mat_LockerCabinet);
            SetSharedMaterial("Environment/HangarShip/StorageCloset/Cube.000", ConfigSettings.Mat_LockerCabinet);
        }

        private static void HangarDoorStuff()
        {
            SetSharedMaterial("Environment/HangarShip/DoorGenerator", ConfigSettings.Mat_DoorGenerator);
            SetSharedMaterial("Environment/HangarShip/AnimatedShipDoor/HangarDoorButtonPanel", ConfigSettings.Mat_DoorControlPanel);

            if (TryGetMeshRenderer("Environment/HangarShip/AnimatedShipDoor/HangarDoorLeft (1)", out MeshRenderer shipDoors))
            {
                Plugin.Spam($"This should be 2 - {shipDoors.sharedMaterials.Length}");
                LogColorBeforeChange(shipDoors.sharedMaterials[0].color, ConfigSettings.Mat_ShipDoors);
                LogColorBeforeChange(shipDoors.sharedMaterials[1].color, ConfigSettings.Mat_ShipDoors2);

                shipDoors.sharedMaterials[0].color = HexToColor(ConfigSettings.Mat_ShipDoors.Value); //shipdoors1
                shipDoors.sharedMaterials[1].color = HexToColor(ConfigSettings.Mat_ShipDoors2.Value); //ShipDoors2
            }

            //doorgenerator = Environment/HangarShip/DoorGenerator
            //doorbuttonspanel = Environment/HangarShip/AnimatedShipDoor/HangarDoorButtonPanel
            //hangerdoors = Environment/HangarShip/AnimatedShipDoor/HangarDoorLeft (1)
        }

        private static void SetSharedMaterial(string FindObject, ConfigEntry<string> setting)
        {
            if(TryGetMeshRenderer(FindObject, out MeshRenderer materialToChange))
            {
                LogColorBeforeChange(materialToChange.sharedMaterial.color, setting);
                materialToChange.sharedMaterial.color = HexToColor(setting.Value);
                Plugin.Spam($"{setting.Definition.Key} has been set");
            }
        }

        private static void ControlPanels()
        {
            SetSharedMaterial("Environment/HangarShip/ControlDesk", ConfigSettings.Mat_DeskBottom);
            SetSharedMaterial("Environment/HangarShip/ControlPanelWTexture", ConfigSettings.Mat_ControlPanel);
        }

        private static void ChargingStation()
        {
            if (TryGetMeshRenderer("Environment/HangarShip/ShipModels2b/ChargeStation", out MeshRenderer charger))
            {
                Plugin.Spam($"This should be 7 - {charger.sharedMaterials.Length}");
                LogColorBeforeChange(charger.sharedMaterials[0].color, ConfigSettings.Mat_Charger);
                LogColorBeforeChange(charger.sharedMaterials[1].color, ConfigSettings.Mat_DarkSteel);
                LogColorBeforeChange(charger.sharedMaterials[2].color, ConfigSettings.Mat_ElevatorSteel);
                LogColorBeforeChange(charger.sharedMaterials[3].color, ConfigSettings.Mat_BlackRubber);


                charger.sharedMaterials[0].color = HexToColor(ConfigSettings.Mat_Charger.Value);
                charger.sharedMaterials[1].color = HexToColor(ConfigSettings.Mat_DarkSteel.Value);
                charger.sharedMaterials[2].color = HexToColor(ConfigSettings.Mat_ElevatorSteel.Value);
                charger.sharedMaterials[3].color = HexToColor(ConfigSettings.Mat_BlackRubber.Value);
                Plugin.Spam("Charger, Darksteel, ElevatorSteel, and BlackRubber materials set.");
            }
        }

        private static bool TryGetMeshRenderer(string GameObjectFind, out MeshRenderer mesh)
        {
            if (GameObject.Find(GameObjectFind) == null)
            {
                mesh = null;
                Plugin.WARNING($"Unable to find object at path {GameObjectFind}");
                return false;
            }

            mesh = GameObject.Find(GameObjectFind).GetComponent<MeshRenderer>();
            if (mesh == null)
                return false;
            else
                return true;
        }

        private static void GetAllMatchingMeshRenderers(Material query)
        {
            List<MeshRenderer> allMeshRenderers = [.. Resources.FindObjectsOfTypeAll<MeshRenderer>()];
            Plugin.Spam($"got all meshrenderers - {allMeshRenderers.Count}");

            for (int i = 0; i < allMeshRenderers.Count; i++)
            {
                if (allMeshRenderers[i] == null)
                {
                    Plugin.Spam("meshRenderer is NULL");
                    continue;
                }
                else if (allMeshRenderers[i].materials == null)
                {
                    Plugin.Spam("meshRenderer is NULL");
                    continue;
                }
                else if (allMeshRenderers[i].materials.Contains(query))
                {
                    Plugin.Spam($"material query found at {allMeshRenderers[i].gameObject.name}");
                    Plugin.Spam($"path: {allMeshRenderers[i].gameObject.scene.path}");
                }
            }
        }
    }
}
