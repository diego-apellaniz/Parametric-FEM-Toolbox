using System;
using System.Collections.Generic;
using System.Drawing;
using Properties;
using Grasshopper.Kernel;
using Parametric_FEM_Toolbox.Utilities;

namespace Parametric_FEM_Toolbox.GUI
{
    // Class to define the Grasshopper Parameter "Param_RFEM".
    // This is the output parameter that the RFEM Objects will use in Grasshopper.
    public class Param_Filter : GH_PersistentParam<GH_RFFilter>
    {
        // We need to supply a constructor without arguments that calls the base class constructor.
        public Param_Filter() :
        base("RF Filter", "RF Filter", "Filter for RFEM Objects", "B+G Toolbox", "RFEM")
        { }

        public override IEnumerable<string> Keywords => new string[] { "rf", "filter" };

        // Always generate a new Guid, but never change it once
        // you've released this parameter to the public.
        public override Guid ComponentGuid => new Guid("{014ad5f9-70c3-4572-bfdc-eba2cdcb9fc3}");
        

          protected override GH_GetterResult Prompt_Plural(ref List<GH_RFFilter> values)
        {
            return GH_GetterResult.success;
        }

        protected override GH_GetterResult Prompt_Singular(ref GH_RFFilter value)
        {
            return GH_GetterResult.success;
        }

        public override GH_Exposure Exposure => GH_Exposure.primary;

        protected override Bitmap Icon => Resources.icon_RFEM_Filter_Param2;

        protected override GH_RFFilter PreferredCast(object data)
        {
            if (data is RFFilter)
            {
                return new GH_RFFilter((RFFilter)data);
            }
            throw new Exception("Cast to RFFilter impossible.");
        }
    }
}
