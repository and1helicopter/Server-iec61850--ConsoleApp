using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ModBusLibrary
{
    public class ModBusDataClass
    {
        byte requestType = 0;
        public byte RequestType
        {
            get { return requestType; }
            set { if (value != 0) { requestType = 1; } else { requestType = 0; } }
        }

        int tag = 0;
        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        ushort startAddr = 0;
        public ushort StartAddr
        {
            get { return startAddr; }
            set { if (value < 0x4000) { startAddr = value; } }
        }

        bool requestError = false;
        public bool RequestError
        {
            get { return requestError; }
            set { requestError = value; }
        }

        ushort[] readData = new ushort[32];
        public ushort[] ReadData
        {
            get { return readData; }
            set { readData = value; }
        }

        public ushort readCount = 0;
        public ushort ReadCount
        {
            get { return readCount; }
            set { if ((value < 33) && ((value + startAddr) < 0x4000)) { readCount = value; } }
        }

        ushort[] writeData = new ushort[32];
        public ushort[] WriteData
        {
            get { return writeData; }
            set { writeData = value; }
        }

        ushort writeCount = 0;
        public ushort WriteCount
        {
            get { return writeCount; }
            set { if ((value < 33) && ((value + startAddr) < 0x4000)) { writeCount = value; } }
        }

        public ModBusDataClass(int newTag)
        {
            tag = newTag;
        }
    }

}
