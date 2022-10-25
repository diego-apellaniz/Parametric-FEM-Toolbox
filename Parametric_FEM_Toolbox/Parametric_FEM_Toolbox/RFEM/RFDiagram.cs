using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFDiagram : IGrassRFEM
    {
          
        //Standard constructors
        public RFDiagram()
        {
        }
        public RFDiagram(NonlinearityDiagram diagram)
        {
            Comment = diagram.Comment;
            NegativeZone = diagram.NegativeZone.ToPointList();
            PositiveZone = diagram.PositiveZone.ToPointList();
            NegativeZoneType = diagram.NegativeZoneType;
            PositiveZoneType = diagram.PositiveZoneType;
            Symmetric = diagram.Symmetric;

        }

        public RFDiagram(RFDiagram other) : this((NonlinearityDiagram)other)
        {
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        
        public List<Point3d> NegativeZone { get; set; }
        public List<Point3d> PositiveZone { get; set; }
        public DiagramAfterLastStepType NegativeZoneType { get; set; }
        public DiagramAfterLastStepType PositiveZoneType { get; set; }
        public bool Symmetric { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }



        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-Diagram;");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator NonlinearityDiagram(RFDiagram diagram)
        {
            NonlinearityDiagram myDiagram = new NonlinearityDiagram
            {
                Comment = diagram.Comment,
                NegativeZone = diagram.NegativeZone.ToDiagramArray(),
                PositiveZone = diagram.PositiveZone.ToDiagramArray(),
                NegativeZoneType = diagram.NegativeZoneType,
                PositiveZoneType = diagram.PositiveZoneType,
                Symmetric = diagram.Symmetric
            };
            return myDiagram;
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
