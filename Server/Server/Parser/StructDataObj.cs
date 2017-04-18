using System;
using System.Collections.Generic;

namespace Server.Parser
{
    public static class StructDataObj
    {
        public static List<DataObj> structDataObj = new List<DataObj>();

        public class DataObj
        {
            public string NameDataObj { get; private set; }
            public ushort AddrDataObj { get; private set; }
            public string TypeDataObj { get; private set; }
            public string MaskDataObj { get; private set; }
            public string ConvertDataObj { get; private set; }
            public string ClassDataObj { get; private set; }
            public long ValueDataObj { get; private set; }
            public bool SendRequestDataObj { get; private set; }
            public DateTime DateValueUpdateDataObj { get; private set; }

            public DataObj(string name, ushort addr, string type, string mask, string convert, string _class)
            {
                NameDataObj = name;
                AddrDataObj = addr;
                TypeDataObj = type;
                MaskDataObj = mask;
                ConvertDataObj = convert;
                ClassDataObj = _class;
            }

            public void SetValueDataObj(long value)
            {
                ValueDataObj = value;
            }

            public void SetSendRequestDataObj(bool request)
            {
                SendRequestDataObj = request;
            }

            public void SetDateValueUpdateDataObj(DateTime date)
            {
                DateValueUpdateDataObj = date;
            }
        }

        public static void AddStructDataObj(string name, ushort addr, string type, string mask, string convert, string _class)
        {
            DataObj dataObj = new DataObj(name, addr, type, mask, convert, _class);
            structDataObj.Add(dataObj);
        }

        public static void RemoveDataObj(int index)
        {
            if(index >= structDataObj.Count) { return;}
            structDataObj.RemoveAt(index); 
        }

        public static void ClearDataObj()
        {
            structDataObj.Clear();
        }
    }

    public class StructDefultDataObj
    {
        public static List<DefultDataObj> structDefultDataObj = new List<DefultDataObj>();

        public class DefultDataObj
        {
            public string IED { get; private set; }
            public string LDevice { get; private set; }
            public string LN { get; private set; }
            public string DOI { get; private set; }
            public string DAI { get; private set; }
            public string Value { get; private set; }

            public DefultDataObj(string ied, string ld, string ln, string doi, string dai, string value)
            {
                IED = ied;
                LDevice = ld;
                LN = ln;
                DOI = doi;
                DAI = dai;
                Value = value;
            }
        }

        public static void AddStructDefultDataObj(string ied, string ld, string ln, string doi, string dai, string value)
        {
            DefultDataObj dataObj = new DefultDataObj(ied, ld, ln, doi, dai, value);
            structDefultDataObj.Add(dataObj);
        }

        public static void RemoveDefultDataObj(int index)
        {
            if (index >= structDefultDataObj.Count) { return; }
            structDefultDataObj.RemoveAt(index);
        }

        public static void ClearDefultDataObj()
        {
            structDefultDataObj.Clear();
        }

    }
    
    public static class StructModelObj
    {
        public static NodeModel Model;

        public class NodeModel
        {
            public string NameModel { get; private set; }
            public List<NodeLD> ListLD= new List<NodeLD>();

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

        public static List<NodeDO> ListTempDO= new List<NodeDO>();

        public class NodeDO
        {
            public string NameDO { get; private set; }
            public string TypeDO { get; private set; }
            public string DescDO { get; set; }

            public List<NodeDA> ListDA = new List<NodeDA>();

            public NodeDO(string nameDO , string typeDO, string descDO)
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

            public NodeDA(string nameDA, string fcDA,string bTypeDa, string typeDA, byte trgOpsDA, string countDA)
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
