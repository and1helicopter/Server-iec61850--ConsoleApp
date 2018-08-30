using System.Xml.Linq;

namespace ServerLib.Parser
{
	public static partial class Parser
	{
		public static bool ParseFile(string filePath)
		{
			// filePath = @"test.icd";
			//читаем данные из файла
			XDocument doc;
			try
			{
				doc = XDocument.Load(filePath);
			}
			catch
			{
				Log.Log.Write("ParseDocunent: File icd no found!!!", "Error");
				return false;
			}

			if (doc.Root != null)
			{
				if (!ParseDocument(doc)) //Парсим классы модели
				{
					Log.Log.Write("ParseDocunent: Finish whith status false", "Error");
					return false;
				}

				if (!CreateClassBitArray(doc)) //Парсим битовые поля
				{
					Log.Log.Write("CreateClassBitArray: Finish whith status false", "Error   ");
					return false;
				}

				if (!CreateClassFromAttribute(doc))//Создаем обновляймые классы 
				{
					Log.Log.Write("CreateClassFromAttribute: Finish whith status false", "Error   ");
					return false;
				}

//				if (!ModelFillDefultParam()) //Заполнение  модели параметрами по-умолчанию
//				{
//					Log.Log.Write("ModelFillDefultParam: Finish whith status false", "Error   ");
//					return false;
//				}

//				if (!FileParseToAttribute(doc)) //Заполняем объектную модель инициализированными параметрами 
//				{
//					Log.Log.Write("FileParseToAttribute: Finish whith status false", "Warning ");
//				}


				//ModelParseToUpdateStatus();//Создаем классы для данных Mod, Beh, Health

				SaveFileConfig(); //Создаем из объектной модели - конфигурационную 
			}

			return true;
		}
	}
}