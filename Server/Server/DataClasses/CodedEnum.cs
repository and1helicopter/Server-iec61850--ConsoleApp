using System.Diagnostics.CodeAnalysis;

namespace ServerLib.DataClasses
{
	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public enum DoublePoint
	{
		INTERMEDIATE_STATE,
		OFF,
		ON,
		BAD_STATE
	}

	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public enum DirectionalProtection 
	{
		UNKNOWN,
		FORWARD,
		BACKWARD,
		BOTH
	}

	[SuppressMessage("ReSharper", "InconsistentNaming")]
	public enum SecurityViolation
	{
		UNKNOWN,
		CRITICAL,
		MAJOR,
		MINOR,
		WARNINING
	}
}
