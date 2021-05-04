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
    public class RFResults : IGrassRFEM
    {
        //Standard constructors
        public RFResults()
        {
        }
        public RFResults(IResults results, IModelData data, string loadcase, bool member_forces, bool surface_forces, bool reaction_forces) // rfem object as array because they are related to the same member
        {
            LoadCase = loadcase;
            if (member_forces)
            {
                MemberForces = GetRFMemberForces(results, data);
            }
            if (surface_forces)
            {
                SurfaceForces = GetRFSurfaceForces(results, data);
            }
            if (reaction_forces)
            {
                NodalSupportForces = GetRFNodalSupportForces(results, data);
            }            
            ToModify = false;
            ToDelete = false;
        }

        //Testing
        //public RFResults(IResults results, IResults results_raster, IModelData data, string loadcase, bool member_forces, bool surface_forces, bool reaction_forces):this(results, data, loadcase, member_forces, surface_forces, reaction_forces)
        //{
        //    if (surface_forces)
        //    {
        //        SurfaceForces = GetRFSurfaceForces(results, results_raster, data);
        //    }
        //}

        public List<RFMemberForces> GetRFMemberForces(IResults results, IModelData data)
        {
            var myForces = new List<RFMemberForces>();
            foreach (var member in data.GetMembers())
            {
                if (member.Type == MemberType.NullMember)
                {
                    continue;
                }
                var forces = results.GetMemberInternalForces(member.No, ItemAt.AtNo, true); // get results of the right type
                  myForces.Add(new RFMemberForces(forces));
            }
            return myForces;
        }

        public List<RFSurfaceForces> GetRFSurfaceForces(IResults results, IModelData data)
        {
            var myForces = new List<RFSurfaceForces>();            
            foreach (var surface in data.GetSurfaces())
            {
                if (surface.GeometryType == SurfaceGeometryType.UnknownGeometryType)
                {
                    continue;
                }
                var forces = results.GetSurfaceInternalForces(surface.No, ItemAt.AtNo);
                //var globalDeformations = results.GetSurfaceDeformations(surface.No, ItemAt.AtNo,false);
                //var localDeformations = results.GetSurfaceDeformations(surface.No, ItemAt.AtNo, true);
                //var local_axis = Component_GetResults.GetSurfaceLocalAxis(globalDeformations, localDeformations);
                myForces.Add(new RFSurfaceForces(forces));
            }
            return myForces;
        }

        //Test
        //public List<RFSurfaceForces> GetRFSurfaceForces(IResults results, IResults results_raster, IModelData data)
        //{
        //    var myForces = new List<RFSurfaceForces>();
        //    foreach (var surface in data.GetSurfaces())
        //    {
        //        if (surface.GeometryType == SurfaceGeometryType.UnknownGeometryType)
        //        {
        //            continue;
        //        }
        //        var forces = results.GetSurfaceInternalForces(surface.No, ItemAt.AtNo);
        //        var globalDeformations = results_raster.GetSurfaceDeformations(surface.No, ItemAt.AtNo, false);
        //        var localDeformations = results_raster.GetSurfaceDeformations(surface.No, ItemAt.AtNo, true);
        //        var local_axis = Component_GetResults.GetSurfaceLocalAxis(globalDeformations, localDeformations);
        //        myForces.Add(new RFSurfaceForces(forces, local_axis));
        //    }
        //    return myForces;
        //}

        public List<RFNodalSupportForces> GetRFNodalSupportForces(IResults results, IModelData data)
        {
            var myForces = new List<RFNodalSupportForces>();
            foreach (var nodalsupportforce in results.GetAllNodalSupportForces(false))
            {
                myForces.Add(new RFNodalSupportForces(nodalsupportforce, data));
            }
            return myForces;
        }

        // Properties to Wrap Fields from RFEM Struct
        public string LoadCase { get; set; }
        public List<RFMemberForces> MemberForces { get; set; }
        public List<RFSurfaceForces> SurfaceForces { get; set; }
        public List<RFNodalSupportForces> NodalSupportForces { get; set; }

        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            var outString = string.Format($"RFEM-Results: {LoadCase}; Available Results:" +
                $"{((MemberForces == null) ? "" : " Member Forces,")}" +
                $"{((SurfaceForces == null) ? "" : " Surface Forces,")}" +
                $"{((NodalSupportForces == null) ? "" : " Nodal Support Forces,")}");
            outString = outString.Substring(0, outString.Length - 1) + ";";
            return outString;
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
