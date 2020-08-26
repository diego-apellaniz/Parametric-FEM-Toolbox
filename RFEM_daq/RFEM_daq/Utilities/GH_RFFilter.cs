using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Grasshopper.Kernel.Types;

namespace RFEM_daq.Utilities
{
    // Custom Grasshopper Data Type GH_Filter. It wraps the Filter Objects so they can be used in Grasshopper and 
    // stored into the Grasshopper Parameter "Param_Filter".
    public class GH_RFFilter : GH_Goo<RFFilter>
    {
        // Default Constructor, sets the state to Unknown.
        public GH_RFFilter()
        {
        }

        // Constructor with initial value
        public GH_RFFilter(RFFilter rFFilter)
            : base(rFFilter)
        {
        }

        // Copy Constructor
        public GH_RFFilter(GH_RFFilter other)
            : base(other.Value)
        {
        }

        // Duplication method (technically not a constructor)
        public override IGH_Goo Duplicate()
        {
            return new GH_RFFilter(this);
        }

        // RFEM objects are always valid
        public override bool IsValid
        {
            get { return true; }
        }

        // Return a string with the name of this Type.
        public override string TypeName
        {
            get
            {
                return "Filter";
            }
        }

        // Return a string describing what this Type is about.
        public override string TypeDescription
        {
            get { return "Filter Object"; }
        }

        // Return a string representation of the state (value) of this instance.
        public override string ToString()
        {
            return (base.Value.ToString());
        }

        // Serialize this instance to a Grasshopper writer object.
        //Serialization is required for saving persistent data into a .gh or .ghx file.
        public override bool Write(GH_IO.Serialization.GH_IWriter writer)
        {
            IFormatter formatter = RFSerializer.GetInstance().Formatter;
            MemoryStream memoryStream = new MemoryStream();
            try
            {
                formatter.Serialize(memoryStream, Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            string text = Convert.ToBase64String(memoryStream.ToArray());
            writer.SetString("Object_bin", text);
            return base.Write(writer);
        }

        // Deserialize this instance from a Grasshopper reader object.
        //Serialization is required for reading persistent data from a .gh or .ghx file.
        public override bool Read(GH_IO.Serialization.GH_IReader reader)
        {
            string s = "";
            if (reader.TryGetString("Object_bin", ref s))
            {
                IFormatter formatter = RFSerializer.GetInstance().Formatter;
                MemoryStream serializationStream = new MemoryStream(Convert.FromBase64String(s));
                try
                {
                    serializationStream.Position = 0;
                    Value = ((RFFilter)formatter.Deserialize(serializationStream));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return base.Read(reader);
        }
    }
}
