using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace ADSPLibrary
{
    public class DigitUnit
    {
        List<string> digitStrings = new List<string>();
        public List<string> DigitStrings
        {
            set { digitStrings = value; }
            get {return digitStrings;}
        }

        string digitTitle;
        public string DigitTitle
        {
            set { digitTitle = value; }
            get {return digitTitle;}
        }

        ushort addrMasks = 0x3FFF;
        ushort addrValue = 0x3FFF;
        public ushort AddrMasks { set { addrMasks = value; } get { return addrMasks; } }
        public ushort AddrValue { set { addrValue = value; } get { return addrValue; } }

        ushort useMask = 0xFFFF;
        public ushort UseMask { set { useMask = value; } get { return useMask; } }

        ushort plateType = 0;
        public ushort PlateType { set { plateType = value; } get { return plateType; } }

        ushort eventPos = 0x0000;
        public ushort EventPos { set { eventPos = value; } get { return eventPos; } }

        public DigitUnit(string xmlFileName, int digitNum)
        {
            XmlNodeList xmls;
            XmlNode xmlline;
            int i;

            var doc = new XmlDocument();
            doc.Load(xmlFileName);
            xmls = doc.GetElementsByTagName("Digit"+digitNum.ToString());

            if (xmls.Count != 1)
                throw new Exception("Ошибки в файле конфигурации!");
            
            xmlline = xmls[0];

            for (i = 0; i < 16; i++)
            {
                digitStrings.Add(Convert.ToString(xmlline.Attributes["Line" + i.ToString()].Value));
            }
            digitTitle = Convert.ToString(xmlline.Attributes["Title"].Value);
            addrMasks = Convert.ToUInt16(xmlline.Attributes["Addr2"].Value);
            addrValue = Convert.ToUInt16(xmlline.Attributes["Addr1"].Value);
            eventPos = Convert.ToUInt16(xmlline.Attributes["EventPos"].Value);
            useMask = Convert.ToUInt16(xmlline.Attributes["UseMask"].Value);
            plateType = Convert.ToUInt16(xmlline.Attributes["Type"].Value);

        }
    }
}
