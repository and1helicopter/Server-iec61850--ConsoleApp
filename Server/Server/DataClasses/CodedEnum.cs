﻿namespace ServerLib.DataClasses
{
	public enum DoublePoint
	{
		INTERMEDIATE_STATE,
		OFF,
		ON,
		BAD_STATE
	}

	public enum DirectionalProtection 
	{
		UNKNOWN,
		FORWARD,
		BACKWARD,
		BOTH
	}

	public enum SecurityViolation
	{
		UNKNOWN,
		CRITICAL,
		MAJOR,
		MINOR,
		WARNINING
	}
}
