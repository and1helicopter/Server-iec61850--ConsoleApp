using System.Collections.Generic;

namespace Server.Parser
{
    public class DataObj
    {
        //Авто инициализированные
        public static readonly List<DefultDataObj> StructDefultDataObj = new List<DefultDataObj>();

        //Инциализированные из файла
        public static readonly List<DefultDataObj> StructFillDataObj = new List<DefultDataObj>();

        public class DefultDataObj
        {
            public string Path { get; }
            public string Type { get; }
            public string Value { get; }

            public DefultDataObj(string path, string type, string value)
            {
                Path = path;
                Type = type;
                Value = value;
            }
        }
    }
}
