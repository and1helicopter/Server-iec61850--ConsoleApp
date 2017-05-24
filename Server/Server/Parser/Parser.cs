using System.Xml.Linq;

namespace Server.Parser
{
    public partial class Parser
    {
        public bool ParseFile()
        {
            string filePath = @"test.icd";
            //читаем данные из файла
            XDocument doc = XDocument.Load(filePath);

            if (doc.Root != null)
            {
                if (!ParseDocument(doc)) //Парсим основные классы модели
                {
                    Logging.Log.Write("ParseDocunent: Finish whith status false", "Error   ");
                    return false;
                }

                if (!ModelFillDefultParam()) //Заполнение  модели параметрами по-умолчанию
                {
                    Logging.Log.Write("ModelFillDefultParam: Finish whith status false", "Error   ");
                    return false;
                }

                if (!FileParseToAttribute(doc)) //Заполняем объектную модель инициализированными параметрами 
                {
                    Logging.Log.Write("FileParseToAttribute: Finish whith status false", "Warning ");
                }

                CreateClassFromAttribute(); //Создаем обновляймые классы 

                SaveFileConfig(); //Создаем из объектной модели - конфигурационную 
            }

            return true;
        }
    }
}