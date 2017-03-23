using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ADSPLibrary
{
    static public class SystemParamsClass
    {
        static List<ushort> systemParams = new List<ushort>();
        public static List<ushort> SystemParams {set{} get{return (systemParams);}}
        public static bool UpdateSystemParams(ushort[] newSystemParams, ushort countParams)
        {
            ushort v = 0;
            try
            {
                v = newSystemParams[countParams - 1];
            }
            catch
            {
                return (false);
            }
            
            systemParams = new List<ushort>();
            for (int i = 0; i < countParams; i++)
            {
                systemParams.Add(newSystemParams[i]);
            }
            return true;
        }
        public static bool UpdateSystemParam(ushort paramNum, ushort paramValue)
        {
            try
            {
                systemParams[paramNum] = paramValue;
            }
            catch
            {
                return false;
            }
            return true;
        }
    }
}
