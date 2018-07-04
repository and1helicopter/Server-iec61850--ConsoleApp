using System;

namespace Server.DataClasses
{
	public class SpsSignature
	{
		public DateTime Time { get; set; }
		public Boolean Value { get; set; }

		public SpsSignature(DateTime time, Boolean value)
		{
			Time = time;
			Value = value;
		}
	}

	public class InsSignature
	{
		public DateTime Time { get; set; }
		public Int32 Value { get; set; }

		public InsSignature(DateTime time, Int32 value)
		{
			Time = time;
			Value = value;
		}
	}

	public class ActSignature
	{
		public DateTime Time { get; set; }
		public Boolean Value { get; set; }

		public ActSignature(DateTime time, Boolean value)
		{
			Time = time;
			Value = value;
		}
	}

	public class BcrSignature
	{
		public DateTime Time { get; set; }
		public Int32 Value { get; set; }

		public BcrSignature(DateTime time, Int32 value)
		{
			Time = time;
			Value = value;
		}
	}

	public class MvSignature
	{
		public DateTime Time { get; set; }
		public Int64 Value { get; set; }

		public MvSignature(DateTime time, Int64 value)
		{
			Time = time;
			Value = value;
		}
	}

	public class CmvSignature
	{
		public DateTime time { get; set; }
		public Int64 valueMag { get; set; }
		public Int64 valueAng { get; set; }
	}

	public class SpcSignature
	{
		public DateTime Time { get; set; }
		public Boolean StValue { get; set; }

		public SpcSignature(DateTime time, Boolean stValue)
		{
			Time = time;
			StValue = stValue;
		}
	}

	public class IncSignature
	{
		public DateTime Time { get; set; }
		public Int32 StValue { get; set; }

		public IncSignature(DateTime time, Int32 stValue)
		{
			Time = time;
			StValue = stValue;
		}
	}
}
