using System;
using System.Drawing;
using Grasshopper.Kernel;

namespace daqRFEM
{
    public class RFEM_daqInfo : GH_AssemblyInfo
    {
        public override string Name
        {
            get
            {
                return "daqRFEM";
            }
        }
        public override Bitmap Icon
        {
            get
            {
                //Return a 24x24 pixel bitmap to represent this GHA library.
                return null;
            }
        }
        public override string Description
        {
            get
            {
                //Return a short string describing the purpose of this GHA library.
                return "This Plug-In enables a functional interoperability between the FEM software RFEM and" +
                    "Grasshopper. Thus, the full potential of Grasshopper can be used to manipulate existing" +
                    "RFEM - Models and get data from them.";
            }
        }
        public override Guid Id
        {
            get
            {
                return new Guid("2b7caeb3-80be-448b-a3bf-d4b393f7edac");
            }
        }

        public override string AuthorName
        {
            get
            {
                //Return a string identifying you or your company.
                return "Diego Apellániz";
            }
        }
        public override string AuthorContact
        {
            get
            {
                //Return a string representing your preferred contact details.
                return "dapellanizq@gmail.com";
            }
        }
        public override string AssemblyVersion
        {
            get
            {
                return "0.1";
            }
        }
    }
}
