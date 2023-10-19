using System;
using Mono.Cecil;
using Orcus.Administration.Plugins;
using Orcus.Administration.Plugins.BuildPlugin;

namespace OrcusPatcher
{
	public class Plugin : BuildPluginBase
	{
		public override void Prepare(IBuilderArguments builderArguments)
		{
			builderArguments.SubscribeBuilderEvent(new ModifyAssemblyBuilderEvent(new ModifyAssemblyBuilderEvent.ModifyAssemblyDelegate(this.ModifyAssemblyDelegate)));
		}

		private void ModifyAssemblyDelegate(IBuilderInformation builderInformation, AssemblyDefinition assemblyDefinition)
		{
			builderInformation.BuildLogger.Status("Поиск Кода для Удаления!");
			bool flag = false;
			foreach (TypeDefinition typeDefinition in assemblyDefinition.Modules[0].Types)
			{
				if (typeDefinition.FullName == "Orcus.OrcusApplicationContext")
				{
					foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
					{
						if (methodDefinition.Name == ".ctor")
						{
							methodDefinition.Body.Instructions.RemoveAt(methodDefinition.Body.Instructions.Count - 2);
							methodDefinition.Body.Instructions.RemoveAt(methodDefinition.Body.Instructions.Count - 2);
							flag = true;
							break;
						}
					}
				}
			}
			if (!flag)
			{
				throw new PluginException("Patcher: Не найден лишний код для удаления!");
			}
			builderInformation.BuildLogger.Status("IL Код успешно удален!");
		}
	}
}