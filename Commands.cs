using System;
using System.Collections.Generic;
using InfinityScript;

namespace MapEdit
{
	public class Commands
	{
		internal string Name;

		internal string Usage;
		internal Action<Entity, string[]> Run;

		internal static List<Commands> CommandList = new List<Commands>();

		internal bool Active;

		internal List<string> Alias;
		public Commands()
		{
		}

		public Commands(string Command_Name, Action<Entity, string[]> run, string usage = null, bool IsActive = true, List<string> alias = null)
		{
			try
			{
				if (this.Exists(Command_Name))
				{
					Log.Error("The Command Already Exists With Name = " + Command_Name);
				}
				else
				{
					Commands item = new Commands
					{
						Name = Command_Name,
						Run = run,
						Usage = usage,
						Active = IsActive,
						Alias = alias
					};
					Commands.CommandList.Add(item);
				}
			}
			catch
			{
				Log.Error("Error when creating the command");
			}
		}

		internal bool Exists(string CommandName)
		{
			using (List<Commands>.Enumerator enumerator = Commands.CommandList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Name.ToLower() == CommandName.ToLower())
					{
						return true;
					}
				}
			}
			return false;
		}


		internal void AddCommand(string Command_Name, Action<Entity, string[]> Run, string Usage = null, List<string> Alias = null)
		{
			try
			{
				new Commands(Command_Name, Run, Usage, true, Alias);
			}
			catch
			{
				Log.Error("Something wrong with AddCommand");
			}
		}
	}
}
