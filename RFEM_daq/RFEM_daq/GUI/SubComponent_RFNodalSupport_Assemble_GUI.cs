using Grasshopper.Kernel;
using Grasshopper.Kernel.Parameters;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using System;
using System.Collections.Generic;
using RFEM_daq.UIWidgets;
using RFEM_daq.Utilities;
using RFEM_daq.HelperLibraries;
using Dlubal.RFEM5;
using RFEM_daq.RFEM;

namespace RFEM_daq.GUI
{
    public class SubComponent_RFNodalSupport_Assemble_GUI : SubComponent
    {
        public override string name()
        {
            return "Assemble";
        }
        public override string display_name()
        {
            return "Assemble";
        }
        public override void registerEvaluationUnits(EvaluationUnitManager mngr)
        {
            EvaluationUnit evaluationUnit = new EvaluationUnit(name(), display_name(), "Assemble Nodal Supports.");
            evaluationUnit.Icon = RFEM_daq.Properties.Resources.Assemble_SupportP;
            mngr.RegisterUnit(evaluationUnit);
            Setup(evaluationUnit);
        }

        public override void OnComponentLoaded()
        {
        }

        protected void Setup(EvaluationUnit unit)
        {

            unit.RegisterInputParam(new Param_Plane(), "Location", "Loc", "Point or Plane to assemble the RFSupport from.", GH_ParamAccess.item);
            unit.Inputs[0].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Support Number", "No", "Optional index number to assign to the RFEM object.", GH_ParamAccess.item);
            unit.Inputs[1].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Displacement Dir X", "Tx", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            unit.Inputs[2].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Displacement Dir Y", "Ty", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            unit.Inputs[3].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Displacement Dir Z", "Tz", "(-1): Fixed; (0): Free; Other: Stiffness in [kN/m]", GH_ParamAccess.item);
            unit.Inputs[4].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Dir X", "Rx", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            unit.Inputs[5].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Dir Y", "Ry", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            unit.Inputs[6].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Number(), "Rotation Dir Z", "Rz", "(-1): Fixed; (0): Free; Other: Stiffness in [kNm/rad]", GH_ParamAccess.item);
            unit.Inputs[7].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_String(), "Comment", "Comment", "Comment.", GH_ParamAccess.item);
            unit.Inputs[8].Parameter.Optional = true;

            GH_ExtendableMenu gH_ExtendableMenu = new GH_ExtendableMenu(0, "advanced");
            gH_ExtendableMenu.Name = "Advanced";
            gH_ExtendableMenu.Collapse();
            unit.RegisterInputParam(new Param_String(), "Node List", "NodeList", "Node List", GH_ParamAccess.item);
            unit.Inputs[9].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Integer(), "Rotation Sequence", "Seq", UtilLibrary.DescriptionRFTypes(typeof(RotationSequence)), GH_ParamAccess.item);
            unit.Inputs[10].EnumInput = UtilLibrary.ListRFTypes(typeof(RotationSequence));
            unit.Inputs[10].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Vector(), "Rotation Angles", "Rot", "Euler Angles [rad]", GH_ParamAccess.item);
            unit.Inputs[11].Parameter.Optional = true;
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[9]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[10]);
            gH_ExtendableMenu.RegisterInputPlug(unit.Inputs[11]);
            unit.AddMenu(gH_ExtendableMenu);

            GH_ExtendableMenu gH_ExtendableMenu2 = new GH_ExtendableMenu(1, "modify");
            gH_ExtendableMenu2.Name = "Modify";
            gH_ExtendableMenu2.Collapse();
            unit.RegisterInputParam(new Param_RFEM(), "RF Nodal Support", "RF NodSup", "Support object from the RFEM model to modify", GH_ParamAccess.item);
            unit.Inputs[12].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Modify", "Modify", "Modify object?", GH_ParamAccess.item);
            unit.Inputs[13].Parameter.Optional = true;
            unit.RegisterInputParam(new Param_Boolean(), "Delete", "Delete", "Delete object?", GH_ParamAccess.item);
            unit.Inputs[14].Parameter.Optional = true;
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[12]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[13]);
            gH_ExtendableMenu2.RegisterInputPlug(unit.Inputs[14]);
            unit.AddMenu(gH_ExtendableMenu2);

            unit.RegisterOutputParam(new Param_RFEM(), "RF Nodal Support", "RF NodSup", "Output RFNodalSupport.");
        }

        public override void SolveInstance(IGH_DataAccess DA, out string msg, out GH_RuntimeMessageLevel level)
        {
            msg = "";
            level = GH_RuntimeMessageLevel.Blank;
            //var line = new LineCurve();
            Plane inPlane = Plane.WorldXY;
            var noIndex = 0;
            var comment = "";
            var rfSup = new RFSupportP();
            var inRFEM = new GH_RFEM();
            var mod = false;
            var del = false;
            var tx = 0.0;
            var ty = 0.0;
            var tz = 0.0;
            var rx = 0.0;
            var ry = 0.0;
            var rz = 0.0;
            var nodeList = "";
            var rotseq = 0;
            var eulerAng = new Vector3d();
            //int newNo = 0;

            if (DA.GetData(12, ref inRFEM))
            {
                rfSup = new RFSupportP((RFSupportP)inRFEM.Value);
                if (DA.GetData(0, ref inPlane))
                {
                    var inPlanes = new List<Plane>();
                    inPlanes.Add(inPlane);
                    rfSup = new RFSupportP(rfSup, inPlanes);
                }
            }
            else if (DA.GetData(0, ref inPlane))
            {
                var inPlanes = new List<Plane>();
                inPlanes.Add(inPlane);
                rfSup = new RFSupportP(new NodalSupport(), inPlanes);
            }
            else if (DA.GetData(9, ref nodeList))
            {
                rfSup = new RFSupportP();
                rfSup.NodeList = nodeList;
            }
                else
            {
                msg = "Insufficient input parameters. Provide either Input Plane/Point or Node List or existing RFSuportP Object. ";
                level = GH_RuntimeMessageLevel.Warning;
                return;
            }
            if (DA.GetData(13, ref mod))
            {
                rfSup.ToModify = mod;
            }
            if (DA.GetData(14, ref del))
            {
                rfSup.ToDelete = del;
            }
            if (DA.GetData(1, ref noIndex))
            {
                rfSup.No = noIndex;
            }
            if (DA.GetData(8, ref comment))
            {
                rfSup.Comment = comment;
            }
            if (DA.GetData(2, ref tx))
            {
                rfSup.Tx = tx;
            }
            if (DA.GetData(3, ref ty))
            {
                rfSup.Ty = ty;
            }
            if (DA.GetData(4, ref tz))
            {
                rfSup.Tz = tz;
            }
            if (DA.GetData(5, ref rx))
            {
                rfSup.Rx = rx;
            }
            if (DA.GetData(6, ref ry))
            {
                rfSup.Ry = ry;
            }
            if (DA.GetData(7, ref rz))
            {
                rfSup.Rz = rz;
            }
            if (DA.GetData(9, ref nodeList))
            {
                rfSup.NodeList = nodeList;
            }
            if (DA.GetData(10, ref rotseq) || (DA.GetData(11, ref rotseq)))
            {
                rfSup.RSType = ReferenceSystemType.UserDefinedSystemType;
                rfSup.UDSType = UserDefinedAxisSystemType.RotatedSystemType;
                if (DA.GetData(10, ref rotseq))
                {
                    rfSup.RSeq = (RotationSequence)rotseq;
                }
                if (DA.GetData(11, ref eulerAng))
                {
                    rfSup.RotX = eulerAng.X;
                    rfSup.RotY = eulerAng.Y;
                    rfSup.RotZ = eulerAng.Z;
                }
                rfSup.GetOrientation();
            }

            DA.SetData(0, rfSup);
        }
    }
}
