using System;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.HelperLibraries;
using System.Collections.Generic;



namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFLine : IGrassRFEM
    {                  
        //Standard constructors
        public RFLine()
        {
        }
        public RFLine(Dlubal.RFEM5.Line line, Point3d[] controlPoints)
        {
            Comment = line.Comment;
            ID = line.ID;
            IsGenerated = line.IsGenerated;
            IsValid = line.IsValid;
            No = line.No;
            Tag = line.Tag;
            Length = line.Length;
            NodeCount = line.NodeCount;
            NodeList = line.NodeList;
            RotationAngle = line.Rotation.Angle;
            RotationHelpNodeNo = line.Rotation.HelpNodeNo;
            RotationPlane = line.Rotation.Plane;
            RotationType = line.Rotation.Type;
            Type = line.Type;
            ControlPoints = controlPoints;
            Order = 2;            
            ToModify = false;
            ToDelete = false;
        }
        public RFLine(Dlubal.RFEM5.Line line, Point3d[] controlPoints, int order, double[] weights, double[] knots) : this(line, controlPoints)
        {
            Order = order;
            Weights = weights;
            Knots = knots;
        }
        public RFLine(Dlubal.RFEM5.Line line) : this(line, null)
        {
        }

        public RFLine(RFLine other) : this(other, other.ControlPoints, other.Order, other.Weights, other.Knots)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
        }

        // Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }        
        public double[] Knots { get; set; }
        public double[] Weights { get; set; }
        public double Length { get; set; }
        public int NodeCount { get; set; }
        public string NodeList { get; set; }
        public double RotationAngle { get; set; }
        public int RotationHelpNodeNo { get; set; }
        public PlaneType RotationPlane { get; set; }
        public RotationType RotationType { get; set; }
        public LineType Type { get; set; }
        // Additional Properties to the RFEM Struct
        public Point3d[] ControlPoints { get; set; }
        
        public int Order { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
           return string.Format($"RFEM-Line;No:{No};Length:{Length}[m];Type:{Type};NodeList:{((NodeList == "") ? "-" : NodeList)};" +
               $"NodeCount:{NodeCount};Nodes:{ControlPoints.ToLabelString()};Tag:{((Tag == "") ? "-" : Tag)};" +
               $"IsValid:{IsValid};IsGenerated:{IsGenerated};ID:{((ID == "") ? "-" : ID)};RotationAngle:{RotationAngle}[rad];" +
               $"RotationHelpNodeNo:{RotationHelpNodeNo};RotationPlane:{RotationPlane};RotationType:{RotationType};" +
               $"Order:{((Order == 0) ? "-" : Order.ToString())};" +
               $"Weights:{(Weights.ToLabelString())};Knots:{(Knots.ToLabelString())};" +
               $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }        

        //Operator to retrieve a Line from an rfLine.
        public static implicit operator Dlubal.RFEM5.Line(RFLine line)
        {
            Dlubal.RFEM5.Line myLine = new Dlubal.RFEM5.Line
            {
                Comment = line.Comment,
                ID = line.ID,
                IsGenerated = line.IsGenerated,
                IsValid = line.IsValid,
                No = line.No,
                Tag = line.Tag,
                Length = line.Length,
                NodeCount = line.NodeCount,
                NodeList = line.NodeList
            };
            myLine.Rotation.Angle = line.RotationAngle;
            myLine.Rotation.HelpNodeNo = line.RotationHelpNodeNo;
            myLine.Rotation.Plane = line.RotationPlane;
            myLine.Rotation.Type = line.RotationType;
            myLine.Type = line.Type;
            return myLine;
        }
        public static implicit operator Dlubal.RFEM5.NurbSpline(RFLine line)
        {
            Dlubal.RFEM5.NurbSpline myNurbSpline = new Dlubal.RFEM5.NurbSpline
            {
                General = line,
                Knots = line.Knots,
                Weights = line.Weights,
                Order = line.Order
            };
            return myNurbSpline;
        }



        // Casting to GH Data Types
        public bool ConvertToGH_Line<T>(ref T target)
        {
            if ((NodeCount == 2) && (Type == LineType.PolylineType))
            {
                Rhino.Geometry.Line pline = new Rhino.Geometry.Line(ControlPoints[0], ControlPoints[1]);
                object obj = new GH_Line(pline);
                target = (T)obj;
                return true;
            }
            return false;
        }
        public Curve ToCurve()
        {
            switch (Type)
            {
                case LineType.PolylineType:
                    if (NodeCount == 2)
                    {
                        return new LineCurve(ControlPoints[0], ControlPoints[1]);
                    }
                    else
                    {
                        return new PolylineCurve(ControlPoints);
                    }
                case LineType.SplineType:
                    {
                        // Warning
                        return Curve.CreateInterpolatedCurve(ControlPoints, 3);
                    }
                case LineType.CircleType:
                    {
                         return new ArcCurve (new Circle(ControlPoints[0], ControlPoints[1], ControlPoints[2]));
                    }
                case LineType.ArcType:
                    {
                        //return Curve.CreateInterpolatedCurve(ControlPoints, 3, CurveKnotStyle.Chord);
                        var vector1 = new Vector3d(ControlPoints[1]- ControlPoints[0]);
                        var vector2 = new Vector3d(ControlPoints[2] - ControlPoints[1]);
                        if (vector1.IsParallelTo(vector2,0.1)==0)
                        {
                            return new Arc(ControlPoints[0], ControlPoints[1], ControlPoints[2]).ToNurbsCurve();
                        }else
                        return new LineCurve(ControlPoints[0], ControlPoints[2]);
                    }
                case LineType.EllipseType:
                    {
                        // Warning
                        return new Ellipse((ControlPoints[0] + ControlPoints[2])/2, ControlPoints[0], ControlPoints[1]).ToNurbsCurve();
                    }
                case LineType.EllipticalArcType:
                    {
                        return Curve.CreateInterpolatedCurve(ControlPoints, 3);
                    }
                case LineType.ParabolaType:
                    {
                        return Curve.CreateInterpolatedCurve(ControlPoints, 3);
                    }
                case LineType.HyperbolaType:
                    {
                        return Curve.CreateInterpolatedCurve(ControlPoints, 3);
                    }
                case LineType.NurbSplineType:
                    {
                        var nurbsCurve =  NurbsCurve.Create(false, Order-1, ControlPoints);
                        for (int j = 0; j < nurbsCurve.Points.Count; j++)
                        {
                            nurbsCurve.Points[j] = new ControlPoint(ControlPoints[j], Weights[j]);
                        }
                        for (int k = 0; k < nurbsCurve.Knots.Count; k++)
                        {
                            nurbsCurve.Knots[k] = Knots[k+1];
                        }
                        return nurbsCurve;
                    }

            }
            return null;
        }

        public bool ConvertToGH_Curve<T>(ref T target)
        {
            var crv = ToCurve();
            if (!(crv is null))
            {
                object obj = new GH_Curve(crv);
                target = (T)obj;
                return true;
            }
            return false;
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
            return ConvertToGH_Line(ref target);
        }
        public bool ToGH_Curve<T>(ref T target)
        {
            return ConvertToGH_Curve(ref target);
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
