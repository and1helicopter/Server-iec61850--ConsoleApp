using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using ServerLib.DataClasses;

namespace ServerLib.Parser
{
	public partial class Parser
	{
		private static bool CreateClassBitArray(XDocument doc)
		{
			try
			{
				if (doc.Root == null)
				{
					Log.Log.Write("FileParseToAttribute: doc.Root == null", "Warning ");
					return false;
				}

				IEnumerable<XElement> xLd = (from x in doc.Descendants()
					where String.Equals(x.Name.LocalName, "LDevice", StringComparison.InvariantCultureIgnoreCase)
					select x).ToList();

				if (!xLd.Any())
				{
					Log.Log.Write("FileParseToAttribute: LDevice == null", "Warning");
					return false;
				}

				foreach (var lditem in xLd)
				{
					if (lditem.Attribute("inst") == null)
					{
						Log.Log.Write("FileParseToAttribute: LDevice.inst == null", "Warning");
						continue;
					}

					IEnumerable<XElement> xLn = lditem.Elements().ToList();

					if (!xLn.Any())
					{
						Log.Log.Write("FileParseToAttribute: LN == null", "Warning");
						return false;
					}

					foreach (var lnitem in xLn)
					{
						if (String.Equals(lnitem.Attribute("type")?.Value, "EnergocomplektBitArray", StringComparison.InvariantCultureIgnoreCase) || String.Equals(lnitem.Attribute("type")?.Value, "BitArray", StringComparison.InvariantCultureIgnoreCase))
						{
							try
							{
								var nameBitArray = lnitem.Value.Split(';')[0];
								var addrBitArray = Convert.ToUInt16(lnitem.Value.Split(';')[1]);

								//Новый объект source
								UpdateDataObj.SourceList.Add(new UpdateDataObj.SourceClassDigital()
								{
									Addr = addrBitArray,
									Count = 1,
									NameBitArray = nameBitArray
								});

								continue;
							}
							catch
							{
								Log.Log.Write("FileParseToAttribute: LN.type == BitArray", "Warning");
								continue;
							}
						}

						if (lnitem.Attribute("lnClass") == null || lnitem.Attribute("inst") == null)
						{
							Log.Log.Write("FileParseToAttribute: LN.lnClass == null or LN.inst == null", "Warning");
						}
					}
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
