using System.Collections.Generic;

namespace Server.Parser
{
    public class StructDefultDataObj
    {
        public static List<DefultDataObj> structDefultDataObj = new List<DefultDataObj>();

        public class DefultDataObj
        {
            public string IED { get; private set; }
            public string LDevice { get; private set; }
            public string LN { get; private set; }
            public string DOI { get; private set; }
            public string DAI { get; private set; }
            public string Value { get; private set; }

            public DefultDataObj(string ied, string ld, string ln, string doi, string dai, string value)
            {
                IED = ied;
                LDevice = ld;
                LN = ln;
                DOI = doi;
                DAI = dai;
                Value = value;
            }
        }

        public static void AddStructDefultDataObj(string ied, string ld, string ln, string doi, string dai, string value)
        {
            DefultDataObj dataObj = new DefultDataObj(ied, ld, ln, doi, dai, value);
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
