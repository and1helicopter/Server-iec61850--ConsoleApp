using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;


namespace ADSPLibrary
{
    public class WordUnit
    {
        List<string> nameStrings = new List<string>();
        public List<string> NameStrings
        {
            set { nameStrings = value; }
            get { return nameStrings; }
        }

        
        public List<string> RelevantList(ushort mask)
        {
            int i = 0;
            ushort mask2 = mask;
            List<string> relevantList = new List<string>();
            
            while (mask2 != 0)
            {
                if ((mask2 & 0x0001) != 0) { relevantList.Add(nameStrings[i]); }
                mask2 = (ushort)(mask2 >> 1);
                i++;
            }
            return relevantList;
        }

        ushort eventPos = 0x0000;
        public ushort EventPos { set { eventPos = value; } get { return eventPos; } }

        public WordUnit(string xmlFileName, string paramName, int blockNum)
        {
            XmlNodeList xmls;
            XmlNode xmlline;
            int i;
            int startpos = 16 * blockNum;

            var doc = new XmlDocument();
            doc.Load(xmlFileName);
            xmls = doc.GetElementsByTagName(paramName);

            if (xmls.Count != 1)
                throw new Exception("Ошибки в файле конфигурации!");

            xmlline = xmls[0];

            for (i = startpos; i < (startpos+16); i++)
            {
                nameStrings.Add(Convert.ToString(xmlline.Attributes["Line" + i.ToString()].Value));
            }

            eventPos = Convert.ToUInt16(xmlline.Attributes["EventPos"+(blockNum).ToString()].Value);

        }
    }
}
