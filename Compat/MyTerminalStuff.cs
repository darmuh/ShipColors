
namespace ShipColors.Compat
{
    internal class MyTerminalStuff
    {
        internal static bool AddTerminalConfigs()
        {
            if (!Plugin.instance.darmuhsTerminalStuff)
                return true;

            if (TerminalStuff.ConfigSettings.TerminalCustomization.Value)
                return false;
            else
                return true;
        }
    }
}
