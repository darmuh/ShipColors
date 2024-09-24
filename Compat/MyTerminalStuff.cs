using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
