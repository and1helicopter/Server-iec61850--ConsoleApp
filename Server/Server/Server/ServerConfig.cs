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
        }
    }
}