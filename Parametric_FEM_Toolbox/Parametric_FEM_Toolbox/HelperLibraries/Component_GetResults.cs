using System;
using System.Collections.Generic;
using System.Linq;
using Dlubal.RFEM5;
using Dlubal.RFEM3;
using Parametric_FEM_Toolbox.Utilities;
using Parametric_FEM_Toolbox.RFEM;
using Grasshopper.Kernel;
using Rhino.Geometry;
using System.Runtime.InteropServices;
using Grasshopper;
using Grasshopper.Kernel.Data;

namespace Parametric_FEM_Toolbox.HelperLibraries
{
    public static class Component_GetResults
    {
       
        public static void GetLoadCasesAndCombos(this ILoads loads, ref List<string> lCases, ref List<string> lCombos , ref List<string> rCombos)
        {
            lCases = new List<string>();
            lCombos = new List<string>();
            rCombos = new List<string>();

            lCases = loads.GetLoadCases().Select(x => "LoadCase " + x.Loading.No.ToString()).ToList();
            lCombos = loads.GetLoadCases().Select(x => "LoadCombo " + x.Loading.No.ToString()).ToList();
            rCombos = loads.GetLoadCases().Select(x => "ResultCombo " + x.Loading.No.ToString()).ToList();
        }

        public static void GetLoadCasesAndCombos(this ILoads loads, ref List<string> lCasesAndCombos, ref int countCases, ref int countCombos, ref int countRcombos)
        {
            var lCases = new List<string>();
            var lCombos = new List<string>();
            var rCombos = new List<string>();

            lCases = loads.GetLoadCases().Select(x => "LoadCase " + x.Loading.No.ToString()).ToList();
            countCases = lCases.Count;
            lCombos = loads.GetLoadCombinations().Select(x => "LoadCombo " + x.Loading.No.ToString()).ToList();
            countCombos = lCombos.Count;
            rCombos = loads.GetResultCombinations().Select(x => "ResultCombo " + x.Loading.No.ToString()).ToList();
            countRcombos = rCombos.Count;

            lCasesAndCombos = new List<string>();
            lCasesAndCombos.AddRange(lCases);
            lCasesAndCombos.AddRange(lCombos);
            lCasesAndCombos.AddRange(rCombos);
        }

        //public class RFMeshResult
        //{
        //    private int _surfaceNo;
        //    private ResultsValueType _valuetype;
        //    private IDictionary<int, int> nodeIndex;

        //    public RFMeshResult(int surfaceNo, ResultsValueType valuetype, IModelData data, List<FeMesh2DElement> elemenets)
        //    {
        //        _surfaceNo = surfaceNo;
        //        _valuetype = valuetype;


        //        foreach (var iteelementm in elemenets)
        //        {

        //        }

        //    }
        //}
    }
}