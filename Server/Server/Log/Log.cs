using System;
using System.IO;

namespace Server.Log
{
    public static class Log
    {
        public static void Write(string logMessage, string exaption)
        {
            using (StreamWriter w = File.AppendText("log.txt"))
            {
                Message(logMessage, exaption, w);
            }
        }

        private static void Message(string logMessage, string exaption, TextWriter w)
        {
            w.Write($"#{DateTime.Now}{DateTime.Now.Millisecond}: {exaption} - Message: \"{logMessage}\"\n");
            if (exaption.ToUpper() == "stop".ToUpper())
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