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
    public class RFSurfaceForces : IGrassRFEM
    {
        //Standard constructors
        public RFSurfaceForces()
        {
        }
        public RFSurfaceForces(SurfaceInternalForces[] surface_forces) // rfem object as array because they are related to the same member
        {
            if (surface_forces.Length == 0)
            {
                return;
            }
            SurfaceNo = surface_forces[0].SurfaceNo;

            Location = new List<Point3d>();
            LocationNo = new List<int>();
            Type = new List<string>();

            AxialForceAlphaM = new List<double>();
            AxialForceN1 = new List<double>();
            AxialForceN2 = new List<double>();
            AxialForceNcD = new List<double>();
            AxialForceNx = new List<double>();
            AxialForceNxD = new List<double>();
            AxialForceNxy = new List<double>();
            AxialForceNy = new List<double>();
            AxialForceNyD = new List<double>();
            AxialForceVMaxM = new List<double>();
            MomentAlphaB = new List<double>();
            MomentM1 = new List<double>();
            MomentM2 = new List<double>();
            MomentMcDNegative = new List<double>();
            MomentMcDPositive = new List<double>();
            MomentMx = new List<double>();
            MomentMxDNegative = new List<double>();
            MomentMxDPositive = new List<double>();
            MomentMxy = new List<double>();
            MomentMy = new List<double>();
            MomentMyDNegative = new List<double>();
            MomentMyDPositive = new List<double>();
            MomentTMaxB = new List<double>();
            ShearForceBetaB = new List<double>();
            ShearForceVMaxB = new List<double>();
            ShearForceVx = new List<double>();
            ShearForceVy = new List<double>();

            for (int i = 0; i < surface_forces.Length; i++)
            {
                Location.Add(surface_forces[i].Coordinates.ToPoint3d());
                LocationNo.Add(surface_forces[i].LocationNo);
                Type.Add(surface_forces[i].Type.ToString());
                AxialForceAlphaM.Add(surface_forces[i].AxialForceAlphaM / 1000);
                AxialForceN1.Add(surface_forces[i].AxialForceN1 / 1000);
                AxialForceN2.Add(surface_forces[i].AxialForceN2 / 1000);
                AxialForceNcD.Add(surface_forces[i].AxialForceNcD / 1000);
                AxialForceNx.Add(surface_forces[i].AxialForceNx / 1000);
                AxialForceNxD.Add(surface_forces[i].AxialForceNxD / 1000);
                AxialForceNxy.Add(surface_forces[i].AxialForceNxy / 1000);
                AxialForceNy.Add(surface_forces[i].AxialForceNy / 1000);
                AxialForceNyD.Add(surface_forces[i].AxialForceNyD / 1000);
                AxialForceVMaxM.Add(surface_forces[i].AxialForceVMaxM / 1000);
                MomentAlphaB.Add(surface_forces[i].MomentAlphaB / 1000);
                MomentM1.Add(surface_forces[i].MomentM1 / 1000);
                MomentM2.Add(surface_forces[i].MomentM2 / 1000);
                MomentMcDNegative.Add(surface_forces[i].MomentMcDNegative / 1000);
                MomentMcDPositive.Add(surface_forces[i].MomentMcDPositive / 1000);
                MomentMx.Add(surface_forces[i].MomentMx / 1000);
                MomentMxDNegative.Add(surface_forces[i].MomentMxDNegative / 1000);
                MomentMxDPositive.Add(surface_forces[i].MomentMxDPositive / 1000);
                MomentMxy.Add(surface_forces[i].MomentMxy / 1000);
                MomentMy.Add(surface_forces[i].MomentMy / 1000);
                MomentMyDNegative.Add(surface_forces[i].MomentMyDNegative / 1000);
                MomentMyDPositive.Add(surface_forces[i].MomentMyDPositive / 1000);
                MomentTMaxB.Add(surface_forces[i].MomentTMaxB / 1000);
                ShearForceBetaB.Add(surface_forces[i].ShearForceBetaB / 1000);
                ShearForceVMaxB.Add(surface_forces[i].ShearForceVMaxB / 1000);
                ShearForceVx.Add(surface_forces[i].ShearForceVx / 1000);
                ShearForceVy.Add(surface_forces[i].ShearForceVy / 1000);
            }

            //Axis = Plane.Unset;
            ToModify = false;
            ToDelete = false;
        }

        //public RFSurfaceForces(SurfaceInternalForces[] surface_forces, Plane local_axis) : this(surface_forces)
        //{
        //    Axis = local_axis;
        //}

        //public RFMemberForces(RFMemberForces other) : this(other)
        //{

        //    //if (other.BaseLine != null)
        //    //{
        //    //    BaseLine = new RFLine(other.BaseLine);
        //    //}            
        //    //Frames = other.Frames;
        //}

        // Properties to Wrap Fields from RFEM Struct
        public int SurfaceNo { get; set; }
        public List<string> Type { get; set; }
        public List<Point3d> Location { get; set; }
        public List<int> LocationNo { get; set; }
        public List<double> AxialForceAlphaM { get; set; }
        public List<double> AxialForceN1 { get; set; }
        public List<double> AxialForceN2 { get; set; }
        public List<double> AxialForceNcD { get; set; }
        public List<double> AxialForceNx { get; set; }
        public List<double> AxialForceNxD { get; set; }
        public List<double> AxialForceNxy { get; set; }
        public List<double> AxialForceNy { get; set; }
        public List<double> AxialForceNyD { get; set; }
        public List<double> AxialForceVMaxM { get; set; }
        public List<double> MomentAlphaB { get; set; }
        public List<double> MomentM1 { get; set; }
        public List<double> MomentM2 { get; set; }
        public List<double> MomentMcDNegative { get; set; }
        public List<double> MomentMcDPositive { get; set; }
        public List<double> MomentMx { get; set; }
        public List<double> MomentMxDNegative { get; set; }
        public List<double> MomentMxDPositive { get; set; }
        public List<double> MomentMxy { get; set; }
        public List<double> MomentMy { get; set; }
        public List<double> MomentMyDNegative { get; set; }
        public List<double> MomentMyDPositive { get; set; }
        public List<double> MomentTMaxB { get; set; }
        public List<double> ShearForceBetaB { get; set; }
        public List<double> ShearForceVMaxB { get; set; }
        public List<double> ShearForceVx { get; set; }
        public List<double> ShearForceVy { get; set; }

        // Additional Properties to the RFEM Struct
        // public Plane Axis { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-MemberForces;MemberNo:{SurfaceNo};" +
                $"Locations:{Location.Count};");
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
