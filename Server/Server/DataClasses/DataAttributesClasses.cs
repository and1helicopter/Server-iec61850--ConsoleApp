using System;
using IEC61850.Common;

namespace Server.DataClasses
{
    public class Quality
    {
        public ushort Validity { get; private set; }
        private DetailQuality DetailQual { get; }
        private string Source { get; }
        private bool Test { get; }
        private bool OperatorBlocked { get;  }
        private readonly int _allowedAge;

        public Quality()
        {
            Validity = 0;
            DetailQual = new DetailQuality();
            Source = "process";
            Test = false;
            OperatorBlocked = false;
            _dateValueOldUpdateDataObj = DateTime.Now;
            _allowedAge = 1000;
        }

        private class DetailQuality
        {
            public bool Overflow;
            public bool OutOfRange;
            public bool BadReference;
            public bool Oscillatory;
            public bool Failure;
            public bool OldData; 
            public bool Inconsistent; 
            public bool Inaccurate;

            public DetailQuality()
            {
                Overflow = false;
                OutOfRange = false;
                BadReference = false;
                Oscillatory = false;
                Failure = false;
                OldData = false;
                Inconsistent = false;
                Inaccurate = false;
            }

            public void UpdateOldData(bool status)
            {
                OldData = status;
            }

            public void UpdateBadReference(bool status)
            {
                BadReference = status;
            }
        }

        private DateTime _dateValueOldUpdateDataObj; 

        public void UpdateQuality(DateTime time)
        {
            int allowedAge = 1000; //допустимый возраст в мск

            UpdateOldData(_dateValueOldUpdateDataObj.AddMilliseconds(allowedAge) > time);
            _dateValueOldUpdateDataObj = time;

            //Проверки
            UpdateQualityClass();
        }

        public void QualityCheckClass(DateTime time)
        {
             //допустимый возраст в мск

            UpdateOldData(time.AddMilliseconds(_allowedAge) < DateTime.Now);
            UpdateBadReference(ModBus.ModBus.ErrorPort);

            //Проверки
            UpdateQualityClass();
        }

        private void UpdateBadReference(bool status)
        {
            DetailQual.UpdateBadReference(status);
        }


        private void UpdateOldData(bool status)
        {
            DetailQual.UpdateOldData(status);
        }

        private void UpdateQualityClass()
        {
            //Обновляем статус качества
            if (DetailQual.Overflow || DetailQual.BadReference || DetailQual.Oscillatory || DetailQual.Failure)
            {
                Validity = MapQuality("INVALID");
            }
            else if (DetailQual.OutOfRange || DetailQual.OldData || DetailQual.Inconsistent || DetailQual.Inaccurate)
            {
                Validity = MapQuality("QUESTIONABLE");
            }
            else
            {
                Validity = MapQuality("GOOD");
            }

            Validity = DetailQual.Overflow ? Convert.ToUInt16(Validity | 0x0004) : Convert.ToUInt16(Validity + 0x0004 ^ 0x0004);
            Validity = DetailQual.OutOfRange ? Convert.ToUInt16(Validity | 0x0008) : Convert.ToUInt16(Validity + 0x0008 ^ 0x0008);
            Validity = DetailQual.BadReference ? Convert.ToUInt16(Validity | 0x0010) : Convert.ToUInt16(Validity + 0x0010 ^ 0x0010);
            Validity = DetailQual.Oscillatory ? Convert.ToUInt16(Validity | 0x0020) : Convert.ToUInt16(Validity + 0x0020 ^ 0x0020);
            Validity = DetailQual.Failure ? Convert.ToUInt16(Validity | 0x0040) : Convert.ToUInt16(Validity + 0x0040 ^ 0x0040);
            Validity = DetailQual.OldData ? Convert.ToUInt16(Validity | 0x0080) : Convert.ToUInt16(Validity + 0x0080 ^ 0x0080);
            Validity = DetailQual.Inconsistent ? Convert.ToUInt16(Validity | 0x0100) : Convert.ToUInt16(Validity + 0x0100 ^ 0x0100);
            Validity = DetailQual.Inaccurate ? Convert.ToUInt16(Validity | 0x0200) : Convert.ToUInt16(Validity + 0x0200 ^ 0x0200);
            Validity = Source != "process" ? Convert.ToUInt16(Validity | 0x0400) : Convert.ToUInt16(Validity + 0x0400 ^ 0x0400);
            Validity = Test ? Convert.ToUInt16(Validity | 0x0800) : Convert.ToUInt16(Validity + 0x0800 ^ 0x0800);
            Validity = OperatorBlocked ? Convert.ToUInt16(Validity | 0x1000) : Convert.ToUInt16(Validity + 0x1000 ^ 0x1000);
        }

        private ushort MapQuality(string quality)
        {
            ushort qual = 0;
            switch (quality.ToUpper())
            {
                case @"GOOD":
                    qual = (ushort)IEC61850.Common.Validity.GOOD;
                    break;
                case @"INVALID":
                    qual = (ushort)IEC61850.Common.Validity.INVALID;
                    break;
                case @"RESERVED":
                    qual = (ushort)IEC61850.Common.Validity.RESERVED;
                    break;
                case @"QUESTIONABLE":
                    qual = (ushort)IEC61850.Common.Validity.QUESTIONABLE;
                    break;
            }
            return qual;
        }
    }

    public class AnalogueValueClass
    {
        public float f;

        public AnalogueValueClass()
        {
            f = 0;
        }
    }

    public class UnitClass
    {
        public int SIUnit;
        public int Multiplier;

        public UnitClass()
        {
            SIUnit = 0;
            Multiplier = 0;
        }
    }

    public class ScaledValueClass
    {
        public float ScaleFactor;
        public float Offset;

        public ScaledValueClass()
        {
            ScaleFactor = 1;
            Offset = 0;
        }
    }

    public class VectorClass
    {
        public AnalogueValueClass mag;
        public AnalogueValueClass ang;

        public VectorClass()
        {
            mag = new AnalogueValueClass();
            ang = new AnalogueValueClass();
        }
    }

	public class OriginatorClass
	{
		public Int32 orCat;
		public String orldent;

		public OriginatorClass(string strOrCat , string strorldent)
		{
			orCat = MapOrCat(strOrCat);
			orldent = strorldent;
		}

		private int MapOrCat(string strOrCat)
		{
			int answer = 0;
			switch (strOrCat.ToLower())
			{
				case @"not-supported":
					answer = (ushort)OrCat.NOT_SUPPORTED;
					break;
				case @"bay-control":
					answer = (ushort)OrCat.BAY_CONTROL;
					break;
				case @"station-control ":
					answer = (ushort)OrCat.STATION_CONTROL;
					break;
				case @"remote-control ":
					answer = (ushort)OrCat.REMOTE_CONTROL;
					break;
				case @"automatic-bay":
					answer = (ushort)OrCat.AUTOMATIC_BAY;
					break;
				case @"automatic-station":
					answer = (ushort)OrCat.AUTOMATIC_STATION;
					break;
				case @"automatic-remote":
					answer = (ushort)OrCat.AUTOMATIC_REMOTE;
					break;
				case @"maintenance":
					answer = (ushort)OrCat.MAINTENANCE;
					break;
				case @"process":
					answer = (ushort)OrCat.PROCESS;
					break;
			}

			return answer;
		}
	}

	public class CtlModelsClass
	{
		public int CtlModels;

		public CtlModelsClass(string ctlModels)
		{
			CtlModels = MapCtlModels(ctlModels);
		}

		private int MapCtlModels(string ctlModels)
		{
			var value = 0;
			switch (ctlModels.ToLower())
			{
				case @"status-only":
					value = (int) ControlModel.STATUS_ONLY;
					break;
				case @"direct-with-normal-security":
					value = (int)ControlModel.DIRECT_NORMAL;
					break;
				case @"sbo-with-normal-security":
					value = (int)ControlModel.SBO_NORMAL;
					break;
				case @"direct-with-enhanced-security":
					value = (int)ControlModel.DIRECT_ENHANCED;
					break;
				case @"sbo-with-enhanced-security":
					value = (int) ControlModel.SBO_ENHANCED;
					break;
				case @"0":
					value = (int)ControlModel.STATUS_ONLY;
					break;
				case @"1":
					value = (int)ControlModel.DIRECT_NORMAL;
					break;
				case @"2":
					value = (int)ControlModel.SBO_NORMAL;
					break;
				case @"3":
					value = (int)ControlModel.DIRECT_ENHANCED;
					break;
				case @"4":
					value = (int)ControlModel.SBO_ENHANCED;
					break;
			}
			return value;
		}
	}
}
