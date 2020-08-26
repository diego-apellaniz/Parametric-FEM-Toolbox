using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using RFEM_daq.HelperLibraries;
using Rhino.Geometry;


namespace RFEM_daq.RFEM
{
    [Serializable]
    public class RFSupportS : IGrassRFEM
    {
          
        //Standard constructors
        public RFSupportS()
        {
        }
        public RFSupportS(SurfaceSupport support, List<RFSurface> baseSrfcs)
        {
            SurfaceList = support.SurfaceList;
            Comment = support.Comment;
            ID = support.ID;
            IsValid = support.IsValid;
            No = support.No;
            Tag = support.Tag;
            Vxz = support.ShearConstantXZ / 1000;
            Vyz = support.ShearConstantYZ / 1000;
            Tx = support.SupportConstantX / 1000;
            Ty = support.SupportConstantY / 1000;
            Tz = support.SupportConstantZ / 1000;
            BaseSrfcs = baseSrfcs;
            ToModify = false;
            ToDelete = false;
        }

        public RFSupportS(RFSupportS other) : this(other, null)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
            var newSrfcs = new List<RFSurface>();
            if (!(other.BaseSrfcs == null))
            {
                foreach (var srf in other.BaseSrfcs)
                {
                    newSrfcs.Add(new RFSurface(srf));
                }
            }
            BaseSrfcs = newSrfcs;
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public string SurfaceList { get; set; }
        public double Tx { get; set; }
        public double Ty { get; set; }
        public double Tz { get; set; }
        public double Vxz { get; set; }
        public double Vyz { get; set; }
        // Additional Properties to the RFEM Struct
        public List<RFSurface> BaseSrfcs { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-SrfcSupport;No:{No};Ux:{Tx.DOF("kN/m")};Uy:{Ty.DOF("kN/m")};Uz:{Tz.DOF("kN/m")};" +
                $"Vxz:{Vxz.DOF("kNm/rad")};Vyz:{Vyz.DOF("kNm/rad")};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"SurfaceList:{((SurfaceList == "") ? "-" : SurfaceList)};IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator SurfaceSupport(RFSupportS support)
        {
            SurfaceSupport mySupport = new SurfaceSupport
            {
                Comment = support.Comment,
                ID = support.ID,
                IsValid = support.IsValid,
                No = support.No,
                Tag = support.Tag,
                SurfaceList = support.SurfaceList,
                SupportConstantX = support.Tx * 1000,
                SupportConstantY = support.Ty * 1000,
                SupportConstantZ = support.Tz * 1000,
                ShearConstantXZ = support.Vxz * 1000,
                ShearConstantYZ = support.Vyz * 1000,
             };
            return mySupport;
        }

        // Convert RFEM Object into Rhino Geometry.
        // These methods are later implemented by the class GH_RFEM.
        public bool ToGH_Integer<T>(ref T target)
        {
            object obj = new GH_Integer(No);
            target = (T)obj;
            return true;
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
            if (BaseSrfcs.Count == 1)
            {
                object obj = new GH_Brep(BaseSrfcs[0].ToBrep());
                target = (T)obj;
                return true;
            }
            return false;
        }
        public bool ToGH_Brep<T>(ref T target)
        {
            if (BaseSrfcs.Count == 1)
            {
                object obj = new GH_Brep(BaseSrfcs[0].ToBrep());
                target = (T)obj;
                return true;
            }
            return false;
        }
        public bool ToGH_Plane<T>(ref T target)
        {
            return false;
        }

    }
}
