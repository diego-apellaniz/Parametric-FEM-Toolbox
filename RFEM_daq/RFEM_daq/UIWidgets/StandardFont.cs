using Grasshopper.Kernel;
using System.Drawing;

namespace RFEM_daq.UIWidgets
{
    public class StandardFont
    {
        public static Font font()
        {
            return GH_FontServer.StandardAdjusted;
        }

        public static Font largeFont()
        {
            return GH_FontServer.LargeAdjusted;
        }
    }
}
