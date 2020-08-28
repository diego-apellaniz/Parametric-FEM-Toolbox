using System;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.Utilities;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFNode: IGrassRFEM
    {
          
        //Standard constructors
        public RFNode()
        {
        }
        public RFNode(Node node, Point3d location)
        {
            Comment = node.Comment;
            CS = node.CS;
            Delta1 = node.Delta1;
            Delta2 = node.Delta2;
            ID = node.ID;
            IsGenerated = node.IsGenerated;
            IsValid = node.IsValid;
            No = node.No;
            RefObjectNo = node.RefObjectNo;
            Tag = node.Tag;
            Type = node.Type;
            X = location.X;
            Y = location.Y;
            Z = location.Z;
            Location = location;
            ToModify = false;
            ToDelete = false;
        }
        public RFNode(Node node) : this (node, new Point3d(node.X,node.Y,node.Z))
        {
        }
        public RFNode(RFNode other) : this(other, other.Location)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public CoordinateSystemType CS { get; set; }
        public double Delta1 { get; set; }
        public double Delta2 { get; set; }
        public string ID { get; set; }
        public bool IsGenerated { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public int RefObjectNo { get; set; }
        public string Tag { get; set; }
        public NodeType Type { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        // Additional Properties to the RFEM Struct
        public Point3d Location { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-Node;No:{No};X:{X}[m];Y:{Y}[m]|[rad];Z:{Z}[m]|[rad];Type:{Type};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"RefObjectNo:{RefObjectNo};IsValid:{IsValid};IsGenerated:{IsGenerated};ID:{((ID == "") ? "-" : ID)};" +
                $"Delta1:{Delta1};Delta2:{Delta2};CS:{CS};ToModify:{ToModify};ToDelete:{ToDelete};" +
                $"Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator Node(RFNode node)
        {
            Node myNode = new Node
            {
                Comment = node.Comment,
                CS = node.CS,
                Delta1 = node.Delta1,
                Delta2 = node.Delta2,
                ID = node.ID,
                IsGenerated = node.IsGenerated,
                IsValid = node.IsValid,
                No = node.No,
                RefObjectNo = node.RefObjectNo,
                Tag = node.Tag,
                Type = node.Type,
                X = node.X,
                Y = node.Y,
                Z = node.Z
            };
            return myNode;
        }

        // Casting to GH Data Types
        public bool ConvertToGH_Point<T>(ref T target)
        {
            object obj = new GH_Point(Location);
            target = (T)obj;
            return true;
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
