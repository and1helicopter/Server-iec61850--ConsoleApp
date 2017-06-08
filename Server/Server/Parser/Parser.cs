using System.Xml.Linq;

namespace Server.Parser
{
    public static partial class Parser
    {
        public static bool ParseFile()
        {
            string filePath = @"test.icd";
            //читаем данные из файла
            XDocument doc;
            try
            {
                doc = XDocument.Load(filePath);
            }
            catch 
            {
                Log.Log.Write("ParseDocunent: File icd no found!!!", "Error   ");
                return false;
            }

            if (doc.Root != null)
            {
                if (!ParseDocument(doc)) //Парсим основные классы модели
                {
                    Log.Log.Write("ParseDocunent: Finish whith status false", "Error   ");
                    return false;
                }

                if (!ModelFillDefultParam()) //Заполнение  модели параметрами по-умолчанию
                {
                    Log.Log.Write("ModelFillDefultParam: Finish whith status false", "Error   ");
                    return false;
                }

                if (!FileParseToAttribute(doc)) //Заполняем объектную модель инициализированными параметрами 
                {
                    Log.Log.Write("FileParseToAttribute: Finish whith status false", "Warning ");
                }

                CreateClassFromAttribute(); //Создаем обновляймые классы 

                SaveFileConfig(); //Создаем из объектной модели - конфигурационную 
            }

            return true;
        }
    }
}