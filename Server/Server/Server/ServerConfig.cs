using ServerLib.Format;

namespace ServerLib.Server
{
	public static partial class ServerIEC61850
	{
		public static class ServerConfig
		{
			public static int ServerPort { get; set; }    //Номер порта на котором открывается сервер
			public static string NameConfigFile { get; set; }   //Имя icd файла
			public static string NameModelFile { get; set; } //Имя cfg файла
			public static string LocalIPAddr { get; set; }   //IP сервера
			public static bool Autostart { get; set; }    //Автостарт 
			public static bool AdditionalParams { get; set; } //Дополнительные возможности
		}
	}
}