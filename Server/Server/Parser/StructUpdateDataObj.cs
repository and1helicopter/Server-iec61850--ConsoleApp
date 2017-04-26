using System;
using System.Collections.Generic;
using System.Data;

namespace Server.Parser
{
    public static class StructUpdateDataObj
    {
        public static List<GetObject> DataClassGet = new List<GetObject>();           //Список классов в которых данные получаем с платы 
        public static List<SetObject> DataClassSet = new List<SetObject>();           //Список классов в которых данные загружаем на плату 

        public class GetObject
        {
            public string NameDataObj { get; private set; }
            public ushort AddrDataObj { get; private set; }
            public string TypeDataObj { get; private set; }
            public string MaskDataObj { get; private set; }
            public string ConvertDataObj { get; private set; }
            public string ClassDataObj { get; private set; }

            public object DataObj;

            public long ValueDataObj { get; private set; }

            public bool SendRequestDataObj { get; private set; }


        }

        public class SetObject
        {
            
        }
    }
}
