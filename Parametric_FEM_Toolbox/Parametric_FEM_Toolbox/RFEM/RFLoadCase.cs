﻿using System;
using System.Collections.Generic;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Parametric_FEM_Toolbox.HelperLibraries;
using Rhino.Geometry;
using System.Linq;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFLoadCase : IGrassRFEM
    {
          
        //Standard constructors
        public RFLoadCase()
        {
        }
        public RFLoadCase(LoadCase load)
        {
            Comment = load.Comment;
            ID = load.ID;
            IsValid = load.IsValid;
            No = load.Loading.No;
            Tag = load.Tag;
            ToSolve = load.ToSolve;
            Description = load.Description;
            ActionCategory = load.ActionCategory;
            SelfWeightFactor = new Vector3d(load.SelfWeightFactor.ToPoint3d());
            ToModify = false;
            ToDelete = false;
        }

        public RFLoadCase(RFLoadCase other) : this((LoadCase)other)
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
        public string Description { get; set; }
        public ActionCategoryType ActionCategory { get; set; }
        public Vector3d SelfWeightFactor { get; set; }
        public bool ToSolve { get; set; }
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-LoadCase;No:{No};" +
                $"Description:{((Description == "") ? "-" : Description)};ActionCategory:{ActionCategory.ToString()};" +
                $"SelfWeightFactor:{SelfWeightFactor.ToString()};ToSolve:{ToSolve.ToString()};" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};Tag:{((Tag == "") ? "-" : Tag)};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }           

        //Operator to retrieve a Node from an rfNode.
        public static implicit operator LoadCase(RFLoadCase lcase)
        {
            LoadCase myLoadCase = new LoadCase
            {
                Comment = lcase.Comment,
                ID = lcase.ID,
                IsValid = lcase.IsValid,
                Tag = lcase.Tag,
                Description = lcase.Description,
                ToSolve = lcase.ToSolve,
                ActionCategory = lcase.ActionCategory
            };
            myLoadCase.Loading.No = lcase.No;
            myLoadCase.Loading.Type = LoadingType.LoadCaseType;
            if (lcase.SelfWeightFactor.Length >0)
            {
                myLoadCase.SelfWeight = true;
                myLoadCase.SelfWeightFactor = new Point3d(lcase.SelfWeightFactor).ToPoint3D();
            }
            else
            {
                myLoadCase.SelfWeight = false;
            }
            return myLoadCase;
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
