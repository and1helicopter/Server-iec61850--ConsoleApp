using Server.Format;

namespace Server.Server
{
	public static partial class Server
	{
		public static class ServerConfig
		{
			public static int PortServer { get; set; }          //Номер порта на котором открывается сервер
			public static string NameConfigFile { get; set; }   //Имя icd файла
			public static int TimeUpdate { get; set; }          //На сколько отправляется сервер в сон
			public static string NameModelFile { get; set; }    //Имя cfg файла
			public static string LocalIPAddr { get; set; }      //IP сервера
			public static bool Autostart { get; set; }          //Автостарт 
			public static bool OldFormat //Новый или старый формат
			{
				get => FormatConverter.OldFormat;
				set => FormatConverter.OldFormat = value;
			}
			public static byte CodeDevice  = 0x07;
		}
	}
}