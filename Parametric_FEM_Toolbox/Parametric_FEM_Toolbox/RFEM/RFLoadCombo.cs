using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;
using System.Linq;

namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFLoadCombo : IGrassRFEM
    {
          
        //Standard constructors
        public RFLoadCombo()
        {
        }
        public RFLoadCombo(LoadCombination load)
        {
            Comment = load.Comment;
            ID = load.ID;
            IsValid = load.IsValid;
            No = load.Loading.No;
            Tag = load.Tag;
            ToSolve = load.ToSolve;
            Description = load.Description;
            Definition = load.Definition;
            DesignSituation = load.DesignSituation;
            ToModify = false;
            ToDelete = false;
        }

        //Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public string Definition { get; set; }
        public DesignSituationType DesignSituation { get; set; }
        public bool ToSolve { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-LoadCombination;No:{No};" +
                $"Definition:{Definition};DesignSituation:{DesignSituation.ToString()};" +
                $"Description:{((Description == "") ? "-" : Description)};ToSolve:{ToSolve.ToString()};" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator LoadCombination(RFLoadCombo lcombo)
        {
            LoadCombination myLoadCombo = new LoadCombination
            {
                Comment = lcombo.Comment,
                ID = lcombo.ID,
                IsValid = lcombo.IsValid,
                Tag = lcombo.Tag,
                Description = lcombo.Description,
                Definition = lcombo.Definition,
                ToSolve = lcombo.ToSolve,
                DesignSituation = lcombo.DesignSituation
            };
            myLoadCombo.Loading.No = lcombo.No;
            myLoadCombo.Loading.Type = LoadingType.LoadCombinationType;
            return myLoadCombo;
        }


        // Casting to GH Data Types
        public bool ToGH_Integer<T>(ref T target)
        {
            object obj = new GH_Integer(No);
            target = (T)obj;
            return true;
        }
        public bool ConvertToGH_Plane<T>(ref T target)
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
