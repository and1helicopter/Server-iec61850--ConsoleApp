using System.Collections.Generic;
using System.Threading;

namespace ServerLib.ModBusTaskController
{
	public static class ModBusTaskController
	{
		public static class CycleClass
		{
			private static readonly List<MethodWork> ListMethodWorks = new List<MethodWork>();
			private static readonly object Locker = new object();
			

			internal static void AddMethodWork(MethodWork methodWork)
			{
				lock (Locker)
				{
					if(!ListMethodWorks.Contains(methodWork))
						ListMethodWorks.Add(methodWork);
				}
			}

			internal static void RemoveAllMethodWork()
			{
				lock (Locker) ListMethodWorks.Clear();
			}

			internal static void Cycle()
			{
				Thread.Sleep(1000);

				while (true)
				{
					lock (Locker)
					{
						var status = ModBus.ModBus.ModBusPort.IsEmpty || ModBus.ModBus.ModBusPort.IsError;

						ListMethodWorks.ForEach(methodWork =>
						{
							methodWork.Request(status);
						});
					}

					Thread.Sleep(1);
				}
			}

			internal abstract class MethodWork
			{
				internal abstract void Request(dynamic status);
				internal abstract void Response(dynamic value, dynamic param, bool status);
			}

			public class ResponseObj
			{
				internal dynamic Item { get; set; }
				internal dynamic Response { get; set; }
			}
		}
	}
}
