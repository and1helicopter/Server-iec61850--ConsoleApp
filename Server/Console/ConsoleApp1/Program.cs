using System;
using System.IO;
using ServerLib.Log;
using ServerLib.Server;

namespace ConsoleApp1
{
	class Program
	{
		static void Main(string[] args)
		{
			var pathDirectory = Directory.GetCurrentDirectory();
			//Установка пути для лог файла
			Log.SetRootPath(pathDirectory);

			//Открываем настройки сервера
			if (!ServerIEC61850.ReadConfig(pathDirectory + "\\Settings.xml"))
			{
				Log.Write(@"Settings: ReadSettings finish with status false. Stop server", @"Error");
			}

			if (!ServerIEC61850.ParseFile())
			{
				Log.Write(@"ParseFile: Finish with status false. Stop server", @"Error");
				return;
			}
			Log.Write(@"ParseFile: File parse success", @"Success");

			//Создаем модель сервера
			if (!ServerIEC61850.ConfigServer()) return;

			//Запуск сервера 
			if (!ServerIEC61850.StartServer()) return;


			// The code provided will print ‘Hello World’ to the console.
			// Press Ctrl+F5 (or go to Debug > Start Without Debugging) to run your app.
			Console.WriteLine("Hello World!");
			Console.ReadLine();

			// Go to http://aka.ms/dotnet-get-started-console to continue learning how to build a console app! 
		}
	}
}
