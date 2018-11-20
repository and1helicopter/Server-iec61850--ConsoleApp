using System.Linq;
using ServerLib.DataClasses;

namespace ServerLib.Parser
{
	public partial class Parser
	{
		#region Создание классов отвечающие за режимы
		private static bool DependencesModel()
		{
			try
			{
				//LLN0.Beh
				var baseBehDataObject = DataObj.UpdateListDestination.First(x =>
					x.NameDataObj.ToUpperInvariant().Contains("LLN0.Beh".ToUpperInvariant()));

				if (baseBehDataObject != null)
				{
					DataObj.ModHead.BaseBehDataObject = baseBehDataObject;						//Добавляю зависимый объект Beh в базовый к ModHead(LLN0)
				}

				//LLN0.Mod
				var baseModDataObject = DataObj.UpdateListDestination.First(x=> 
					x.NameDataObj.ToUpperInvariant().Contains("LLN0.Mod".ToUpperInvariant()));

				if (baseModDataObject != null)
				{
					baseModDataObject.WriteValueHandler += DataObj.ModHead.OnWriteValue;			//Подписываюсь на событие изменения статуса 

					var val = ((IncClass)baseModDataObject.BaseClass).stVal;

					DataObj.ModHead.BaseModDataObject = baseModDataObject;						//Добавляю зависимый объект Mod в базовый к ModHead(LLN0)
					DataObj.ModHead.OnWriteValue(new {Value = val, Key = "stVal" });				//Устанавлваю значения по-умолчанию
				}

				//Список Mod
				var modDataObjects = DataObj.UpdateListDestination.Where(x =>
					x.NameDataObj.ToUpperInvariant().Contains(".Mod".ToUpperInvariant()) && 
					!x.NameDataObj.ToUpperInvariant().Contains("LLN0.Mod".ToUpperInvariant())).ToList();

				if (modDataObjects.Count != 0)
				{
					modDataObjects.ForEach(mod =>
					{
						var text = mod.NameDataObj.ToUpperInvariant().Replace(".Mod".ToUpperInvariant(), "");

						var beh = DataObj.UpdateListDestination.First(z =>
							z.NameDataObj.ToUpperInvariant().Contains(text.ToUpperInvariant() + ".Beh".ToUpperInvariant()));

						var items = DataObj.UpdateListDestination.Where(y =>
							y.NameDataObj.ToUpperInvariant().Contains(text.ToUpperInvariant()) &&
							!y.NameDataObj.ToUpperInvariant().Contains(".Mod".ToUpperInvariant())).ToList();

						var modDependences = new DataObj.ModDependences
						{
							DependencesDataObjects = items,
							BaseModDataObject = mod,
							BaseBehDataObject = beh
						};

						mod.WriteValueHandler += modDependences.OnWriteValue;

						var val = ((IncClass)mod.BaseClass).stVal;
						modDependences.OnWriteValue(new { Value = val, Key = "stVal" });				//Устанавлваю значения по-умолчанию

						DataObj.ModHead.DependencesGroupes.Add(modDependences);
					});
				}

				//Список Health
				var baseHealthDataObject = DataObj.UpdateListDestination.First(x =>
					x.NameDataObj.ToUpperInvariant().Contains("LLN0.Health".ToUpperInvariant()));

				//Список Health
				var healthDataObject = DataObj.UpdateListDestination.Where(x =>
					x.NameDataObj.ToUpperInvariant().Contains("Health".ToUpperInvariant()) &&
					!x.NameDataObj.ToUpperInvariant().Contains("LLN0.Health".ToUpperInvariant())).ToList();

				if (baseHealthDataObject != null)
				{
					baseHealthDataObject.ReadValueHandler += DataObj.HealthHead.OnReadValue;

					var val = ((InsClass)baseHealthDataObject.BaseClass).stVal;
					DataObj.HealthHead.BaseHealthDataObject = baseHealthDataObject;

					DataObj.HealthHead.OnReadValue(new { Value = val, Key = "stVal" });
				}

				if (healthDataObject.Any())
				{
					healthDataObject.ForEach(health =>
					{
						var healthDependences = new DataObj.HealthDependences
						{
							BaseHealthDataObject = health
						};

						health.ReadValueHandler += healthDependences.OnReadValue;

						var val = ((InsClass)health.BaseClass).stVal;
						healthDependences.OnReadValue(new { Value = val, Key = "stVal" });                //Устанавлваю значения по-умолчанию

						DataObj.HealthHead.DependencesHealth.Add(healthDependences);
					});
				}

				return true;
			}
			catch
			{
				return false;
			}
		}
		#endregion
	}
}
