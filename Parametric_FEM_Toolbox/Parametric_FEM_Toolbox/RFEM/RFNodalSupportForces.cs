using System;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;
using System.Collections.Generic;
using System.Linq;

namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFNodalSupportForces : IGrassRFEM
    {
        //Standard constructors
        public RFNodalSupportForces()
        {
        }
        public RFNodalSupportForces(NodalSupportForces support_forces, IModelData data) // rfem object as array because they are related to the same member
        {
            NodeNo = support_forces.NodeNo;
            Forces = new Vector3d(support_forces.Forces.ToPoint3d());
            Forces /= 1000; // N to kN
            Moments = new Vector3d(support_forces.Moments.ToPoint3d());
            Moments /= 1000; // N to kN
            Location = data.GetNode(NodeNo, ItemAt.AtNo).GetData().ToPoint3d(data);
              //  , x.ToPoint3d(data))
            Type = support_forces.Type.ToString();
            ToModify = false;
            ToDelete = false;
        }

        // Properties to Wrap Fields from RFEM Struct
        public int NodeNo { get; set; }
        public string Type { get; set; }
        //public int CrossSectionNo { get; set; } // makes no sense?
        public Vector3d Forces { get; set; }
        public Vector3d Moments { get; set; }
        public Point3d Location { get; set; }

        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-SupportForces;NodeNo:{NodeNo};");
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
