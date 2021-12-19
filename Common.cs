using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using InfinityScript;

namespace MapEdit
{
	public class Common
	{
		public static readonly int moneyfx = Function.Call<int>("loadfx", "props/cash_player_drop");
		public void ToggleFly(Entity player)
		{
			if (!player.HasField("fly") || player.GetField<string>("fly") != "true")
			{
				player.Call("allowspectateteam", new Parameter[]
				{
					"frelook",
					true
				});
				player.SetField("sessionstate", "spectator");
				player.Call("setcontents", new Parameter[]
				{
					0
				});
				player.SetField("fly", "true");
				//player.SetField("isDrawing", "false");
				return;
			}
			player.Call("allowspectateteam", new Parameter[]
			{
				"frelook",
				false
			});
			player.SetField("sessionstate", "playing");
			player.Call("setcontents", new Parameter[]
			{
				100
			});
			player.SetField("fly", "false");
		}

		public void RawSayToMultiline(Entity player, string[] message, int delay)
		{
			int num = 0;
			for (int i = 0; i < message.Length; i++)
			{
				string messagez2 = message[i];
				string messagez = messagez2;
				Main.Instance.Script.AfterDelay(num * delay, delegate
				{
					this.RawSayTo(player, messagez);
				});
				num++;
			}
		}

		public static void PlayFx(int fx, Vector3 origin)
		{
			Function.SetEntRef(-1);
			Function.Call("playfx", fx, origin);
		}

		public static void DropMoney(Vector3 origin)
		{
			PlayFx(moneyfx, origin);
		}

		public static Entity FindByName(string name)
		{

			int cont = 0;
			Entity player = null;
			foreach (Entity Player in Main.Instance.Script.Players)
			{
				if (0 <= Player.Name.IndexOf(name, StringComparison.InvariantCultureIgnoreCase))
				{
					player = Player;
					cont++;
				}
			}
			if (cont > 1) { return null; }
			if (cont == 1) { return player; }

			return null;
		}
		public static string GetPlayerTeam(Entity player)
		{
			return player.GetField<string>("sessionteam");
		}

		public void RawSayTo(Entity sender, string message)
		{
			Utilities.RawSayTo(sender, $"{ScriptAdmins.ClanTag}: ^7{message}");
		}

		public void RawSayToCondense(Entity player, string[] messages, int delay = 1000, int condenselevel = 40, string separator = ", ")
		{
			this.RawSayToMultiline(player, this.Condense(messages, condenselevel, separator), delay);
		}
		public static class WriteLog
		{
			public static void None(string message)
			{
				Log.Write(LogLevel.None, message);
			}
			public static void Info(string message)
			{
				Log.Write(LogLevel.Info, message);
			}

			public static void Error(string message)
			{
				Log.Write(LogLevel.Error, message);
			}

			public static void Warning(string message)
			{
				Log.Write(LogLevel.Warning, message);
			}

			public static void print(string s)
			{
				Log.Write(LogLevel.All, s);
			}
		}

		public static void fly(Entity player)
		{
			if (player.GetField<string>("sessionstate") != "spectator")
			{
				player.Call("allowspectateteam", new Parameter[]
				{
					"freelook",
					true
				});
				player.SetField("sessionstate", "spectator");
				player.Call("setcontents", new Parameter[]
				{
					0
				});
			}
			else
			{
				player.Call("allowspectateteam", new Parameter[]
				{
					"freelook",
					false
				});
				player.SetField("sessionstate", "playing");
				player.Call("setcontents", new Parameter[]
				{
					100
				});
			}
		}

		public static void SetSpeed(Entity player, float speed) => player.SetField("speed", speed);

		public static string FirstLetterToUpper(string str)
		{
			if (str == null)
				return null;

			if (str.Length > 1)
				return char.ToUpper(str[0]) + str.Substring(1);

			return str.ToUpper();
		}

		public static string _mapname()
		{
			string _mapname = Function.Call<string>("getdvar", new Parameter[]
			{
				"mapname"
			});
			return FirstLetterToUpper(_mapname.Replace("mp_", "").Replace("_cls", "").Replace("_ss", ""));
		}

		public static string _mapnameFull(bool full = false)
		{
			string _mapname = Function.Call<string>("getdvar", new Parameter[]
			{
				"mapname"
			});
			if (full == true)
			{
				return FirstLetterToUpper(_mapname.Replace("mp_", "").Replace("_cls", "").Replace("_ss", ""));
			}
			else
			{
				return _mapname;
			}
		}

		public static void WriteChatToAll(string message)
		{
			Utilities.RawSayAll(ScriptAdmins.ClanTag + " " + message);
		}

		public static void WriteChatToPlayer(Entity player, string message)
		{
			if (ScriptAdmins.ClanTag != "")
			{
				ScriptAdmins.ClanTag += " ";
				Utilities.RawSayTo(player, ScriptAdmins.ClanTag + "^7" + message);
			}
			else
			{
				Utilities.RawSayTo(player, message);
			}
		}

		public static string _svname()
		{
			string _svname = Function.Call<string>("getdvar", new Parameter[]
			{
				"sv_hostname"
			});
			return _svname.Replace("^1", "").Replace("^2", "").Replace("^3", "").Replace("^4", "").Replace("^5", "").Replace("^6", "").Replace("^7", "").Replace("^8", "").Replace("^9", "").Replace("^:", "").Replace("^;", "").Replace("^", "").Replace("", "");
		}

		public static bool IsNullOrEmpty(IDictionary Dictionary)
		{
			return (Dictionary == null || Dictionary.Count < 1);
		}


		public static string _mapnameSimple()
		{
			string _mapname = Function.Call<string>("getdvar", new Parameter[]
			{
				"mapname"
			});
			return _mapname;
		}

		public static int lines(string directory)
		{
			List<string> list = new List<string>();
			using (StreamReader streamReader = new StreamReader(directory))
			{
				string item;
				while ((item = streamReader.ReadLine()) != null)
				{
					list.Add(item);
				}
			}
			string[] array = list.ToArray();
			return array.Length;
		}

		public static bool EmptyFile(string directory)
		{
			return lines(directory) <= 0;
		}

		public string NoColor(string message)
		{
			for (int i = 0; i < 10; i++)
			{
				message = message.Replace("^" + i.ToString(), "").Replace("^:", "").Replace("^;", "");
			}
			return message;
		}
		public string[] Condense(string[] arr, int condenselevel = 40, string separator = ", ")
		{
			if (arr.Length < 1)
			{
				return arr;
			}
			List<string> list = new List<string>();
			int i = 0;
			string text = arr[i++];
			while (i < arr.Length)
			{
				if (this.NoColor(text + separator + arr[i]).Length > condenselevel)
				{
					list.Add(text);
					text = arr[i];
					i++;
				}
				else
				{
					text = text + separator + arr[i];
					i++;
				}
			}
			list.Add(text);
			return list.ToArray();
		}
	}
}
