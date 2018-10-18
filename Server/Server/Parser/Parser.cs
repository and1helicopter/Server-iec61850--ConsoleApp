using System.Xml.Linq;

namespace ServerLib.Parser
{
	public static partial class Parser
	{
		public static bool ParseFile(string filePath, bool dependencesModel)
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

			if (doc.Root == null) return false;
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

			if (!CreateServices(doc))
			{
				Log.Log.Write("CreateServices: Finish whith status false", "Error");
				return false;
			}

			if (dependencesModel)
			{
				if (!DependencesModel())
				{
					Log.Log.Write("DependencesModel: Finish whith status false", "Error");
				}
			}

			SaveFileConfig(); //Создаем из объектной модели - конфигурационную 

			return true;
		}
	}
}