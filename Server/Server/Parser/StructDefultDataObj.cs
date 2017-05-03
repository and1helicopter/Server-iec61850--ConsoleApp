using System.Collections.Generic;

namespace Server.Parser
{
    public class StructDefultDataObj
    {
        public static List<DefultDataObj> structDefultDataObj = new List<DefultDataObj>();

        public class DefultDataObj
        {
            public string Path { get; private set; }
            public string Type { get; private set; }
            public string Value { get; private set; }

            public DefultDataObj(string path, string type, string value)
            {
                Path = path;
                Type = type;
                Value = value;
            }
        }

        public static void AddStructDefultDataObj(string path, string type, string value)
        {
            DefultDataObj dataObj = new DefultDataObj(path, type, value);
            structDefultDataObj.Add(dataObj);
        }

        public static void RemoveDefultDataObj(int index)
        {
            if (index >= structDefultDataObj.Count) { return; }
            structDefultDataObj.RemoveAt(index);
        }

        public static void ClearDefultDataObj()
        {
            structDefultDataObj.Clear();
        }
    }
}
