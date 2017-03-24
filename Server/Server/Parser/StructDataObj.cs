using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server.Parser
{
    public static class StructDataObj
    {
        public static List<DataObj> structDataObj = new List<DataObj>();

        public class DataObj
        {
            public string nameDataObj { get; private set; }
            public ushort addrDataObj { get; private set; }
            public string formatDataObj { get; private set; }
            public long valueDataObj { get; set; }
            public bool SendRequestDataObj { get; set; }

            public DataObj(string name, ushort addr, string format)
            {
                nameDataObj = name;
                addrDataObj = addr;
                formatDataObj = format;
            }
        }

        public static void AddStructDataObj(string name, ushort addr, string format)
        {
            DataObj dataObj = new DataObj(name, addr, format);
            structDataObj.Add(dataObj);
        }

        public static void RemoveDataObj(int index)
        {
            if(index >= structDataObj.Count) { return;}
            structDataObj.RemoveAt(index); 
        }

        public static void ClearDataObj()
        {
            structDataObj.Clear();
        }
    }
}
