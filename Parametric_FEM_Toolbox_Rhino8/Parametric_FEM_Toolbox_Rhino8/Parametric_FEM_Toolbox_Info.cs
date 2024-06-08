using Grasshopper;
using Grasshopper.Kernel;
using System;
using System.Drawing;

namespace Parametric_FEM_Toolbox_Rhino8
{
    public class Parametric_FEM_Toolbox_Info : GH_AssemblyInfo
    {
        public override string Name => "Parametric_FEM_Toolbox";

        //Return a 24x24 pixel bitmap to represent this GHA library.
        public override Bitmap Icon => Parametric_FEM_Toolbox.Properties.Resources.toolbox_vrey_small_Rainbow;

        //Return a short string describing the purpose of this GHA library.
        public override string Description => "This Plug-In enables a functional interoperability between the FEM software RFEM and " +
                    "Grasshopper. Thus, the full potential of Grasshopper can be used to manipulate existing" +
                    " RFEM - Models and get data from them.";

        public override Guid Id => new Guid("2b7caeb3-80be-448b-a3bf-d4b393f7edac");

        //Return a string identifying you or your company.
        public override string AuthorName => "Diego Apellániz";

        //Return a string representing your preferred contact details.
        public override string AuthorContact => "dapellanizq@gmail.com";

        public override string AssemblyVersion
        {
            get
            {
                return "1.4.6"; //
            }
        }
    }
}