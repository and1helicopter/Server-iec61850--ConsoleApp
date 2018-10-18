using System.Collections.Generic;
using System.Threading;

namespace ServerLib.Update
{
	internal static class UpdateClass
	{
		internal static class CycleClass
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

			internal static void Cycle()
			{
				while (true)
				{
					lock (Locker)
					{
						ListMethodWorks.ForEach(methodWork =>
						{
							var status = ModBus.ModBus.Status.IsEmpty || ModBus.ModBus.Status.IsError;
							methodWork.Request(status);
						});
					}

					Thread.Sleep(10);
				}
			}

			internal abstract class MethodWork
			{
				internal abstract void Request(dynamic status);
				internal abstract void Response(dynamic value, dynamic param, bool status);
			}
		}
	}
}
