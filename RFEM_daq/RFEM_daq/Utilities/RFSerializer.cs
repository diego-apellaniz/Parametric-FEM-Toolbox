using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace RFEM_daq.Utilities
{
    public class RFSerializer
    {
        private static RFSerializer instance;

        private IFormatter formatter;

        public IFormatter Formatter => formatter;

        private RFSerializer()
        {
            formatter = new BinaryFormatter
            {
                Binder = new ActiveTypeBinder()
            };
            
        }

        public static RFSerializer GetInstance()
        {
            if (instance == null)
            {
                instance = new RFSerializer();
            }
            return instance;
        }
    }
}
