using System;
using System.Linq;
using Dlubal.RFEM5;
using Dlubal.RFEM3;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using RFEM_daq.Utilities;
using System.Collections.Generic;
using RFEM_daq.HelperLibraries;

namespace RFEM_daq.RFEM
{
    [Serializable]
    public class RFCroSec : IGrassRFEM
    {
        //Standard constructors
        public RFCroSec()
        {
        }
        public RFCroSec(CrossSection croSec)
        {
            Comment = croSec.Comment;
            ID = croSec.ID;
            UserDefined = croSec.UserDefined;
            IsValid = croSec.IsValid;
            No = croSec.No;
            Tag = croSec.Tag;
            Description = croSec.Description;
            MatNo = croSec.MaterialNo;
            RotationAngle = croSec.Rotation * 180 / Math.PI;
            A = croSec.AxialArea;
            Ay = croSec.ShearAreaY;
            Az = croSec.ShearAreaZ;
            Iy = croSec.BendingMomentY;
            Iz = croSec.BendingMomentZ;
            Jt = croSec.TorsionMoment;
            TempWidth = croSec.TemperatureLoadWidth;
            TempDepth = croSec.TemperatureLoadDepth;
            UserDefined = croSec.UserDefined;
            TextID = croSec.TextID;
            Shape = null;
            ToModify = false;
            ToDelete = false;
        }

        public RFCroSec(RFCroSec other) : this((CrossSection)other)
        {
            Shape = other.Shape;
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
        }

        // Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public int MatNo { get; set; }
        public double RotationAngle { get; set; }
        public double A { get; set; }
        public double Ay { get; set; }
        public double Az { get; set; }
        public double Iy { get; set; }
        public double Iz { get; set; }
        public double Jt { get; set; }
        public double TempWidth { get; set; }
        public double TempDepth { get; set; }
        public bool UserDefined { get; set; }
        public List<Curve> Shape { get; set; }
        public string TextID { get; set; }
        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-CroSec;No:{No};Description:{((Description == "") ? "-" : Description)};MatNo:{MatNo};" +
                $"AxialArea:{A}[m²];ShearAreaY:{Ay}[m²];ShearAreaZ:{Az}[m²];BendingMomentY:{Iy}[m⁴];BendingMomentZ:{Iz}[m⁴];" + 
                $"TorsionMoment:{Jt}[m⁴];TemperatureLoadWidth:{TempWidth}[m];TemperatureLoadDepth:{TempWidth}[m];" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};TextID:{((TextID == "") ? "-" : TextID)};RotationAngle:{RotationAngle}[°];" +
                $"UserDefined:{UserDefined};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }

        //Operator to retrieve a Line from an rfLine.
        public static implicit operator CrossSection(RFCroSec croSec)
        {
            var myCroSec = new CrossSection
            {
                Comment = croSec.Comment,
                ID = croSec.ID,
                IsValid = croSec.IsValid,
                No = croSec.No,
                Tag = croSec.Tag,
            };
            myCroSec.Description = croSec.Description;
            myCroSec.MaterialNo = croSec.MatNo;
            myCroSec.AxialArea = croSec.A;
            myCroSec.ShearAreaY = croSec.Ay;
            myCroSec.ShearAreaZ = croSec.Az;
            myCroSec.BendingMomentY = croSec.Iy;
            myCroSec.BendingMomentZ = croSec.Iz;
            myCroSec.TorsionMoment = croSec.Jt;
            myCroSec.TemperatureLoadWidth = croSec.TempWidth;
            myCroSec.TemperatureLoadDepth = croSec.TempDepth;
            myCroSec.UserDefined = croSec.UserDefined;
            myCroSec.TextID = croSec.TextID;
            myCroSec.Rotation = croSec.RotationAngle*Math.PI/180;
            return myCroSec;
        }

        public void SetShape(CURVE_2D[] curves)
        {
            var csCurves = new List<Curve>();
            foreach (var crv in curves)
            {
                var pts = crv.arrPoints;
                switch (crv.type)
                {
                    case CURVE_TYPE.CT_ARC:
                        csCurves.Add(new ArcCurve(new Arc(pts[0].ToPoint3d(), pts[1].ToPoint3d(), pts[2].ToPoint3d())));
                        break;
                    case CURVE_TYPE.CT_LINE:
                        csCurves.Add(new LineCurve(pts[0].ToPoint3d(), pts[1].ToPoint3d()));
                        break;
                    case CURVE_TYPE.CT_CIRCLE:
                        var center = pts[0].ToPoint3d();
                        var radius = pts[0].ToPoint3d().DistanceTo(pts[1].ToPoint3d());
                        csCurves.Add(new Circle(center, radius).ToNurbsCurve());
                        break;
                }                    
            }
            Shape = Curve.JoinCurves(csCurves).ToList();
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
