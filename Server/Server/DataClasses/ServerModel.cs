using System.Collections.Generic;
using IEC61850.Common;
using IEC61850.Server;

namespace ServerLib.DataClasses
{
	public static class ServerModel
	{
		public static NodeModel Model;   //Модель сервера 

		public class NodeModel
		{
			public string NameModel { get; private set; }

			public List<NodeLD> ListLD = new List<NodeLD>();

			public void Clear()
			{
				Model.NameModel = "";
				Model.ListLD.Clear();
				ListTempLN.Clear();
				ListTempDO.Clear();
				ListTempDA.Clear();
				ListEnumType.Clear();
			}

			public NodeModel(string nameModel)
			{
				NameModel = nameModel;
			}
		}

		public class NodeLD  //LD - логические устройства
		{
			public string NameLD { get; }

			public List<NodeLN> ListLN = new List<NodeLN>();

			public NodeLD(string nameLd)
			{
				NameLD = nameLd;
			}
		}

		public static readonly List<NodeLN> ListTempLN = new List<NodeLN>(); //Логические узлы необходимые для формирования модели

		public class NodeLN  //LN - догические узлы
		{
			public string NameLN { get; }
			public string LnClassLN { get; }
			public string DescLN { get; set; }

			public List<NodeDO> ListDO = new List<NodeDO>();
			public List<DataSet> ListDS = new List<DataSet>();
			public List<RCB> ListRCB = new List<RCB>();
			public List<LCB> ListLCB = new List<LCB>();

			public NodeLN(string nameLN, string lnClassLN, string descLN)
			{
				NameLN = nameLN;
				LnClassLN = lnClassLN;
				DescLN = descLN;
			}
		}

		public static readonly List<NodeDO> ListTempDO = new List<NodeDO>(); //Объекты данных необходимые для формирования модели
		
		public class NodeDO  //DO - объекты данных
		{
			public string NameDO { get; }
			public string TypeDO { get; private set; }
			public string DescDO { get; }

			public List<NodeDA> ListDA = new List<NodeDA>();
			public List<NodeDO> ListDO = new List<NodeDO>();
			
			public NodeDO(string nameDO, string typeDO, string descDO)
			{
				NameDO = nameDO;
				TypeDO = typeDO;
				DescDO = descDO;
			}

			public void SetTypeDO(string typeDO)
			{
				TypeDO = typeDO;
			}
		}

		public class NodeDA  //DA - атрибуты данных
		{
			public string NameDA { get;  }
			public FunctionalConstraint FCDA { get; }
			public DataAttributeType BTypeDA { get; }
			public string TypeDA { get;  }
			public byte TrgOpsDA { get; }
			public string CountDA { get;  }
			public string Value { get; set; }

			public List<NodeDA> ListDA = new List<NodeDA>();

			public NodeDA(string nameDA, FunctionalConstraint fcDA, DataAttributeType bTypeDa, string typeDA, byte trgOpsDA, string countDA)
			{
				NameDA = nameDA;
				FCDA = fcDA;
				BTypeDA = bTypeDa;
				TypeDA = typeDA;
				TrgOpsDA = trgOpsDA;
				CountDA = countDA;
			}
		}

		public static List<TempDA> ListTempDA = new List<TempDA>();

		public class TempDA
		{
			public string NameTypeDA { get; private set; }

			public List<NodeDA> ListDA = new List<NodeDA>();

			public TempDA(string nameTypeDA)
			{
				NameTypeDA = nameTypeDA;
			}
		}

		public static readonly List<EnumType> ListEnumType = new List<EnumType>();

		public class EnumType
		{
			public string NameEnumType { get; }

			public List<EnumVal> ListEnumVal = new List<EnumVal>();

			public EnumType(string nameEnumType)
			{
				NameEnumType = nameEnumType;
			}

			public class EnumVal
			{
				public int OrdEnumVal { get; }
				public string ValEnumVal { get; }

				public EnumVal(int ordEnumVal, string valEnumVal)
				{
					OrdEnumVal = ordEnumVal;
					ValEnumVal = valEnumVal;
				}
			}
		}

		public class DataSet
		{
			public string DSName { get; }
			public string DSRef { get; }

			public List<string> DSMemberRef = new List<string>();

			public DataSet(string dsname, string dsref)
			{
				DSName = dsname;
				DSRef = dsref;
			}
		}

		public class RCB
		{
			public string RCBName { get; }
			public string RCBRef { get; }
			public ReportOptions RCBrptOptions { get; }
			public TriggerOptions RCBtrgOptions { get; }
			public string RCBbuffered { get; }
			public string RCBrptId { get; }
			public string RCBdatSet { get; }
			public uint RCBconfRev { get; }
			public uint RCBbufTime { get; }
			public uint RCBintgPd { get; }

			public RCB(string rcbName, string rcbRef, ReportOptions rcbrptOptions, TriggerOptions rcbtrgOptions, string rcbBuffered, string rcbrptId, string rcbdatSet, uint rcbconfRev, uint rcbbufTime, uint rcbintgPd)
			{
				RCBName = rcbName;
				RCBRef = rcbRef;
				RCBrptOptions = rcbrptOptions;
				RCBtrgOptions = rcbtrgOptions;
				RCBbuffered = rcbBuffered;
				RCBrptId = rcbrptId;
				RCBdatSet = rcbdatSet;
				RCBconfRev = rcbconfRev;
				RCBbufTime = rcbbufTime;
				RCBintgPd = rcbintgPd;
			}
		}

		public class LCB
		{
			public string LCBName { get; }
			public string LCBDatSet { get; }
			public string LCBRef { get; }			/* object reference to the journal. Defaults to <MMS DomainName>/<LNName>$GeneralLog */

			public TriggerOptions LCBtrgOptions { get; }		/* TrgOps - trigger conditions */
			public bool LCBLogEna { get; }								/* enable log by default */
			public uint LCBintgPd { get; }								/* IntgPd - integrity period */
			public bool LCBreasonCode { get; }						/* include reason code in log */

			public LCB(string name, string reference, bool logEna, string dataSet, TriggerOptions trgOptions, uint  intgPd, bool reasonCode)
			{
				LCBName = name;
				LCBRef = reference;
				LCBLogEna = logEna;
				LCBDatSet = dataSet;
				LCBtrgOptions = trgOptions;
				LCBintgPd = intgPd;
				LCBreasonCode = reasonCode;
			}
		}

		public static void Clear()
		{
			Model.Clear();

		}
	}
}

