using ShipColors.ConfigManager;
using static OpenLib.Common.Misc;
using UnityEngine;
using UnityEngine.UI;

namespace ShipColors.Customizer
{
    internal class TerminalCustomizations
    {
        internal static void TerminalStuff()
        {
            if (Plugin.instance.darmuhsTerminalStuff || !ConfigSettings.TerminalCustomization.Value)
                return;

            if (Plugin.instance.Terminal == null)
            {
                Plugin.ERROR("FATAL ERROR: Terminal is NULL");
                return;
            }

            TerminalCustomization();
        }

        private static void TerminalBodyColors()
        {
            if (!ConfigSettings.TerminalCustomization.Value || Plugin.instance.darmuhsTerminalStuff)
                return;

            MeshRenderer termMesh = GameObject.Find("Environment/HangarShip/Terminal").GetComponent<MeshRenderer>();

            if (termMesh != null)
            {
                if (termMesh.materials.Length <= 3)
                {
                    termMesh.materials[0].color = HexToColor(ConfigSettings.TerminalColor.Value); //body
                    termMesh.materials[1].color = HexToColor(ConfigSettings.TerminalButtonsColor.Value); //glass buttons
                                                                                                                        //2 = warning sticker
                }
                else
                {
                    Plugin.WARNING("termMesh does not have expected number of materials, only setting terminal body color");
                    termMesh.material.color = HexToColor(ConfigSettings.TerminalColor.Value);
                }
            }
            else
                Plugin.WARNING("customization failure: termMesh is null");
        }

        private static void TerminalKeyboardColors()
        {
            if (!ConfigSettings.TerminalCustomization.Value || Plugin.instance.darmuhsTerminalStuff)
                return;

            MeshRenderer kbMesh = GameObject.Find("Environment/HangarShip/Terminal/Terminal.003").GetComponent<MeshRenderer>();

            if (kbMesh != null)
            {
                kbMesh.material.color = HexToColor(ConfigSettings.TerminalKeyboardColor.Value);
            }
            else
                Plugin.WARNING("customization failure: kbMesh is null");
        }

        internal static void TerminalCustomization()
        {
            if (!ConfigSettings.TerminalCustomization.Value || Plugin.instance.darmuhsTerminalStuff)
                return;

            TerminalBodyColors();
            TerminalKeyboardColors();

            Color moneyBG = HexToColor(ConfigSettings.TerminalMoneyBGColor.Value);
            moneyBG.a = ConfigSettings.TerminalMoneyBGAlpha.Value;

            Plugin.instance.Terminal.screenText.textComponent.color = HexToColor(ConfigSettings.TerminalTextColor.Value);
            Plugin.instance.Terminal.topRightText.color = HexToColor(ConfigSettings.TerminalMoneyColor.Value);
            Plugin.instance.Terminal.terminalUIScreen.gameObject.transform.GetChild(0).GetChild(5).gameObject.GetComponent<Image>().color = moneyBG;
            Plugin.instance.Terminal.screenText.caretColor = HexToColor(ConfigSettings.TerminalCaretColor.Value);
            Plugin.instance.Terminal.scrollBarVertical.image.color = HexToColor(ConfigSettings.TerminalScrollbarColor.Value);
            Plugin.instance.Terminal.scrollBarVertical.gameObject.GetComponent<Image>().color = HexToColor(ConfigSettings.TerminalScrollBGColor.Value);
            Plugin.instance.Terminal.terminalLight.color = HexToColor(ConfigSettings.TerminalLightColor.Value);

            Image bgImage = GameObject.Find("Environment/HangarShip/Terminal/Canvas/MainContainer/Scroll View/Viewport/InputField (TMP)").GetComponent<Image>();

            if (bgImage != null)
            {
                bgImage.enabled = ConfigSettings.TerminalCustomBG.Value;
                Color newColor = HexToColor(ConfigSettings.TerminalCustomBGColor.Value);
                newColor.a = ConfigSettings.TerminalCustomBGAlpha.Value;
                bgImage.color = newColor;
            }
        }
    }
}
