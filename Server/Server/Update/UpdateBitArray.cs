using System;
using System.Collections;

namespace Server.Update
{
	public static partial class UpdateDataObj
	{
		public class BitArrayObj
		{
			/// <summary>  Имя битового поля</summary>
			public string NameBitArray { get; }
			/// <summary>  Битовое поле</summary>
			public BitArray BitArray { get; private set; }

			/// <summary>  Инициалтзация класса битовго поля</summary>
			public BitArrayObj(string name, ushort value)
			{
				NameBitArray = name;
				BitArray = new BitArray(value);
			}

			/// <summary>  Установить в BitArray ushort</summary>
			public void GetBitArrayObj(ushort value)
			{
				BitArray = new BitArray(value);
			}

			/// <summary>  Преобразовать BitArrayObj к ushort</summary>
			public ushort SetBitArrayObj()
			{
				return Convert.ToUInt16(BitArray);
			}

			/// <summary> Установить бит index в значение value </summary>
			public void SetBit(ushort index, bool value)
			{
				BitArray.Set(index, value);
			}


		}
	}
}
