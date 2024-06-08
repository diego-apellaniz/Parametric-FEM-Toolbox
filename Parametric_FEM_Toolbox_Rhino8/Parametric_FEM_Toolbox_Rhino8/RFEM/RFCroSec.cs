using System;
using System.Linq;
using Dlubal.RFEM5;
using Dlubal.RFEM3;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Parametric_FEM_Toolbox.Utilities;
using System.Collections.Generic;
using Parametric_FEM_Toolbox.HelperLibraries;

namespace Parametric_FEM_Toolbox.RFEM
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
        public CSType Type { get; set; } // ICS, Hollow Square, Hollow Circular
        public double H { get; set; } // Height, Height, Diameter
        public double W { get; set; } // Width, Width, ""
        public double Tw { get; set; } // Tw, Tw, Thickness
        public double Tf { get; set; } // Tf, "", "" 
        public double Ri { get; set; } // Fillet Radius, Inner Radius, ""
        public double Ro { get; set; } // Flange Edge Radius, Outer Radius
        public enum CSType
        {
            UnknownType = 0,
            ISection = 1,
            Channel = 2,
            TSection = 3,
            Angle = 4,
            Hollow = 5,
            Pipe = 6,
            ZSection = 7,
            Solid = 8,
            Rail = 9,
            CorrugatedSheet = 10,
            Eliptical = 11,
            SixtyDegreeAngle = 12
        }
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

        public void SetShapeGeometry(IrfDatabaseCrSc crosec_database, IModelData data, CrossSection cs)
        {
            var ics = data.GetCrossSection(cs.No, ItemAt.AtNo);
            Dlubal.RFEM3.IrfCrossSectionDB2 cssDB = (Dlubal.RFEM3.IrfCrossSectionDB2)ics.GetDatabaseCrossSection();
            Type = (CSType)(int)cssDB.rfGetProperty((DB_CRSC_PROPERTY_ID)189);

            IrfCrossSectionDB ICrossSectionDB = crosec_database.rfGetCrossSection(cs.Description);
            int count = 9;
            DB_CRSC_PROPERTY_ID[] ids = new DB_CRSC_PROPERTY_ID[count];
            double[] values = new double[count];
            ids[0] = DB_CRSC_PROPERTY_ID.CRSC_PROP_h;
            ids[1] = DB_CRSC_PROPERTY_ID.CRSC_PROP_b;
            ids[2] = DB_CRSC_PROPERTY_ID.CRSC_PROP_t_g;
            ids[3] = DB_CRSC_PROPERTY_ID.CRSC_PROP_t_s;
            ids[4] = DB_CRSC_PROPERTY_ID.CRSC_PROP_r;
            ids[5] = DB_CRSC_PROPERTY_ID.CRSC_PROP_r_1;
            ids[6] = DB_CRSC_PROPERTY_ID.CRSC_PROP_s;
            ids[7] = DB_CRSC_PROPERTY_ID.CRSC_PROP_r_o;
            ids[8] = DB_CRSC_PROPERTY_ID.CRSC_PROP_D;
            ICrossSectionDB.rfGetPropertyArr(count, ids, values);

            switch (Type)
            {
                case CSType.ISection:
                    H = values[0];
                    W = values[1];
                    Tf = values[2];
                    Tw = values[3];
                    Ri = values[4];
                    Ro = values[5];
                    break;

                case CSType.Hollow:
                    H = Math.Max(values[0], values[1]);
                    W = values[1];
                    Tw = Math.Max(values[3], values[6]);
                    Ri = values[4];
                    Ro = values[7];
                    break;

                case CSType.Pipe:
                    H = values[8];
                    W = values[8];
                    Tw = values[6];
                    break;
            }
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
