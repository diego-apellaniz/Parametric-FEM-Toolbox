using System;
using Dlubal.RFEM5;
using Grasshopper.Kernel.Types;
using Rhino.Geometry;
using Parametric_FEM_Toolbox.Utilities;


namespace Parametric_FEM_Toolbox.RFEM
{
    [Serializable]
    public class RFMaterial : IGrassRFEM
    {
        //Standard constructors
        public RFMaterial()
        {
        }
        public RFMaterial(Material material)
        {
            Comment = material.Comment;
            ID = material.ID;
            UserDefined = material.UserDefined;
            IsValid = material.IsValid;
            No = material.No;
            Tag = material.Tag;
            Description = material.Description;
            UserDefined = material.UserDefined;
            E = material.ElasticityModulus;
            Gamma = material.PartialSafetyFactor;
            Mu = material.PoissonRatio;
            W = material.SpecificWeight;
            Alpha = material.ThermalExpansion;
            G = material.ShearModulus;
            ModelType = material.ModelType;
            TextID = material.TextID;
            ToModify = false;
            ToDelete = false;
        }

        public RFMaterial(RFMaterial other) : this((Material)other)
        {
            ToModify = other.ToModify;
            ToDelete = other.ToDelete;
        }

        // Properties to Wrap Fields from RFEM Struct
        public string Comment { get; set; }
        public string ID { get; set; }
        public bool IsValid { get; set; }
        public int No { get; set; }
        public string Tag { get; set; }
        public string Description { get; set; }
        public double E { get; set; }
        public double Gamma { get; set; }
        public double Mu { get; set; }
        public double W { get; set; }
        public double G { get; set; }
        public double Alpha { get; set; }
        public MaterialModelType ModelType { get; set; }
        public bool UserDefined { get; set; }
        public string TextID { get; set; }
        // Additional Properties to the RFEM Struct
        public bool ToModify { get; set; }
        public bool ToDelete { get; set; }
        //public int NewNo { get; set; }

        // Display Info of the RFEM Objects on Panels
        // Parameters are separated by ";". The component split text can be used to break the string down into a list.
        public override string ToString()
        {
            return string.Format($"RFEM-Material;No:{No};Description:{((Description == "") ? "-" : Description)};" +
                $"ElasticityModulus:{E}[N/m²];PartialSafetyFactor:{Gamma};PoissonRatio:{Mu};" + 
                $"SpecificWeight:{W}[N/m³];ThermalExpansion:{Alpha}[1/°];ShearModulus:{G}[N/m²];" +
                $"IsValid:{IsValid};ID:{((ID == "") ? "-" : ID)};TextID:{((TextID == "") ? "-" : TextID)};" +
                $"UserDefined:{UserDefined};" +
                $"ToModify:{ToModify};ToDelete:{ToDelete};Comment:{((Comment == "") ? "-" : Comment)};");
        }

        //Operator to retrieve a Line from an rfLine.
        public static implicit operator Material(RFMaterial material)
        {
            var myMat = new Material
            {
                Comment = material.Comment,
                ID = material.ID,
                IsValid = material.IsValid,
                No = material.No,
                Tag = material.Tag,
            };
            myMat.Description = material.Description;
            myMat.UserDefined = material.UserDefined;
            myMat.TextID = material.TextID;
            myMat.ElasticityModulus = material.E;
            myMat.PartialSafetyFactor = material.Gamma;
            myMat.PoissonRatio = material.Mu;
            myMat.SpecificWeight = material.W;
            myMat.ThermalExpansion = material.Alpha;
            myMat.ShearModulus = material.G;
            myMat.ModelType = material.ModelType;
            return myMat;
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
