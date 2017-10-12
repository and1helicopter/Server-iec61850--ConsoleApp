using Server.DataClasses;

namespace Server.Parser
{
	public partial class Parser
	{
		private static void ModelParseToUpdateStatus()
		{
			foreach (var listLD in ServerModel.Model.ListLD)
			{
				ParseListLD(listLD);
			}
		}

		private static void ParseListLD(ServerModel.NodeLD listLD)
		{
			foreach (var listLN in listLD.ListLN)
			{
				string path = listLD.NameLD;
				ParseListLN(listLN, path);
			}
		}

		private static void ParseListLN(ServerModel.NodeLN listLN, string path)
		{
			foreach (var itemDO in listLN.ListDO)
			{
				string newPath = path + "/" + listLN.NameLN;
			   ParseDO(itemDO, newPath);
			}
		}

		private static void ParseDO(ServerModel.NodeDO itemDO, string path)
		{
			if (itemDO.NameDO.ToUpper() == @"Mod".ToUpper())
			{
				Update.UpdateDataObj.ModClass.PathModList.Add(path +  "." + itemDO.NameDO); 
			}
			else if (itemDO.NameDO.ToUpper() == @"Beh".ToUpper())
			{
				Update.UpdateDataObj.BehClass.PathBehList.Add(path + "." + itemDO.NameDO);

			}
			else if (itemDO.NameDO.ToUpper() == @"Health".ToUpper())
			{
				Update.UpdateDataObj.HealthClass.PathHealthList.Add(path + "." + itemDO.NameDO);

			}
			else if (itemDO.NameDO.ToUpper() == @"PhyHealth".ToUpper())
			{
				Update.UpdateDataObj.HealthClass.PathPhyHealthList.Add(path + "." + itemDO.NameDO);

			}
		}
	}
}
