using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Server.Parser
{
    public class SCLParser
    {
        public static void ParseFile()
        {
            string filePath = @"Config.icd";
            //читаем данные из файла
            XDocument doc = XDocument.Load(filePath);

            if (doc.Root != null)
            {
                IEnumerable<XElement> xElements = doc.Root.Elements().ToList();

                IEnumerable<XElement> xElemen = (from x in xElements
                                                 where x.Name.LocalName == "IED"
                                                 select x).Elements().ToList();


                IEnumerable<XElement> xEl = (from x in xElements.Descendants()
                                            where x.Name.LocalName == "LDevice"
                                            select x).Elements().ToList();
            }


            //var ll = xRoot.Element("SCL");

            // var xElement = doc.Element("SCL"); 
            // var xRoot = doc.Root;
            // var xElement = xRoot.Element("SCL");

            //var xNode = xRoot.Nodes().ToList();
            //var oo = xNode.Select(from x in xNode
            //                    where (XElement)x.Name == );
            // var xElement = (XElement)xNode.
            // var elem = xElement.Nodes().ToList();

            //XElement root = xRoot.Element("SCL");
            //var xElement = xRoot.Element("IED");
            //doc.Element("LDevice");


        }
    }
}