using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using RFEM_daq.HelperLibraries;
using Rhino.Geometry;


namespace RFEM_daq.RFEM
{
    [Serializable]
    public class RFSupportP : IGrassRFEM
    {
          
        //Standard constructors
        public RFSupportP()
        {
        }
        public RFSupportP(NodalSupport support, List<Plane> orientation)
        {
            NodeList = support.NodeList;
            Comment = support.Comment;
            ID = support.ID;
            IsValid = support.IsValid;
            No = support.No;
            Tag = support.Tag;
            Rx = support.RestraintConstantX/1000;
            Ry = support.RestraintConstantY / 1000;
            Rz = support.RestraintConstantZ / 1000;
            Tx = support.SupportConstantX / 1000;
            Ty = support.SupportConstantY / 1000;
            Tz = support.SupportConstantZ / 1000;
            RSType = support.ReferenceSystem;
            if (support.UserDefinedReferenceSystem.Type == UserDefinedAxisSystemType.RotatedSystemType)
            {
                UDSType = UserDefinedAxisSystemType.RotatedSystemType;
                RSeq = support.UserDefinedReferenceSystem.RotationSequence;
                RotX = support.UserDefinedReferenceSystem.RotationAngles.X;
                RotY = support.UserDefinedReferenceSystem.RotationAngles.Y;
                RotZ = support.UserDefinedReferenceSystem.RotationAngles.Z;
            }
            //UDAxisSystem = support.UserDefinedReferenceSystem;
            // UDAxisSystemType = UDAxisSystem.Type;
                Orientation = orientation;
            ToModify = false;
            ToDelete = false;
            SetOrientation();
        //}
        //public RFSupportP(NodalSupport support, Point3d[] location) : this(support, location, new Plane(Plane.WorldXY))
        //{

        }
        public RFSupportP(NodalSupport support) : this (support, null)
        {
        }


        public RFSupportP(RFSupportP other) : this(other, other.Orientation)
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
        public ReferenceSystemType RSType { get; set; }
        public UserDefinedAxisSystemType UDSType { get; set; }
        public RotationSequence RSeq { get; set; }
        public double RotX { get; set; }
        public double RotY { get; set; }
        public double RotZ { get; set; }
        // public UserDefinedAxisSystem UDAxisSystem { get; set; }
        // public UserDefinedAxisSystemType UDAxisSystemType { get; set; }
        public double Tx { get; set; }
        public double Ty { get; set; }
        public double Tz { get; set; }
        public double Rx { get; set; }
        public double Ry { get; set; }
        public double Rz { get; set; }
        //public RotationSequence RSequence { get; set; }
        //public Point3D RotationAngles { get; set; }

        // Additional Properties to the RFEM Struct
        public List<Plane> Orientation { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-NodalSupport;No:{No};Ux:{Tx.DOF("kN/m")};Uy:{Ty.DOF("kN/m")};Uz:{Tz.DOF("kN/m")};" +
                $"φx:{Rx.DOF("kNm/rad")};φy:{Ry.DOF("kNm/rad")};φz:{Rz.DOF("kNm/rad")};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"NodeList:{((NodeList == "") ? "-" : NodeList)};IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};ReferenceSystemType:{RSType.ToString()};" +
                $"Orientation:{Orientation.ToLabelString()};RSeq:{RSeq};RotX:{RotX};RotY:{RotY};RotZ:{RotZ};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator NodalSupport(RFSupportP support)
        {
            NodalSupport mySupport = new NodalSupport
            {
                Comment = support.Comment,
                ID = support.ID,
                IsValid = support.IsValid,
                No = support.No,
                Tag = support.Tag,
                NodeList = support.NodeList,
                SupportConstantX = support.Tx*1000,
                SupportConstantY = support.Ty * 1000,
                SupportConstantZ = support.Tz * 1000,
                RestraintConstantX = support.Rx * 1000,
                RestraintConstantY = support.Ry * 1000,
                RestraintConstantZ = support.Rz * 1000,
                ReferenceSystem = support.RSType,                
                //UserDefinedReferenceSystem = support.UDAxisSystem
             };
            if (support.RSType == ReferenceSystemType.UserDefinedSystemType)
            {
                var myudaxis = new UserDefinedAxisSystem();
                myudaxis.Type = UserDefinedAxisSystemType.RotatedSystemType;
                myudaxis.RotationSequence = support.RSeq;
                myudaxis.RotationAngles.X = support.RotX;
                myudaxis.RotationAngles.Y = support.RotY;
                myudaxis.RotationAngles.Z = support.RotZ;
                mySupport.UserDefinedReferenceSystem = myudaxis;
            }
            return mySupport;
        }

        public void GetOrientation()
        {
            if (UDSType == UserDefinedAxisSystemType.RotatedSystemType)
            {
                    var axisX = new Vector3d(Vector3d.XAxis);
                    var axisY = new Vector3d(Vector3d.YAxis);
                    var axisZ = new Vector3d(Vector3d.ZAxis);

                    switch (RSeq)
                    {
                        case RotationSequence.SequenceXYZ:
                            {
                                axisY.Rotate(RotX, axisX);
                                axisZ.Rotate(RotX, axisX);
                                axisX.Rotate(RotY, axisY);
                                axisZ.Rotate(RotY, axisY);
                                axisX.Rotate(RotZ, axisZ);
                                axisY.Rotate(RotZ, axisZ);
                                break;
                            }
                        case RotationSequence.SequenceXZY:
                            {
                                axisY.Rotate(RotX, axisX);
                                axisZ.Rotate(RotX, axisX);
                                axisX.Rotate(RotZ, axisZ);
                                axisY.Rotate(RotZ, axisZ);
                                axisX.Rotate(RotY, axisY);
                                axisZ.Rotate(RotY, axisY);
                                break;
                            }
                        case RotationSequence.SequenceYXZ:
                            {
                                axisX.Rotate(RotY, axisY);
                                axisZ.Rotate(RotY, axisY);
                                axisY.Rotate(RotX, axisX);
                                axisZ.Rotate(RotX, axisX);
                                axisX.Rotate(RotZ, axisZ);
                                axisY.Rotate(RotZ, axisZ);
                                break;
                            }
                        case RotationSequence.SequenceYZX:
                            {
                                axisX.Rotate(RotY, axisY);
                                axisZ.Rotate(RotY, axisY);
                                axisX.Rotate(RotZ, axisZ);
                                axisY.Rotate(RotZ, axisZ);
                                axisY.Rotate(RotX, axisX);
                                axisZ.Rotate(RotX, axisX);
                                break;
                            }
                        case RotationSequence.SequenceZXY:
                            {
                                axisX.Rotate(RotZ, axisZ);
                                axisY.Rotate(RotZ, axisZ);
                                axisY.Rotate(RotX, axisX);
                                axisZ.Rotate(RotX, axisX);
                                axisX.Rotate(RotY, axisY);
                                axisZ.Rotate(RotY, axisY);
                                break;
                            }
                        case RotationSequence.SequenceZYX:
                            {
                                axisX.Rotate(RotZ, axisZ);
                                axisY.Rotate(RotZ, axisZ);
                                axisX.Rotate(RotY, axisY);
                                axisZ.Rotate(RotY, axisY);
                                axisY.Rotate(RotX, axisX);
                                axisZ.Rotate(RotX, axisX);
                                break;
                            }
                    }
                var newOrientList = new List<Plane>();
                foreach (var item in Orientation)
                {
                    newOrientList.Add(new Plane(item.Origin,axisX,axisY));
                }
                    Orientation = newOrientList;                
            }
        }

        public void SetOrientation()
        {
            if (!(Orientation[0].XAxis == Vector3d.XAxis) && !(Orientation[0].YAxis == Vector3d.YAxis))
            {
            RSType = ReferenceSystemType.UserDefinedSystemType;
            //var myAxisSystem = new UserDefinedAxisSystem();
            UDSType = UserDefinedAxisSystemType.RotatedSystemType;
            RSeq = RotationSequence.SequenceZYX;
            // Define global axis
            var x1 = new Vector3d(Vector3d.XAxis);
            var x2 = new Vector3d(Vector3d.YAxis);
            var x3 = new Vector3d(Vector3d.ZAxis);
            // Get new Axis
            var x1Prime = Orientation[0].XAxis;
            var x2Prime = Orientation[0].YAxis;
            var x3Prime = Orientation[0].ZAxis;
            // Get Rotation Matrix
            var m11 = Vector3d.Multiply(x1, x1Prime);
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
            RotX = psi;
            RotY = theta;
            RotZ = phi;
            }
        }

        // Casting to GH Data Types
        public bool ConvertToGH_Point<T>(ref T target)
        {
            if (Orientation.Count == 1)
            {
                object obj = new GH_Point(Orientation[0].Origin);
                target = (T)obj;
                return true;
            }
            return false;
        }

        public bool ConvertToGH_Plane<T>(ref T target)
        {
            if (Orientation.Count == 1)
            {
                object obj = new GH_Plane(Orientation[0]);
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
            return ConvertToGH_Plane(ref target);
        }

    }
}
