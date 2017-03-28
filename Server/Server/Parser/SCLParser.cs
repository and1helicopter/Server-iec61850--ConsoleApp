using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Server.Parser
{
    public class SCLParser
    {
        public static void ParseFile()
        {
            string filePath = @"Config.icd";
            //читаем данные из файла
            XDocument doc = XDocument.Load(filePath);
        }
    }
}