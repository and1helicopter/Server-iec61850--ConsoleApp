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
            private static volatile bool work = false;

            private static bool GetWork()
            {
                bool status = false;
                lock (Locker)
                {
                    status = work;
                }
                return status;
            }

            public static void SetWork(bool status)
            {
                lock (Locker)
                {
                    work = status;
                }
            }
			

			internal static void AddMethodWork(MethodWork methodWork)
			{
				lock (Locker)
				{
					if (!ListMethodWorks.Contains(methodWork))
					{
						ListMethodWorks.Add(methodWork);
					}
						
				}
			}

			internal static void RemoveAllMethodWork()
			{
				lock (Locker) ListMethodWorks.Clear();
			}

			internal static void Cycle()
			{
                //var IsStart = ModBus.ModBus.StartModBus();

                //if (!IsStart) return;
                SetWork(true);
                while (GetWork())
				{
					lock (ModBus.ModBus.Locker)
					{
                        var status = System.DateTime.Now > ModBus.ModBus.ModBusPort.SuccessRead.AddMilliseconds(1000);

                        var start = ModBus.ModBus.ModBusPort.SerialPort.IsOpen && !ModBus.ModBus.ModBusPort.SerialPort.PortError;

                        if (start)
						{
							ListMethodWorks.ForEach(methodWork => { methodWork.Request(status); });
						}
                        
                        // ModBus.ModBus.CheckPort();

                        //else ModBus.ModBus.StartModBus();
                    }

                    // Кладем спать 
                    Thread.Sleep(1);
				}

                ModBus.ModBus.CloseModBus();

            }

            internal abstract class MethodWork
			{
				internal abstract int MaxCountRequest { get; set; }
				internal abstract int RequestCount { get; set; }
				internal abstract void Request(dynamic restart);
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
