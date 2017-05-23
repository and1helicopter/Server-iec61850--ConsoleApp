using System.Collections.Generic;

namespace Server.Parser
{
    public class DataObj
    {
        //Авто инициализированные
        public static readonly List<DefultDataObj> StructDataObj = new List<DefultDataObj>();

        //Инциализированные из файла
        public class DefultDataObj
        {
            public string Path { get; }
            public string Type { get; set; }
            public string Value { get; set; }

            public DefultDataObj(string path, string type, string value)
            {
                Path = path;
                Type = type;
                Value = value;
            }
        }
    }
}
