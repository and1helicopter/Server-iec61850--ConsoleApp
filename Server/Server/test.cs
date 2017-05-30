namespace Server
{
    public static class Test
    {
        public static void Main(string[] args)
        {
            //Открываем настройки сервера
            Settings.Settings.ReadSettings();
            //Settings.Settings.SaveSettings();

            //Парсим файл конфигурации
            if (!Parser.Parser.ParseFile())
            {
                Log.Log.Write(@"ParseFile: Finish with status false. Stop server", @"Error   ");
                return;
            }
            Log.Log.Write(@"ParseFile: File parse success", @"Success ");

            //ModBus.ModBus.CloseModBus();
            ModBus.ModBus.ConfigDownloadScope("true", "false", "comtrade", "1999", "512", "4096", "Scope\\", "50");
            ModBus.ModBus.ConfigModBus("115200", "Odd", "One", "COM1");
            ModBus.ModBus.StartModBus();
            
            //Создаем модель сервера
            Server.Server.ConfigServer(@"test.cfg");

            //Запуск сервера 
            Server.Server.StartServer(Settings.Settings.ConfigServer.PortServer);
        }
    }
}