using System;
using System.Runtime.Serialization;

namespace Parametric_FEM_Toolbox.Utilities
{
    //Class used to serialize the RFEM objects.
    internal sealed class ActiveTypeBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            if (GetType().Assembly.FullName.Equals(assemblyName))
            {
                return GetType().Assembly.GetType(typeName);
            }
            return null;
        }
    }
}
