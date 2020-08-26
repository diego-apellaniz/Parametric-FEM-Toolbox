using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RFEM_daq.RFEM;
using RFEM_daq.HelperLibraries;
using Dlubal.RFEM5;
using System.Reflection;
using Rhino.Geometry;

namespace RFEM_daq.Utilities
{
    public class RFFilter
    {
        public RFFilter()
        {
        }
        public string Type { get; set; }
        public override string ToString()
        {
            return Type;
        }
        public List<int> NodeList { get; set; }
        public List<string> NodeComment { get; set; }
        public List<Interval> NodesX {get;set;}
        public List<Interval> NodesY { get; set; }
        public List<Interval> NodesZ { get; set; }
        public List<string> NodeCS { get; set; }
        public List<int> NodeRef { get; set; }
        public List<int> LineList { get; set; }
        public List<string> LineComment { get; set; }
        public List<Interval> LinesX { get; set; }
        public List<Interval> LinesY { get; set; }
        public List<Interval> LinesZ { get; set; }
        public List<int> LineNoList { get; set; }
        public List<string> LineType { get; set; }
        public List<string> LineRotType { get; set; }
        public List<Interval> LineRotAngle { get; set; }
        public List<Interval> LineLength { get; set; }
        public List<int> MemberList { get; set; }
        public List<string> MemberComment { get; set; }
        public List<Interval> MemberX { get; set; }
        public List<Interval> MemberY { get; set; }
        public List<Interval> MemberZ { get; set; }
        public List<string> MemberType { get; set; }
        public List<int> MemberLineNo { get; set; }
        public List<int> MemberStartCS { get; set; }
        public List<int> MemberEndCS { get; set; }
        public List<string> MemberRotType { get; set; }
        public List<Interval> MemberRotAngle { get; set; }
        public List<int> MemberStartHinge { get; set; }
        public List<int> MemberEndHinge { get; set; }
        public List<int> MemberEcc { get; set; }
        public List<int> MemberDivision { get; set; }
        public List<string> MemberTaper { get; set; }
        public List<Interval> MemberFactorY { get; set; }
        public List<Interval> MemberFactorZ { get; set; }
        public List<Interval> MemberLength { get; set; }
        public List<Interval> MemberWeight { get; set; }
        public List<int> SrfcList { get; set; }
        public List<string> SrfcComment { get; set; }
        public List<int> SrfcBoundLineNo { get; set; }
        public List<Interval> SrfcX { get; set; }
        public List<Interval> SrfcY { get; set; }
        public List<Interval> SrfcZ { get; set; }
        public List<Interval> SrfcThickness { get; set; }
        public List<int> SrfcMaterial { get; set; }
        public List<string> SrfcType { get; set; }
        public List<string> SrfcStiffType { get; set; }
        public List<string> ThickType { get; set; }
        public List<Interval> SrfcEcc { get; set; }
        public List<string> SrfcIntNodeNo { get; set; }
        public List<string> SrfcIntLineNo { get; set; }
        public List<string> SrfcIntOpNo { get; set; }
        public List<Interval> SrfcArea { get; set; }
        public List<int> OpList { get; set; }
        public List<string> OpComment { get; set; }
        public List<Interval> OpX { get; set; }
        public List<Interval> OpY { get; set; }
        public List<Interval> OpZ { get; set; }
        public List<int> OpBoundLineNo { get; set; }
        public List<int> OpSrfcNo { get; set; }
        public List<Interval> OpArea { get; set; }
        public List<int> SupList { get; set; }
        public List<string> SupComment { get; set; }
        public List<int> SupNodeList { get; set; }
        public List<Interval> SupTx { get; set; }
        public List<Interval> SupTy { get; set; }
        public List<Interval> SupTz { get; set; }
        public List<Interval> SupRx { get; set; }
        public List<Interval> SupRy { get; set; }
        public List<Interval> SupRz { get; set; }
        public List<Interval> SupX { get; set; }
        public List<Interval> SupY { get; set; }
        public List<Interval> SupZ { get; set; }
        public List<int> SupLList { get; set; }
        public List<string> SupLComment { get; set; }
        public List<int> SupLineList { get; set; }
        public List<Interval> SupLTx { get; set; }
        public List<Interval> SupLTy { get; set; }
        public List<Interval> SupLTz { get; set; }
        public List<Interval> SupLRx { get; set; }
        public List<Interval> SupLRy { get; set; }
        public List<Interval> SupLRz { get; set; }
        public List<Interval> SupLX { get; set; }
        public List<Interval> SupLY { get; set; }
        public List<Interval> SupLZ { get; set; }
        public List<string> SupLRefSys { get; set; }
        public List<int> SupSList { get; set; }
        public List<string> SupSComment { get; set; }
        public List<int> SupSrfcList { get; set; }
        public List<Interval> SupSTx { get; set; }
        public List<Interval> SupSTy { get; set; }
        public List<Interval> SupSTz { get; set; }
        public List<Interval> SupSVxz { get; set; }
        public List<Interval> SupSVyz { get; set; }
        public List<Interval> SupSX { get; set; }
        public List<Interval> SupSY { get; set; }
        public List<Interval> SupSZ { get; set; }        
        public List<int> LHList { get; set; }
        public List<string> LHComment { get; set; }
        public List<int> LHLineNo { get; set; }
        public List<int> LHSfcNo { get; set; }
        public List<Interval> LHTx { get; set; }
        public List<Interval> LHTy { get; set; }
        public List<Interval> LHTz { get; set; }
        public List<Interval> LHRx { get; set; }
        public List<Interval> LHRy { get; set; }
        public List<Interval> LHRz { get; set; }
        public List<Interval> LHX { get; set; }
        public List<Interval> LHY { get; set; }
        public List<Interval> LHZ { get; set; }
        public List<string> LHSide { get; set; }
        public List<int> CSList { get; set; }
        public List<string> CSComment { get; set; }
        public List<string> CSDes { get; set; }
        public List<int> CSMatNo { get; set; }
        public List<Interval> CSA { get; set; }
        public List<Interval> CSAy { get; set; }
        public List<Interval> CSAz { get; set; }
        public List<Interval> CSIy { get; set; }
        public List<Interval> CSIz { get; set; }
        public List<Interval> CSJt { get; set; }
        public List<Interval> CSRotAngle { get; set; }
        public List<Interval> CSTempW { get; set; }
        public List<Interval> CSTempD { get; set; }
        public List<bool> CSUserDefined { get; set; }
        public List<int> MatList { get; set; }
        public List<string> MatComment { get; set; }
        public List<string> MatDes { get; set; }
        public List<Interval> MatE { get; set; }
        public List<Interval> MatGamma { get; set; }
        public List<Interval> MatMu { get; set; }
        public List<Interval> MatW { get; set; }
        public List<Interval> MatG { get; set; }
        public List<Interval> MatAlpha { get; set; }
        public List<string> MatModelType { get; set; }
        public List<bool> MatUserDefined { get; set; }
        public List<int> NLList { get; set; }
        public List<string> NLComment { get; set; }
        public List<int> NLLC { get; set; }
        public List<int> NLNodeList { get; set; }
        public List<string> NLDefinition { get; set; }
        public List<Interval> NLFx { get; set; }
        public List<Interval> NLFy { get; set; }
        public List<Interval> NLFz { get; set; }
        public List<Interval> NLMx { get; set; }
        public List<Interval> NLMy { get; set; }
        public List<Interval> NLMz { get; set; }
        public List<Interval> NLX { get; set; }
        public List<Interval> NLY { get; set; }
        public List<Interval> NLZ { get; set; }
        public List<int> LLList { get; set; }
        public List<string> LLComment { get; set; }
        public List<int> LLLC { get; set; }
        public List<int> LLLineList { get; set; }
        public List<Interval> LLF1 { get; set; }
        public List<Interval> LLF2 { get; set; }
        public List<Interval> LLF3 { get; set; }
        public List<Interval> LLt1 { get; set; }
        public List<Interval> LLt2 { get; set; }
        public List<bool> LLTotalLength { get; set; }
        public List<bool> LLRelativeDistances { get; set; }
        public List<string> LLType { get; set; }
        public List<string> LLDir { get; set; }
        public List<string> LLDist { get; set; }
        public List<string> LLRef { get; set; }
        public List<Interval> LLX { get; set; }
        public List<Interval> LLY { get; set; }
        public List<Interval> LLZ { get; set; }
        public List<int> MLList { get; set; }
        public List<string> MLComment { get; set; }
        public List<int> MLLC { get; set; }
        public List<int> MLMemberList { get; set; }
        public List<Interval> MLF1 { get; set; }
        public List<Interval> MLF2 { get; set; }
        public List<Interval> MLF3 { get; set; }
        public List<Interval> MLT4 { get; set; }
        public List<Interval> MLT5 { get; set; }
        public List<Interval> MLT6 { get; set; }
        public List<Interval> MLt1 { get; set; }
        public List<Interval> MLt2 { get; set; }
        public List<bool> MLTotalLength { get; set; }
        public List<bool> MLRelativeDistances { get; set; }
        public List<string> MLType { get; set; }
        public List<string> MLDir { get; set; }
        public List<string> MLDist { get; set; }
        public List<string> MLRef { get; set; }
        public List<Interval> MLX { get; set; }
        public List<Interval> MLY { get; set; }
        public List<Interval> MLZ { get; set; }
        public List<int> SLList { get; set; }
        public List<string> SLComment { get; set; }
        public List<int> SLLC { get; set; }
        public List<int> SLSurfaceList { get; set; }
        public List<Interval> SLF1 { get; set; }
        public List<Interval> SLF2 { get; set; }
        public List<Interval> SLF3 { get; set; }
        public List<Interval> SLT4 { get; set; }
        public List<Interval> SLT5 { get; set; }
        public List<Interval> SLT6 { get; set; }
        public List<string> SLType { get; set; }
        public List<string> SLDir { get; set; }
        public List<string> SLDist { get; set; }
        public List<Interval> SLX { get; set; }
        public List<Interval> SLY { get; set; }
        public List<Interval> SLZ { get; set; }
        public List<int> PLList { get; set; }
        public List<string> PLComment { get; set; }
        public List<int> PLLC { get; set; }
        public List<int> PLSurfaceList { get; set; }
        public List<Interval> PLF1 { get; set; }
        public List<Interval> PLF2 { get; set; }
        public List<Interval> PLF3 { get; set; }
        public List<string> PLProjection { get; set; }
        public List<string> PLDir { get; set; }
        public List<string> PLDist { get; set; }
        public List<Interval> PLX { get; set; }
        public List<Interval> PLY { get; set; }
        public List<Interval> PLZ { get; set; }
        public List<int> LCList { get; set; }
        public List<string> LCComment { get; set; }
        public List<string> LCAction { get; set; }
        public List<string> LCDescription { get; set; }
        public List<bool> LCToSolve { get; set; }
        public List<Interval> LCSWX { get; set; }
        public List<Interval> LCSWY { get; set; }
        public List<Interval> LCSWZ { get; set; }
        public List<int> LCoList { get; set; }
        public List<string> LCoComment { get; set; }
        public List<string> LCoDesign { get; set; }
        public List<string> LCoDescription { get; set; }
        public List<string> LCoDefinition { get; set; }
        public List<bool> LCoToSolve { get; set; }
        public List<int> RCoList { get; set; }
        public List<string> RCoComment { get; set; }
        public List<string> RCoDesign { get; set; }
        public List<string> RCoDescription { get; set; }
        public List<string> RCoDefinition { get; set; }
        public List<bool> RCoToSolve { get; set; }

    }
}
