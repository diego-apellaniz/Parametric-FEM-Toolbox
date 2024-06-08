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

        public RFResults(IResults results, IModelData data, ref HashSet<ResultsValueType> result_types, string loadcase, bool member_forces, bool surface_forces, bool reaction_forces, bool line_reaction_forces) // rfem object as array because they are related to the same member
        {
            LoadCase = loadcase;
            if (member_forces)
            {
                MemberForces = GetRFMemberForces(results, data, ref result_types);
            }
            if (surface_forces)
            {
                SurfaceForces = GetRFSurfaceForces(results, data, ref result_types);
            }
            if (reaction_forces)
            {
                NodalSupportForces = GetRFNodalSupportForces(results, data, ref result_types);
            }
            if (line_reaction_forces)
            {
               LineSupportForces = GetRFLineSupportForces(results, data, ref result_types);
            }
            ToModify = false;
            ToDelete = false;
        }

        public RFResults(IResults results, IModelData data, bool local, ref HashSet<ResultsValueType> result_types, string loadcase, bool member_forces, bool surface_forces, bool reaction_forces, bool line_reaction_forces) // rfem object as array because they are related to the same member
        {
            LoadCase = loadcase;
            if (member_forces)
            {
                MemberForces = GetRFMemberForces(results, data, ref result_types);
            }
            if (surface_forces)
            {
                SurfaceForces = GetRFSurfaceForces(results, data, ref result_types);
            }
            if (reaction_forces)
            {
                NodalSupportForces = GetRFNodalSupportForces(results, data, ref result_types, local);
            }
            if (line_reaction_forces)
            {
                LineSupportForces = GetRFLineSupportForces(results, data, ref result_types, local);
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

        public List<RFMemberForces> GetRFMemberForces(IResults results, IModelData data, ref HashSet<ResultsValueType> types)
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
                foreach (var type in forces.Select(x => x.Type).Distinct())
                {
                    types.Add(type);
                }                
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

        public List<RFSurfaceForces> GetRFSurfaceForces(IResults results, IModelData data, ref HashSet<ResultsValueType> types)
        {
            var myForces = new List<RFSurfaceForces>();
            foreach (var surface in data.GetSurfaces())
            {
                if (surface.GeometryType == SurfaceGeometryType.UnknownGeometryType)
                {
                    continue;
                }
                var forces = results.GetSurfaceInternalForces(surface.No, ItemAt.AtNo);
                myForces.Add(new RFSurfaceForces(forces));
                foreach (var type in forces.Select(x => x.Type).Distinct())
                {
                    types.Add(type);
                }
            }
            return myForces;
        }


        public List<RFNodalSupportForces> GetRFNodalSupportForces(IResults results, IModelData data)
        {
            var myForces = new List<RFNodalSupportForces>();
            foreach (var nodalsupportforce in results.GetAllNodalSupportForces(false))
            {
                myForces.Add(new RFNodalSupportForces(nodalsupportforce, data));
            }
            return myForces;
        }

        public List<RFNodalSupportForces> GetRFNodalSupportForces(IResults results, IModelData data, ref HashSet<ResultsValueType> types)
        {
            var myForces = new List<RFNodalSupportForces>();
            foreach (var nodalsupportforce in results.GetAllNodalSupportForces(false))
            {
                myForces.Add(new RFNodalSupportForces(nodalsupportforce, data));
                types.Add(nodalsupportforce.Type);
            }
            return myForces;
        }

        public List<RFNodalSupportForces> GetRFNodalSupportForces(IResults results, IModelData data, ref HashSet<ResultsValueType> types, bool local)
        {
            var myForces = new List<RFNodalSupportForces>();
            foreach (var nodalsupportforce in results.GetAllNodalSupportForces(local))
            {
                myForces.Add(new RFNodalSupportForces(nodalsupportforce, data));
                types.Add(nodalsupportforce.Type);
            }
            return myForces;
        }

        public List<RFLineSupportForces> GetRFLineSupportForces(IResults results, IModelData data)
        {
            var myForces = new List<RFLineSupportForces>();
            foreach (var linesupport in data.GetLineSupports())
            {
                foreach (var line in linesupport.LineList.ToInt())
                {
                    var forces = results.GetLineSupportForces(line, ItemAt.AtNo, false);
                    myForces.Add(new RFLineSupportForces(forces));
                }                
            }
            return myForces;
        }

        public List<RFLineSupportForces> GetRFLineSupportForces(IResults results, IModelData data, ref HashSet<ResultsValueType> types)
        {
            var myForces = new List<RFLineSupportForces>();
            foreach (var linesupport in data.GetLineSupports())
            {
                foreach (var line in linesupport.LineList.ToInt())
                {
                    var forces = results.GetLineSupportForces(line, ItemAt.AtNo, false);
                    myForces.Add(new RFLineSupportForces(forces));
                    foreach (var type in forces.Select(x => x.Type).Distinct())
                    {
                        types.Add(type);
                    }
                }
            }
            return myForces;
        }

        public List<RFLineSupportForces> GetRFLineSupportForces(IResults results, IModelData data, ref HashSet<ResultsValueType> types, bool local)
        {
            var myForces = new List<RFLineSupportForces>();
            foreach (var linesupport in data.GetLineSupports())
            {
                foreach (var line in linesupport.LineList.ToInt())
                {
                    var forces = results.GetLineSupportForces(line, ItemAt.AtNo, local);
                    myForces.Add(new RFLineSupportForces(forces));
                    foreach (var type in forces.Select(x => x.Type).Distinct())
                    {
                        types.Add(type);
                    }
                }
            }
            return myForces;
        }

        // Properties to Wrap Fields from RFEM Struct
        public string LoadCase { get; set; }
        public List<RFMemberForces> MemberForces { get; set; }
        public List<RFSurfaceForces> SurfaceForces { get; set; }
        public List<RFNodalSupportForces> NodalSupportForces { get; set; }
        public List<RFLineSupportForces> LineSupportForces { get; set; }
        public string ResultType { get; set; }

        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            var outString = string.Format($"RFEM-Results: {LoadCase}; ResultType: {ResultType}, Available Results:" +
                $"{((MemberForces == null) ? "" : " Member Forces,")}" +
                $"{((SurfaceForces == null) ? "" : " Surface Forces,")}" +
                $"{((NodalSupportForces == null) ? "" : " Nodal Support Forces,")}" +
                $"{((LineSupportForces == null) ? "" : " Line Support Forces,")}");
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
