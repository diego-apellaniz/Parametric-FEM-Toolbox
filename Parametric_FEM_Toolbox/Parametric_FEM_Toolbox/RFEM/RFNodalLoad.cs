using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFNodalLoad : IGrassRFEM
    {
          
        //Standard constructors
        public RFNodalLoad()
        {
        }
        public RFNodalLoad(NodalLoad load, List<Point3d> location, int loadcase)
        {
            NodeList = load.NodeList;
            Comment = load.Comment;
            ID = load.ID;
            IsValid = load.IsValid;
            No = load.No;
            Tag = load.Tag;
            LoadDefType = load.Definition;
            if (LoadDefType == LoadDefinitionType.ByComponentsType)
            {
                Force = new Vector3d(load.Component.Force.X, load.Component.Force.Y, load.Component.Force.Z)*0.001;
                Moment = new Vector3d(load.Component.Moment.X, load.Component.Moment.Y, load.Component.Moment.Z)*0.001;
            }else if ((LoadDefType == LoadDefinitionType.ByDirectionType) && (load.Direction.Type == NodalLoadDirectionType.RotatedDirectionType) )
            {
                var basisVec = SetOrientation(load.Direction.RotationSequence, load.Direction.RotationAngles);
                Force = new Vector3d(basisVec * load.Direction.Force)*0.001;
                Moment = new Vector3d(basisVec * load.Direction.Moment)*0.001;
            }
            LoadCase = loadcase;
            Location = location;
            ToModify = false;
            ToDelete = false;
        //}
        //public RFSupportP(NodalSupport support, Point3d[] location) : this(support, location, new Plane(Plane.WorldXY))
        //{

        }

        public RFNodalLoad(RFNodalLoad other) : this(other, other.Location, other.LoadCase)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public string NodeList { get; set; }
        public LoadDefinitionType LoadDefType { get; set; }
        public Vector3d Force { get; set; }
        public Vector3d Moment { get; set; }
        // Additional Properties to the RFEM Struct
        public int LoadCase { get; set; }
        public List<Point3d> Location { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-NodalLoad;No:{No};LoadCase:{LoadCase};" +
                $"Fx:{Force.X.ToString("0.00")}[kN];Fy:{Force.Y.ToString("0.00")}[kN];Fz:{Force.Z.ToString("0.00")}[kN];" +
                $"Mx:{Moment.X.ToString("0.00")}[kN];My:{Moment.Y.ToString("0.00")}[kN];Mz:{Moment.Z.ToString("0.00")}[kN];" +
                $"LoadDefinitionType:{LoadDefType};" +
                $"NodeList:{((NodeList == "") ? "-" : NodeList)};IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator NodalLoad(RFNodalLoad load)
        {
            NodalLoad myLoad = new NodalLoad
            {
                Comment = load.Comment,
                ID = load.ID,
                IsValid = load.IsValid,
                No = load.No,
                Tag = load.Tag,
                NodeList = load.NodeList,
             };
            if (load.LoadDefType == LoadDefinitionType.ByComponentsType)
            {
                myLoad.Definition = LoadDefinitionType.ByComponentsType;
                myLoad.Component.Force.X = load.Force.X*1000;
                myLoad.Component.Force.Y = load.Force.Y * 1000;
                myLoad.Component.Force.Z = load.Force.Z * 1000;
                myLoad.Component.Moment.X = load.Moment.X * 1000;
                myLoad.Component.Moment.Y = load.Moment.Y * 1000;
                myLoad.Component.Moment.Z = load.Moment.Z * 1000;
            }
            else if (load.LoadDefType == LoadDefinitionType.ByDirectionType)
            {
                myLoad.Definition = LoadDefinitionType.ByDirectionType;
                myLoad.Direction.Type = NodalLoadDirectionType.RotatedDirectionType;
                myLoad.Direction.RotationSequence = RotationSequence.SequenceZYX;
                myLoad.Direction.RotationAngles = load.GetOrientation();
                myLoad.Direction.Force = load.Force.Length * 1000;
                myLoad.Direction.Moment = load.Moment.Length * 1000;
            }
            return myLoad;
        }

        public static Vector3d SetOrientation(RotationSequence seq, Point3D rotationAngles)
        {
            var axisX = new Vector3d(Vector3d.XAxis);
            var axisY = new Vector3d(Vector3d.YAxis);
            var axisZ = new Vector3d(Vector3d.ZAxis);

                    switch (seq)
                    {
                        case RotationSequence.SequenceXYZ:
                            {
                                axisZ.Rotate(rotationAngles.X, axisX);
                                axisX.Rotate(rotationAngles.Y, axisY);
                                axisZ.Rotate(rotationAngles.Y, axisY);
                                axisX.Rotate(rotationAngles.Z, axisZ);
                                axisY.Rotate(rotationAngles.Z, axisZ);
                                break;
                            }
                        case RotationSequence.SequenceXZY:
                            {
                                axisY.Rotate(rotationAngles.X, axisX);
                                axisZ.Rotate(rotationAngles.X, axisX);
                                axisX.Rotate(rotationAngles.Z, axisZ);
                                axisY.Rotate(rotationAngles.Z, axisZ);
                                axisX.Rotate(rotationAngles.Y, axisY);
                                axisZ.Rotate(rotationAngles.Y, axisY);
                                break;
                            }
                        case RotationSequence.SequenceYXZ:
                            {
                                axisX.Rotate(rotationAngles.Y, axisY);
                                axisZ.Rotate(rotationAngles.Y, axisY);
                                axisY.Rotate(rotationAngles.X, axisX);
                                axisZ.Rotate(rotationAngles.X, axisX);
                                axisX.Rotate(rotationAngles.Z, axisZ);
                                axisY.Rotate(rotationAngles.Z, axisZ);
                                break;
                            }
                        case RotationSequence.SequenceYZX:
                            {
                                axisX.Rotate(rotationAngles.Y, axisY);
                                axisZ.Rotate(rotationAngles.Y, axisY);
                                axisX.Rotate(rotationAngles.Z, axisZ);
                                axisY.Rotate(rotationAngles.Z, axisZ);
                                axisY.Rotate(rotationAngles.X, axisX);
                                axisZ.Rotate(rotationAngles.X, axisX);
                                break;
                            }
                        case RotationSequence.SequenceZXY:
                            {
                                axisX.Rotate(rotationAngles.Z, axisZ);
                                axisY.Rotate(rotationAngles.Z, axisZ);
                                axisY.Rotate(rotationAngles.X, axisX);
                                axisZ.Rotate(rotationAngles.X, axisX);
                                axisX.Rotate(rotationAngles.Y, axisY);
                                axisZ.Rotate(rotationAngles.Y, axisY);
                                break;
                            }
                        case RotationSequence.SequenceZYX:
                            {
                                axisX.Rotate(rotationAngles.Z, axisZ);
                                axisY.Rotate(rotationAngles.Z, axisZ);
                                axisX.Rotate(rotationAngles.Y, axisY);
                                axisZ.Rotate(rotationAngles.Y, axisY);
                                axisY.Rotate(rotationAngles.X, axisX);
                                axisZ.Rotate(rotationAngles.X, axisX);
                                break;
                            }
                    }
            // RFEM uses the positive axis z as the basis vector
            return axisZ;         
        }

            public Point3D GetOrientation()
            {
            // Use Force as objective vector to rotate angles
            var vector = new Vector3d(Force);
                vector.Unitize();
                // Define global axis
                var x1 = new Vector3d(Vector3d.XAxis);
                var x2 = new Vector3d(Vector3d.YAxis);
                var x3 = new Vector3d(Vector3d.ZAxis);
                // Get new Axis                
                var x1Prime = Vector3d.CrossProduct(vector, x3);
            x1Prime.Unitize();
            var x2Prime = Vector3d.CrossProduct(vector, x1Prime);
            x1Prime.Unitize();
            var x3Prime = vector;
            // Get Rotation Matrix
            double m11 = Vector3d.Multiply(x1, x1Prime);
                var m12 = Vector3d.Multiply(x1, x2Prime);
                var m13 = Vector3d.Multiply(x1, x3Prime);
                var m21 = Vector3d.Multiply(x2, x1Prime);
                var m22 = Vector3d.Multiply(x2, x2Prime);
                var m23 = Vector3d.Multiply(x2, x3Prime);
                var m31 = Vector3d.Multiply(x3, x1Prime);
                var m32 = Vector3d.Multiply(x3, x2Prime);
                var m33 = Vector3d.Multiply(x3, x3Prime);
                // Get Euler angles
                var theta = 0.0;
                var psi = 0.0;
                var phi = 0.0;
                
                if ((!(m31 == 1))&&(!(m31 == -1)))
                {
                    theta = -Math.Asin(m31);
                    psi = Math.Atan2(m32 / Math.Cos(theta), m33 / Math.Cos(theta));
                    phi = Math.Atan2(m21 / Math.Cos(theta), m11 / Math.Cos(theta));
                }else
                {
                    phi = 0;
                    if (m31 == -1)
                    {
                        theta = Math.PI / 2;
                        psi = phi + Math.Atan2(m12, m13);
                    }else
                    {
                        theta = - Math.PI / 2;
                        psi = -phi + Math.Atan2(-m12, -m13);
                    }
                }

            // Output in degrees?
            //myAxisSystem.RotationAngles.X = psi * 180 / Math.PI;
            //myAxisSystem.RotationAngles.Y = theta * 180 / Math.PI;
            //myAxisSystem.RotationAngles.Z = phi * 180 / Math.PI;
            var myRotadAngles = new Point3D();
            myRotadAngles.X = psi;
            myRotadAngles.Y = theta;
            myRotadAngles.Z = phi;
            return myRotadAngles;
            }

                // Casting to GH Data Types

                public bool ConvertToGH_Point<T>(ref T target)
        {
            if (Location.Count == 1)
            {
                object obj = new GH_Point(Location[0]);
                target = (T)obj;
                return true;
            }
            return false;
        }

        public bool ConvertToGH_Plane<T>(ref T target)
        {
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
            return ConvertToGH_Point(ref target);
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
