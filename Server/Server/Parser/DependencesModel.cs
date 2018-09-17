using System.Linq;
using ServerLib.DataClasses;
using ServerLib.Update;

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
				var baseBehDataObject = UpdateDataObj.UpdateListDestination.First(x =>
					x.NameDataObj.ToUpperInvariant().Contains("LLN0.Beh".ToUpperInvariant()));

				if (baseBehDataObject != null)
				{
					UpdateDataObj.ModHead.BaseBehDataObject = baseBehDataObject;						//Добавляю зависимый объект Beh в базовый к ModHead(LLN0)
				}

				//LLN0.Mod
				var baseModDataObject = UpdateDataObj.UpdateListDestination.First(x=> 
					x.NameDataObj.ToUpperInvariant().Contains("LLN0.Mod".ToUpperInvariant()));

				if (baseModDataObject != null)
				{
					baseModDataObject.WriteValueHandler += UpdateDataObj.ModHead.OnWriteValue;			//Подписываюсь на событие изменения статуса 

					var val = ((IncClass)baseModDataObject.BaseClass).stVal;

					UpdateDataObj.ModHead.BaseModDataObject = baseModDataObject;						//Добавляю зависимый объект Mod в базовый к ModHead(LLN0)
					UpdateDataObj.ModHead.OnWriteValue(new {Value = val, Key = "stVal" });				//Устанавлваю значения по-умолчанию
				}

				//Список Mod
				var modDataObjects = UpdateDataObj.UpdateListDestination.Where(x =>
					x.NameDataObj.ToUpperInvariant().Contains(".Mod".ToUpperInvariant()) && 
					!x.NameDataObj.ToUpperInvariant().Contains("LLN0.Mod".ToUpperInvariant())).ToList();

				if (modDataObjects.Count != 0)
				{
					modDataObjects.ForEach(mod =>
					{
						var text = mod.NameDataObj.ToUpperInvariant().Replace(".Mod".ToUpperInvariant(), "");

						var beh = UpdateDataObj.UpdateListDestination.First(z =>
							z.NameDataObj.ToUpperInvariant().Contains(text.ToUpperInvariant() + ".Beh".ToUpperInvariant()));

						var items = UpdateDataObj.UpdateListDestination.Where(y =>
							y.NameDataObj.ToUpperInvariant().Contains(text.ToUpperInvariant()) &&
							!y.NameDataObj.ToUpperInvariant().Contains(".Mod".ToUpperInvariant())).ToList();

						var modDependences = new UpdateDataObj.ModDependences
						{
							DependencesDataObjects = items,
							BaseModDataObject = mod,
							BaseBehDataObject = beh
						};

						mod.WriteValueHandler += modDependences.OnWriteValue;

						var val = ((IncClass)mod.BaseClass).stVal;
						modDependences.OnWriteValue(new { Value = val, Key = "stVal" });				//Устанавлваю значения по-умолчанию

						UpdateDataObj.ModHead.DependencesGroupes.Add(modDependences);
					});
				}

				//Список Health
				var baseHealthDataObject = UpdateDataObj.UpdateListDestination.First(x =>
					x.NameDataObj.ToUpperInvariant().Contains("LLN0.Health".ToUpperInvariant()));

				//Список Health
				var healthDataObject = UpdateDataObj.UpdateListDestination.Where(x =>
					x.NameDataObj.ToUpperInvariant().Contains("Health".ToUpperInvariant()) &&
					!x.NameDataObj.ToUpperInvariant().Contains("LLN0.Health".ToUpperInvariant())).ToList();

				if (baseHealthDataObject != null)
				{
					baseHealthDataObject.ReadValueHandler += UpdateDataObj.HealthHead.OnReadValue;

					var val = ((InsClass)baseHealthDataObject.BaseClass).stVal;
					UpdateDataObj.HealthHead.BaseHealthDataObject = baseHealthDataObject;

					UpdateDataObj.HealthHead.OnReadValue(new { Value = val, Key = "stVal" });
				}

				if (healthDataObject.Any())
				{
					healthDataObject.ForEach(health =>
					{
						var healthDependences = new UpdateDataObj.HealthDependences
						{
							BaseHealthDataObject = health
						};

						health.ReadValueHandler += healthDependences.OnReadValue;

						var val = ((InsClass)health.BaseClass).stVal;
						healthDependences.OnReadValue(new { Value = val, Key = "stVal" });                //Устанавлваю значения по-умолчанию

						UpdateDataObj.HealthHead.DependencesHealth.Add(healthDependences);
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
