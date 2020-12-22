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

        public static List<string> GetLoadCasesAndCombos(this ILoads loads, ref int countCases, ref int countCombos, ref int countRcombos)
        {
            var lCasesAndCombos = new List<string>();

            countCases = 0;
            countCombos = 0;
            countRcombos = 0;
            // Load cases
            foreach (var lc in loads.GetLoadCases())
            {
                var loading = lc.Loading;
                if (loads.HasLoadingResults(loading))
                {
                    lCasesAndCombos.Add($"LoadCase {loading.No}");
                    countCases++;
                }
            }

            // Load combos
            foreach (var lc in loads.GetLoadCombinations())
            {
                var loading = lc.Loading;
                if (loads.HasLoadingResults(loading))
                {
                    lCasesAndCombos.Add($"LoadCombo {loading.No}");
                }
                countCombos++;
            }

            // Result combos
            foreach (var lc in loads.GetResultCombinations())
            {
                var loading = lc.Loading;
                if (loads.HasLoadingResults(loading))
                {
                    lCasesAndCombos.Add($"ResultCombo {loading.No}");
                }
                countRcombos++;
            }

            return lCasesAndCombos;
        }

        public static void GetAllCalculatedResults(this ILoads loads, ref List<string> lCasesAndCombos, ref ICalculation results, ref int countCases, ref int countCombos, ref int countResultCombos, ref List<string> msg)
        {
            /// Get results just of calculated load cases and combos and add names to dropdown menu

            lCasesAndCombos = new List<string>();
            countCases = 0;
            countCombos = 0;
            countResultCombos = 0;

            // Load cases
            foreach (var lc in loads.GetLoadCases())
            {
                var loading = lc.Loading;
                if (loads.HasLoadingResults(loading))
                {
                    lCasesAndCombos.Add($"LoadCase {loading.No}");
                    var errors = results.Calculate(loading.Type, loading.No);
                    if (errors != null)
                    {
                        msg.AddRange(errors.Select(x => x.Description));
                    }
                    countCases++;
                }
            }

            // Load combos
            foreach (var lc in loads.GetLoadCombinations())
            {
                var loading = lc.Loading;
                if (loads.HasLoadingResults(loading))
                {
                    lCasesAndCombos.Add($"LoadCombo {loading.No}");
                    var errors = results.Calculate(loading.Type, loading.No);
                    if (errors != null)
                    {
                        msg.AddRange(errors.Select(x => x.Description));
                    }
                }
                countCombos++;
            }

            // Result combos
            foreach (var lc in loads.GetResultCombinations())
            {
                var loading = lc.Loading;
                if (loads.HasLoadingResults(loading))
                {
                    lCasesAndCombos.Add($"ResultCombo {loading.No}");
                    var errors = results.Calculate(loading.Type, loading.No);
                    if (errors != null)
                    {
                        msg.AddRange(errors.Select(x => x.Description));
                    }
                }
                countResultCombos++;
            }

        }

        public static void GetResults(ref ICalculation results, string iLoadCase, ref Vector3d displacement, ref List<string> msg)
        {
            var lc_name_parts = iLoadCase.Split(' ');
            if (lc_name_parts.Length < 2)
            {
                msg.Add("Provide valid load cases.");
                return;
            }
            int no = 0;
            if(!int.TryParse(lc_name_parts[1], out no))
            {
                msg.Add("Provide valid load cases.");
                return;
            }
            ErrorInfo[] errors;
            switch (lc_name_parts[0])
            {
                case "LoadCase":
                    errors = results.Calculate(LoadingType.LoadCaseType, no);
                    if (errors != null && errors.Length>0 && errors[0].Description != "")
                    {
                        msg.AddRange(errors.Select(x => x.Description));
                    }else
                    {
                        displacement = new Vector3d(results.GetResultsInFeNodes(LoadingType.LoadCaseType, no).GetMaximum().Displacement.ToPoint3d());
                    }
                    break;
                case "LoadCombo":
                    errors = results.Calculate(LoadingType.LoadCombinationType, no);
                    if (errors != null && errors.Length > 0 && errors[0].Description != "")
                    {
                        msg.AddRange(errors.Select(x => x.Description));
                    }
                    else
                    {
                        displacement = new Vector3d(results.GetResultsInFeNodes(LoadingType.LoadCombinationType, no).GetMaximum().Displacement.ToPoint3d());
                    }
                    break;
                case "ResultCombo":
                    errors = results.Calculate(LoadingType.ResultCombinationType, no);
                    if (errors != null && errors.Length > 0 && errors[0].Description != "")
                    {
                        msg.AddRange(errors.Select(x => x.Description));
                    }
                    else
                    {
                        //displacement = new Vector3d(results.GetResultsInFeNodes(LoadingType.ResultCombinationType, no).GetMaximum().Displacement.ToPoint3d());
                        displacement = new Vector3d(); // result combinations have no max information
                    }
                    break;
                default:
                    msg.Add("Error retrieving calculation results from the model");
                    break;
            }
        }


    }
}