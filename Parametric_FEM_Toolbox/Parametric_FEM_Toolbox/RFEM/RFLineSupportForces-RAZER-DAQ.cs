﻿using System;
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
    public class RFLineSupportForces : IGrassRFEM
    {
        //Standard constructors
        public RFLineSupportForces()
        {
        }
        public RFLineSupportForces(LineSupportForces[] support_forces) // rfem object as array because they are related to the same member
        {
            if (support_forces.Length == 0)
            {
                return;
            }
            LineNo = support_forces[0].LineNo;
            NodeNo = support_forces[0].NodeNo;
            Flag = support_forces[0].Flag;
            Forces = new List<Vector3d>();
            Moments = new List<Vector3d>();
            Location = new List<double>();
            Type = new List<string>();
            for (int i = 0; i < support_forces.Length; i++)
            {
                Forces.Add(new Vector3d(support_forces[i].Forces.ToPoint3d()/1000));
                Moments.Add(new Vector3d(support_forces[i].Moments.ToPoint3d()/1000));
                Location.Add(support_forces[i].Location);
                Type.Add(support_forces[i].Type.ToString());
            }
            ToModify = false;
            ToDelete = false;
        }

        // Properties to Wrap Fields from RFEM Struct
        public int LineNo { get; set; }
        public int NodeNo { get; set; }
        public List<string> Type { get; set; }
        //public int CrossSectionNo { get; set; } // makes no sense?
        public ResultsFlag Flag { get; set; }
        public List<Vector3d> Forces { get; set; }
        public List<Vector3d> Moments { get; set; }
        public List<double> Location { get; set; }

        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-LineSupportForces;LineNo:{LineNo};" +
                $"Flag:{Flag};Locations:{Location.Count};");
        }

        ////Operator to retrieve a Line from an rfLine.
        //public static implicit operator MemberForces(RFMemberForces member_forces)
        //{
        //    Dlubal.RFEM5.MemberForces myForces = new Dlubal.RFEM5.MemberForces

            
        //    return myForces;
        //}

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
