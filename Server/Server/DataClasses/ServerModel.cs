using System.Collections.Generic;

namespace Server.DataClasses
{
    public static class ServerModel
    {
        public static NodeModel Model;      //Модель сервера 

        public class NodeModel
        {
            public string NameModel { get; }

            public List<NodeLD> ListLD = new List<NodeLD>();

            public NodeModel(string nameModel)
            {
                NameModel = nameModel;
            }
        }

        public class NodeLD     //LD - логические устройства
        {
            public string NameLD { get; }

            public List<NodeLN> ListLN = new List<NodeLN>();

            public NodeLD(string nameLd)
            {
                NameLD = nameLd;
            }
        }

        public static readonly List<NodeLN> ListTempLN = new List<NodeLN>();    //Логические узлы необходимые для формирования модели

        public class NodeLN     //LN - догические узлы
        {
            public string NameLN { get; }
            public string LnClassLN { get; }
            public string DescLN { get; set; }

            public List<NodeDO> ListDO = new List<NodeDO>();

            public NodeLN(string nameLN, string lnClassLN, string descLN)
            {
                NameLN = nameLN;
                LnClassLN = lnClassLN;
                DescLN = descLN;
            }
        }

        public static readonly List<NodeDO> ListTempDO = new List<NodeDO>();    //Объекты данных необходимые для формирования модели
        
        public class NodeDO     //DO - объекты данных
        {
            public string NameDO { get; }
            public string TypeDO { get; }
            public string DescDO { get; }

            public List<NodeDA> ListDA = new List<NodeDA>();

            //Служебная информация
            public string Type { get; set; }
            public string Format { get; set; }
            public ushort Mask { get; set; }
            public ushort Addr { get; set; }
            
            public NodeDO(string nameDO, string typeDO, string descDO)
            {
                NameDO = nameDO;
                TypeDO = typeDO;
                DescDO = descDO;
            }
        }

        public class NodeDA     //DA - атрибуты данных
        {
            public string NameDA { get;  }
            public string FCDA { get; }
            public string BTypeDA { get; }
            public string TypeDA { get;  }
            public byte TrgOpsDA { get; }
            public string CountDA { get;  }
            public string Value { get; set; }

            public List<NodeDA> ListDA = new List<NodeDA>();

            public NodeDA(string nameDA, string fcDA, string bTypeDa, string typeDA, byte trgOpsDA, string countDA)
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
    }
}
