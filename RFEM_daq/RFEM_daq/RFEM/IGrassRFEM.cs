
namespace RFEM_daq.RFEM
{
    // Interface to be implemented by all RFEM Classes so that their instances can be converted into the Grasshopper Data Type "GH_RFEM"
    public interface IGrassRFEM
    {
        // Display Info of the RFEM Objects on Panels
        string ToString();
        // Define Casts
        bool ToGH_Integer<T>(ref T target);
        bool ToGH_Point<T>(ref T target);
        bool ToGH_Line<T>(ref T target);
        bool ToGH_Curve<T>(ref T target);
        bool ToGH_Surface<T>(ref T target);
        bool ToGH_Brep<T>(ref T target);
        bool ToGH_Plane<T>(ref T target);
        // Properties required to modify objects
        bool ToModify { get; set; }
        bool ToDelete { get; set; }
        //int NewNo { get; set; }
    }
}
