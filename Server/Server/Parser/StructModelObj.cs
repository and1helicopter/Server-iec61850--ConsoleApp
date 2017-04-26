﻿using System.Collections.Generic;

namespace Server.Parser
{
    public static class StructModelObj
    {
        public static NodeModel Model;

        public class NodeModel
        {
            public string NameModel { get; private set; }
            public List<NodeLD> ListLD = new List<NodeLD>();

            public NodeModel(string nameModel)
            {
                NameModel = nameModel;
            }

            public void AddListLD(NodeLD LD)
            {
                ListLD.Add(LD);
            }
        }

        public class NodeLD
        {
            public string NameLD { get; private set; }
            public List<NodeLN> ListLN = new List<NodeLN>();

            public NodeLD(string nameModel)
            {
                NameLD = nameModel;
            }
        }

        public static List<NodeLN> ListTempLN = new List<NodeLN>();

        public class NodeLN
        {
            public string NameLN { get; private set; }
            public string LnClassLN { get; private set; }
            public string DescLN { get; set; }

            public List<NodeDO> ListDO = new List<NodeDO>();

            public NodeLN(string nameLN, string lnClassLN, string descLN)
            {
                NameLN = nameLN;
                LnClassLN = lnClassLN;
                DescLN = descLN;
            }

            public void AddDOToLN(NodeDO DO)
            {
                ListDO.Add(DO);
            }
        }

        public static List<NodeDO> ListTempDO = new List<NodeDO>();

        public class NodeDO
        {
            public string NameDO { get; private set; }
            public string TypeDO { get; private set; }
            public string DescDO { get; set; }

            //Служебная информация
            public string Type { get; private set; }
            public ushort Addr { get; private set; }

            public List<NodeDA> ListDA = new List<NodeDA>();

            public NodeDO(string nameDO, string typeDO, string descDO)
            {
                NameDO = nameDO;
                TypeDO = typeDO;
                DescDO = descDO;
            }

            public void AddDAToDA(NodeDA DA)
            {
                ListDA.Add(DA);
            }
        }

        public class NodeDA
        {
            public string NameDA { get; private set; }
            public string FCDA { get; private set; }
            public string BTypeDA { get; private set; }
            public string TypeDA { get; private set; }
            public byte TrgOpsDA { get; private set; }
            public string CountDA { get; private set; }
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

        public static List<TypeDA> ListTempDA = new List<TypeDA>();

        public class TypeDA
        {
            public string NameTypeDA { get; private set; }

            public List<NodeDA> ListDA = new List<NodeDA>();

            public TypeDA(string nameTypeDA)
            {
                NameTypeDA = nameTypeDA;
            }
        }

        public static List<EnumType> ListEnumType = new List<EnumType>();

        public class EnumType
        {
            public string NameEnumType { get; private set; }

            public List<EnumVal> ListEnumVal = new List<EnumVal>();

            public EnumType(string nameEnumType)
            {
                NameEnumType = nameEnumType;
            }

            public class EnumVal
            {
                public int OrdEnumVal { get; private set; }
                public string ValEnumVal { get; private set; }

                public EnumVal(int ordEnumVal, string valEnumVal)
                {
                    OrdEnumVal = ordEnumVal;
                    ValEnumVal = valEnumVal;
                }
            }
        }
    }
}