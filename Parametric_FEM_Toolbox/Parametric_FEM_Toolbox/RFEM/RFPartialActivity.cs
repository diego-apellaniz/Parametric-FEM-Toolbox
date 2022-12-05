using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFPartialActivity : IGrassRFEM
    {
          
        //Standard constructors
        public RFPartialActivity()
        {
        }
        public RFPartialActivity(PartialActivity activity)
        {

            NegativeZone = activity.NegativeZone;
            NegativeLimit = activity.NegativeLimit;
            NegativeSlippage = activity.NegativeSlippage;
            PositiveZone = activity.PositiveZone;
            PositiveLimit = activity.PositiveLimit;
            PositiveSlippage = activity.PositiveSlippage;
        }

        public RFPartialActivity(RFPartialActivity other) : this((PartialActivity)other)
        {
        }

        //Properties to Wrap Fields from RFEM Struct      
        public PartialActivityType NegativeZone { get; set; }
        public PartialActivityType PositiveZone { get; set; }
        public double NegativeLimit { get; set; }
        public double PositiveLimit { get; set; }
        public double NegativeSlippage { get; set; }
        public double PositiveSlippage { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }


        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-PartialActivity;");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator PartialActivity(RFPartialActivity activity)
        {
            PartialActivity myActivity = new PartialActivity
            {
                NegativeZone = activity.NegativeZone,
                PositiveZone = activity.PositiveZone,
                PositiveLimit = activity.PositiveLimit,
                PositiveSlippage= activity.PositiveSlippage,
                NegativeLimit = activity.NegativeLimit,
                NegativeSlippage= activity.NegativeSlippage
            };
            return myActivity;
        }

        // Convert RFEM Object into Rhino Geometry.
        // These methods are later implemented by the class GH_RFEM.
        public bool ToGH_Integer<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Point<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Line<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Curve<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Surface<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Brep<T>(ref T target)
        {
            return false;
        }
        public bool ToGH_Plane<T>(ref T target)
        {
            return false;
        }



    }
}
