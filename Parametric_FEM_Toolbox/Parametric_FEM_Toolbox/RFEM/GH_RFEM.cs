using System;
using System.Runtime.Serialization;
using System.IO;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.Utilities;
using System.Windows.Forms;
using Parametric_FEM_Toolbox.GUI;

namespace Parametric_FEM_Toolbox.RFEM
{
    // Custom Grasshopper Data Type GH_RFEM. It wraps the RFEM Objects so they can be used in Grasshopper and 
    // stored into the Grasshopper Parameter "Param_RFEM".
    public class GH_RFEM : GH_Goo<IGrassRFEM>
    {
        // Default Constructor, sets the state to Unknown.
        public GH_RFEM()
        {
        }

        // Constructor with initial value
        public GH_RFEM(IGrassRFEM RFobject)
            : base(RFobject)
        {
        }

        // Copy Constructor
        public GH_RFEM(GH_RFEM other)
            : base(other.Value)           
        {
        }

        // Duplication method (technically not a constructor)
        public override IGH_Goo Duplicate()
        {
            return new GH_RFEM(this);
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
                if (!(base.Value is null))
                {
                    return Value.GetType().Name;
                }
                else
                {
                    return "RFEM";
                }
            }
        }

        // Return a string describing what this Type is about.
        public override string TypeDescription
        {
            get { return "RFEM Object"; }
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
                    Value = ((IGrassRFEM)formatter.Deserialize(serializationStream));
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return base.Read(reader);
        }

        // This function is called when Grasshopper needs to convert 
        // an RFEM Object into Rhino Geometry.
        public override bool CastTo<Q>(ref Q target)
        {
            // GH_Point
            if (target is GH_Point)
            {
                return Value.ToGH_Point(ref target);
            }
            //GH_Line
            else if (target is GH_Line)
            {
                return Value.ToGH_Line(ref target);
            }
            // GH_Curve
            else if (target is GH_Curve)
            {
                return Value.ToGH_Curve(ref target);
            }
            // GH_Surface
            else if (target is GH_Surface)
            {
                return Value.ToGH_Surface(ref target);
            }
            else if (target is GH_Brep)
            {
                return Value.ToGH_Brep(ref target);
            }
            // GH_Plane
            else if (target is GH_Plane)
            {
                return Value.ToGH_Plane(ref target);
            }
            // Integer
            else if (target is GH_Integer)
            {
                return Value.ToGH_Integer(ref target);
            }
            //In any other case return false.
            return false;
        }
    }
}