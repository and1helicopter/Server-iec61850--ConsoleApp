namespace ServerLib.Server
{
	public static partial class ServerIEC61850
	{
		public static class ServerConfig
		{
			public static int ServerPort { get; set; }					//Номер порта на котором открывается сервер
			public static string LocalIpAddr { get; set; }				//IP сервера
			public static bool Autostart { get; set; }					//Автостарт 
			public static bool AdditionalParams { get; set; }			//Дополнительные возможности
			
			public static string NamePathDirectory { get; set; }		//Директория
			public static string NameConfigFile { get; set; } = "ServerModel.icd";           //Имя icd файла
			public static string NameModelFile { get; set; }			//Имя cfg файла
			public static string NameDirectoryServer { get; set; }		//Рабочая директория сервера
		}
	}
}