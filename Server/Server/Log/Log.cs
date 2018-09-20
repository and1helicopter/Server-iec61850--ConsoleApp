using System;
using System.IO;

namespace ServerLib.Log
{
	public static class Log
	{
		private static bool EnableLog = true;
		private static string root;

		public static void SetRootPath(string rootPath)
		{
			root = rootPath;
		}

		public static void Write(string logMessage, string exaption)
		{
			if (EnableLog)
			{
				try
				{
					using (StreamWriter w = File.AppendText(root != String.Empty ? $"{root}\\log.txt":"log.txt"))
					{
						Message(logMessage, exaption, w);
					}
				}
				catch
				{
					// ignored
				}
			}
		}

		private static void Message(string logMessage, string exaption, TextWriter w)
		{
			w.Write($"#{DateTime.Now}{DateTime.Now.Millisecond}: {exaption} - Message: \"{logMessage}\"\n");
			if (exaption.ToUpperInvariant() == "stop".ToUpperInvariant())
			{
				StopLine(w);
			}
		}

		private static void StopLine(TextWriter w)
		{
			w.Write("---------------------------------------------------------------\n");
		}
	}
}