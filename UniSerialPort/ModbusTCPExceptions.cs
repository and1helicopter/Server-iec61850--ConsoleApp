namespace UniSerialPort
{
    public enum ModbusTCPExceptions : byte
    {
        /// <summary>Constant for exception illegal function.</summary>
        ExcIllegalFunction = 1,
        /// <summary>Constant for exception illegal data address.</summary>
        ExcIllegalDataAdr = 2,
        /// <summary>Constant for exception illegal data value.</summary>
        ExcIllegalDataVal = 3,
        /// <summary>Constant for exception slave device failure.</summary>
        ExcSlaveDeviceFailure = 4,
        /// <summary>Constant for exception acknowledge.</summary>
        ExcAck = 5,
        /// <summary>Constant for exception slave is busy/booting up.</summary>
        ExcSlaveIsBusy = 6,
        /// <summary>Constant for exception gate path unavailable.</summary>
        ExcGatePathUnavailable = 10,
        /// <summary>Constant for exception not connected.</summary>
        ExcExceptionNotConnected = 253,
        /// <summary>Constant for exception connection lost.</summary>
        ExcExceptionConnectionLost = 254,
        /// <summary>Constant for exception response timeout.</summary>
        ExcExceptionTimeout = 255,
        /// <summary>Constant for exception wrong offset.</summary>
        ExcExceptionOffset = 128,
        /// <summary>Constant for exception send failt.</summary>
        ExcSendFailt = 100,
        /// <summary>Slave answer that incorrect format request.</summary>
        ExcRecieveSlaveException = 252,
    }
}
