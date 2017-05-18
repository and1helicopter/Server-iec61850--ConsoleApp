using System.Collections.Generic;

namespace Server.Parser
{
    public class StructDefultDataObj
    {
        //Авто инициализированные
        public static List<DefultDataObj> structDefultDataObj = new List<DefultDataObj>();

        //Инциализированные из файла
        public static List<DefultDataObj> structFillDataObj = new List<DefultDataObj>();

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
