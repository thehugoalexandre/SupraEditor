using System;
using System.Collections.Generic;
using System.Linq;
using InfinityScript;

namespace MapEdit
{
	public class CommandManager
	{
		public Commands Commands;
		private int m_pos;

		public CommandManager()
		{
			this.Commands = new Commands();
			this.AddCommand("/help", delegate (Entity sender, string[] args)
			{
				if (args.Length < 2)
				{
					List<Commands> source = (from x in Commands.CommandList where x.Active select x).ToList<Commands>();
					Main.Instance.Common.RawSayToCondense(sender, (from x in source select x.Name).ToArray<string>(), 2000, 40, ", ");
					return;
				}
				List<Commands> list = (from x in Commands.CommandList where x.Active && x.Name.ToLower().Contains(args[1].ToLower()) select x).ToList<Commands>();
				if (list.Count == 0)
				{
					Main.Instance.Common.RawSayTo(sender, "^1Command not found");
				}
				if (list.Count > 1)
				{
					Main.Instance.Common.RawSayTo(sender, "^5More than one command exists:");
					Main.Instance.Common.RawSayToCondense(sender, (from x in list select x.Name).ToArray<string>(), 2000, 40, ", ");
					return;
				}
				Main.Instance.Common.RawSayTo(sender, "^5Command: " + list.First<Commands>().Name);
				Main.Instance.Common.RawSayTo(sender, "^5Usage: " + list.First<Commands>().Usage);
			}, "/help [command]", null);
			this.AddCommand("removeedit", delegate (Entity sender, string[] args)
			{
				if (args.Length < 2)
				{
					Main.Instance.Common.RawSayTo(sender, "^1Usage: !removeedit [<id> | last]");
					return;
				}
				if (args[1] == "last")
				{
					Main.Instance.ScriptModel.Remove(Main.Instance.ScriptModel.ModelsList.Last<ScriptModel.ModelInfo>());
					Main.Instance.Common.RawSayTo(sender, "^2Last edit has been removed");
					return;
				}
				int id;
				if (int.TryParse(args[1], out id))
				{
					Main.Instance.ScriptModel.Remove(id);
					Main.Instance.Common.RawSayTo(sender, "^2Edit with id " + id.ToString() + " has been removed");
					return;
				}
				Main.Instance.Common.RawSayTo(sender, "^1Usage: !removeedit [<id> | last]");
			}, "re [<id> | last]", new List<string>
			{
				"re"
			});
			this.AddCommand("addadmin", delegate (Entity sender, string[] args)
			{
				if (args.Length < 2)
				{
					Main.Instance.Common.RawSayTo(sender, "^1Usage: !add <target>");
					return;
				}
				List<Entity> list = (from x in Main.Instance.Script.Players where x.Name.ToLower().Contains(args[1]) select x).ToList<Entity>();
				if (list.Count != 1)
				{
					Main.Instance.Common.RawSayTo(sender, "^1No or more players found");
					return;
				}
				Entity entity = list.First<Entity>();
				if (Main.Instance.ScriptAdmins.Exists(entity.HWID, false))
				{
					Main.Instance.Common.RawSayTo(sender, "^1Player is already in admins list");
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (args.Length > 2)
				{
					foreach (string text in args.Skip(2))
					{
						dictionary[text.Split(new char[]
						{
							':'
						})[0]] = text.Split(new char[]
						{
							':'
						})[1];
					}
				}
				Main.Instance.ScriptAdmins.Add(entity.Name, entity.HWID, dictionary);
				Main.Instance.Common.RawSayTo(sender, "^1Player has been added to admins");
				entity.SetField("IsAdmin", "true");
				Main.Instance.Common.RawSayTo(entity, "You have been added to SupraEditor admins list by ^2" + sender.Name);
			}, "add <target>", new List<string>
			{
				"add"
			});
			this.AddCommand("addtomembers", delegate (Entity sender, string[] args)
			{
				if (args.Length < 2)
				{
					Main.Instance.Common.RawSayTo(sender, "^1Usage: !add <target>");
					return;
				}
				List<Entity> list = (from x in Main.Instance.Script.Players where x.Name.ToLower().Contains(args[1]) select x).ToList<Entity>();
				if (list.Count != 1)
				{
					Main.Instance.Common.RawSayTo(sender, "^1No or more players found");
					return;
				}
				Entity entity = list.First<Entity>();
				if (Main.Instance.ScriptAdmins.Exists2(entity.HWID, false))
				{
					Main.Instance.Common.RawSayTo(sender, "^1Player is already in members list");
					return;
				}
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				if (args.Length > 2)
				{
					foreach (string text in args.Skip(2))
					{
						dictionary[text.Split(new char[]
						{
							':'
						})[0]] = text.Split(new char[]
						{
							':'
						})[1];
					}
				}
				Main.Instance.ScriptAdmins.AddToMembers(entity.Name, entity.HWID);
				Main.Instance.Common.RawSayTo(sender, "^1Player has been added to members");
				entity.SetField("IsAdmin", "true");
				Main.Instance.Common.RawSayTo(entity, "You have been added to SupraEditor Members list by ^2" + sender.Name);
			}, "add <target>", new List<string>
			{
				"addtom"
			});
			this.AddCommand("removeadmin", delegate (Entity sender, string[] args)
			{
				if (args.Length < 2)
				{
					Main.Instance.Common.RawSayTo(sender, "^1Usage: !remove <target>");
					return;
				}
				if (!Main.Instance.ScriptAdmins.Get(sender.HWID).Parameters.ContainsKey("Administrator") || Main.Instance.ScriptAdmins.Get(sender.HWID).Parameters["Administrator"] != "true")
				{
					Main.Instance.Common.RawSayTo(sender, "^1You need Administrator perm to use this command");
					return;
				}
				List<ScriptAdmins.AdminsInfo> list = (from x in Main.Instance.ScriptAdmins.AdminsList where x.Name.ToLower().Contains(args[1]) select x).ToList<ScriptAdmins.AdminsInfo>();
				if (list.Count != 1)
				{
					Main.Instance.Common.RawSayTo(sender, "^1No or more admins exists");
					return;
				}
				if (list.First<ScriptAdmins.AdminsInfo>().Parameters.ContainsKey("Administrator") && list.First<ScriptAdmins.AdminsInfo>().Parameters["Administrator"] == "true" || list.First<ScriptAdmins.AdminsInfo>().Id <= 0)
				{
					Main.Instance.Common.RawSayTo(sender, "^1You can't remove admins with perm Administrator");
					return;
				}
				Main.Instance.ScriptAdmins.Remove(list.First<ScriptAdmins.AdminsInfo>());
				Main.Instance.Common.RawSayTo(sender, "^1Player has been removed from admins");
				List<Entity> list2 = (from x in Main.Instance.Script.Players where x.Name.ToLower().Contains(args[1]) select x).ToList<Entity>();
				if (list2.Count == 1)
				{
					list2.First<Entity>().SetField("IsAdmin", "false");
					return;
				}
			}, "remove <player>", new List<string>
			{
				"remove"
			});
			this.AddCommand("/admins", delegate (Entity sender, string[] args)
			{
				List<Entity> source = (from x in Main.Instance.Script.Players where x.HasField("IsAdmin") && x.GetField<string>("IsAdmin") == "true" select x).ToList<Entity>();
				Main.Instance.Common.RawSayTo(sender, "^2SupraEditor Online Admins:");
				Main.Instance.Common.RawSayToCondense(sender, (from x in source select x.Name).ToArray<string>(), 1000, 40, ", ");
			}, "/admins", null);
			this.AddCommand("cmd", delegate (Entity sender, string[] args)
			{
				Main.Instance.Common.RawSayTo(sender, "^7Executing: ^2" + string.Join(" ", args.Skip(1)));
				Utilities.ExecuteCommand(string.Join(" ", args.Skip(1)));
			}, "cmd <params>", null);
			this.AddCommand("version", delegate (Entity sender, string[] args)
			{
				Common.WriteLog.Info("SupraEditor v" + ((Main._dllVersion != null) ? Main._dllVersion : null) + " Developed by 0x450 & ReV#PaR$");
			}, "version", null);
			this.AddCommand("model", delegate (Entity sender, string[] args)
			{
				if (args.Length < 2)
				{
					Main.Instance.Common.RawSayTo(sender, "^2Usage: ^7!model ^2<model> ^7[<param>:<value>]");
					return;
				}
				string text = "";
				string modelName = "";

				bool Modelsin = Utilitys.Models.TryGetValue(args[1].ToLower(), out text);

				if (Modelsin)
				{
					modelName = args[1];
					args[1] = text;
					Log.Write(LogLevel.All, text);
				}

				Log.Write(LogLevel.All, Modelsin.ToString());
				ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
				{
					Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
					Model = (modelName == "") ? "Unknown" : Common.FirstLetterToUpper(modelName),
					Angles = sender.GetField<Vector3>("angles").ToString(),
					Origin = sender.Origin.ToString(),
					Parameters = new Dictionary<string, string>
					{
						{
							"Type",
							"Model"
						},
						{
							"Spawn-Type",
							"script_model"
						},
						{
							"Model-Type",
							(args[1].Length < 3) ? "Unknown/Error" : args[1]
						}
					}
				};
				if (args.Length > 2)
				{
					foreach (string text2 in args.Skip(2))
					{
						modelInfo.Parameters[text2.Split(new char[]
						{
							':'
						})[0]] = text2.Split(new char[]
						{
							':'
						})[1];
					}
				}
				Main.Instance.ScriptModel.Add(modelInfo);
				Main.Instance.Common.RawSayTo(sender, "Model ^2Saved^7, Id: ^2" + modelInfo.Id.ToString());
			}, "model [Spawn-Type:<type>]", null);
			this.AddCommand("Box", delegate (Entity sender, string[] args)
			{
				ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
				{
					Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
					Model = "Box",
					Angles = sender.GetField<Vector3>("angles").ToString(),
					Origin = sender.Origin.ToString(),
					Parameters = new Dictionary<string, string>
					{
						{
							"Type",
							"Model"
						},
						{
							"Spawn-Type",
							"script_model"
						},
						{
							"Model-Type",
							"com_plasticcase_friendly"
						},
						{
							"Position",
							"Horizontal"
						}
					}
				};
				Main.Instance.ScriptModel.Add(modelInfo);
				Main.Instance.Common.RawSayTo(sender, "Box ^2Saved^7, Id: ^2" + modelInfo.Id.ToString());
			}, "box", null);
			this.AddCommand("Trampoline", delegate (Entity sender, string[] args)
			{
				ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
				{
					Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
					Model = "Trampoline",
					Angles = sender.GetField<Vector3>("angles").ToString(),
					Origin = sender.Origin.ToString(),
					Parameters = new Dictionary<string, string>
					{
						{
							"Type",
							"Trampoline"
						},
						{
							"Spawn-Type",
							"script_model"
						},
						{
							"Model-Type",
							"com_plasticcase_friendly"
						},
						{
							"Hide",
							"False"
						},
						{
							"icon",
							"cardicon_tictacboom"
						},
						{
							"Height",
							"700"
						},
						{
							"PressToUse",
							"False"
						},
						{
							"Position",
							"Horizontal"
						}
					}
				};
				Main.Instance.ScriptModel.Add(modelInfo);
				Main.Instance.Common.RawSayTo(sender, "Trampoline ^2Saved^7, Id: ^2" + modelInfo.Id.ToString());
			}, "trampoline", null);
			this.AddCommand("Winner", delegate (Entity sender, string[] args)
			{
				ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
				{
					Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
					Model = "Winner",
					Angles = sender.GetField<Vector3>("angles").ToString(),
					Origin = sender.Origin.ToString(),
					Parameters = new Dictionary<string, string>
					{
						{
							"Type",
							"Winner"
						},
						{
							"PressToUse",
							"True"
						},
						{
							"Hide",
							"False"
						},
						{
							"icon",
							"xp"
						}
					}
				};
				Main.Instance.ScriptModel.Add(modelInfo);
				Main.Instance.Common.RawSayTo(sender, "Winner-Spot ^2Saved^7, Id: ^2" + modelInfo.Id.ToString());
			}, "winner", null);
			this.AddCommand("turret", delegate (Entity sender, string[] args)
			{
				sender.SetField("Start", sender.Origin);
				ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
				{
					Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
					Model = "Turret",
					Angles = sender.GetField<Vector3>("angles").ToString(),
					Origin = sender.Origin.ToString(),
					Parameters = new Dictionary<string, string>
					{
						{
							"Type",
							"Turret"
						},
						{
							"Laser",
							"True"
						},
						{
							"Model",
							"sentry_minigun_weak"
						},
						{
							"Weapon",
							"sentry_minigun_mp"
						}
					}
				};
				Main.Instance.ScriptModel.Add(modelInfo);
				Main.Instance.Common.RawSayTo(sender, "Turret ^2Saved^7, Id: ^2" + modelInfo.Id.ToString());
			}, "turret", null);
			this.AddCommand("flame", delegate (Entity sender, string[] args)
			{
				AddEditFX(sender, "Flame");
			}, "flame", null);
			this.AddCommand("tacgreen", delegate (Entity sender, string[] args)
			{
				AddEditFX(sender, "Tacgreen");
			}, "tacgreen", null);
			this.AddCommand("tacred", delegate (Entity sender, string[] args)
			{
				AddEditFX(sender, "Tacred");
			}, "tacred", null);
			this.AddCommand("smoke", delegate (Entity sender, string[] args)
			{
				AddEditFX(sender, "Smoke");
			}, "smoke", null);
			this.AddCommand("laserglow", delegate (Entity sender, string[] args)
			{
				AddEditFX(sender, "LaserGlow");
			}, "laserglow", null);
			this.AddCommand("redcircle", delegate (Entity sender, string[] args)
			{
				AddEditFX(sender, "RedCircle");
			}, "redcircle", null);
			this.AddCommand("goldcircle", delegate (Entity sender, string[] args)
			{
				AddEditFX(sender, "GoldCircle");
			}, "goldcircle", null);
			this.AddCommand("whitecircle", delegate (Entity sender, string[] args)
			{
				AddEditFX(sender, "WhiteCircle");
			}, "whitecircle", null);
			this.AddCommand("fly", delegate (Entity sender, string[] args)
			{
				Main.Instance.Common.ToggleFly(sender);
			}, "fly", null);
			this.AddCommand("ramp", delegate (Entity sender, string[] args)
			{
				this.AddEdit(sender, args, "Ramp", Utilitys._RampModel, false, 1);
			}, "ramp", null);
			this.AddCommand("hramp", delegate (Entity sender, string[] args)
			{
				this.AddEdit(sender, args, "Ramp", Utilitys._RampModel, true, 1);
			}, "hramp", null);
			this.AddCommand("wall", delegate (Entity sender, string[] args)
			{
				this.AddEdit(sender, args, "Wall", Utilitys._WallModel, false, 1);
			}, "Wall", null);
			this.AddCommand("hwall", delegate (Entity sender, string[] args)
			{
				this.AddEdit(sender, args, "Wall", Utilitys._WallModel, true, 1);
			}, "hwall", null);
			this.AddCommand("floor", delegate (Entity sender, string[] args)
			{
				this.AddEdit(sender, args, "Floor", Utilitys._FloorModel, false, 1);
			}, "floor", null);
			this.AddCommand("hfloor", delegate (Entity sender, string[] args)
			{
				this.AddEdit(sender, args, "Floor", Utilitys._FloorModel, true, 1);
			}, "hfloor", null);
			this.AddCommand("zipline", delegate (Entity sender, string[] args)
			{
				this.AddEditZL(sender, args, "Zipline", "null", false, 1);
			}, "zipline", null);
			this.AddCommand("tp", delegate (Entity sender, string[] args)
			{
				this.AddEditTP(sender, args, "Teleport", "null", false, 1);
			}, "tp", null);
			this.AddCommand("htp", delegate (Entity sender, string[] args)
			{
				this.AddEditTP(sender, args, "Teleport", "null", true, 1);
			}, "htp", null);
			this.AddCommand("elevator", delegate (Entity sender, string[] args)
			{
				this.AddEdit(sender, args, "Elevator", Utilitys._ElevatorModel, false, 1);
			}, "elevator", null);
			this.AddCommand("helevator", delegate (Entity sender, string[] args)
			{
				this.AddEdit(sender, args, "Elevator", Utilitys._ElevatorModel, true, 1);
			}, "helevator", null);
			this.AddCommand("door", delegate (Entity sender, string[] args)
			{
				if (!sender.HasField("isDrawing") || sender.GetField<string>("isDrawing") == "false")
				{
					sender.SetField("isDrawing", "true");
					sender.SetField("Start", sender.Origin);
					sender.Call("iprintlnbold", new Parameter[]
					{
						"^2Start Set"
					});
					Main.Instance.Common.ToggleFly(sender);
					return;
				}
				ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
				{
					Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
					Model = "Door",
					Angles = new Vector3(90f, sender.GetField<Vector3>("angles").Y, 90f).ToString(),
					Origin = "null",
					Parameters = new Dictionary<string, string>
					{
						{
							"Type",
							"Door"
						},
						{
							"Model-Type",
							Utilitys._DoorModel
						},
						{
							"Start",
							sender.GetField<Vector3>("Start").ToString()
						},
						{
							"End",
							sender.Origin.ToString()
						},
						{
							"Size",
							"3"
						},
						{
							"Heigth",
							"2"
						},
						{
							"Health",
							"16"
						}
					}
				};
				if (args.Length > 1)
				{
					foreach (string text in args.Skip(1))
					{
						modelInfo.Parameters[text.Split(new char[]
						{
							':'
						})[0]] = text.Split(new char[]
						{
							':'
						})[1];
					}
				}
				Main.Instance.ScriptModel.Add(modelInfo);
				Main.Instance.Common.RawSayTo(sender, "Door ^2Saved^7, Id: ^2" + modelInfo.Id.ToString());
				sender.SetField("isDrawing", "false");
				Main.Instance.Script.AfterDelay(1000, delegate
				{
					Main.Instance.Common.ToggleFly(sender);
				});
			}, "door / door [Size:<size>] [Heigth:<heigth>] [Health:<health>", null);
		}

		private void AddEditFX(Entity sender, string type)
		{
			sender.SetField("Start", sender.Origin);
			ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
			{
				Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
				Model = type,
				Angles = sender.GetField<Vector3>("angles").ToString(),
				Origin = sender.Origin.ToString(),
				Parameters = new Dictionary<string, string>
				{
					{
						"Type",
						type
					}
				}
			};
			Main.Instance.ScriptModel.Add(modelInfo);
			Main.Instance.Common.RawSayTo(sender, type + " ^2Saved^7, Id: ^2" + modelInfo.Id.ToString());
		}

		private void AddEdit(Entity sender, string[] args, string type, string modelHash, bool hidden = false, int paramIndex = 1)
		{
			if (!sender.HasField("isDrawing") || sender.GetField<string>("isDrawing") == "false")
			{
				sender.SetField("isDrawing", "true");
				sender.SetField("Start", sender.Origin);
				sender.Call("iprintlnbold", new Parameter[]
				{
					"^2Start Set"
				});
				Main.Instance.Common.ToggleFly(sender);
				return;
			}
			ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
			{
				Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
				Model = type,
				Angles = "null",
				Origin = "null",
				Parameters = new Dictionary<string, string>
				{
					{
						"Type",
						type
					},
					{
						"Start",
						sender.GetField<Vector3>("Start").ToString()
					},
					{
						"End",
						sender.Origin.ToString()
					},
					{
						"Hide",
						hidden.ToString()
					},
					{
						"Model-Type",
						modelHash
					}
				}
			};
			if (args.Length > paramIndex)
			{
				foreach (string text in args.Skip(paramIndex))
				{
					modelInfo.Parameters[text.Split(new char[]
					{
						':'
					})[0]] = text.Split(new char[]
					{
						':'
					})[1];
				}
			}
			Main.Instance.ScriptModel.Add(modelInfo);
			Main.Instance.Common.RawSayTo(sender, string.Concat(new object[]
			{
				hidden ? "^9Hidden ^7" : "^7",
				type,
				" ^2Saved^7, Id: ^2",
				modelInfo.Id
			}));
			sender.SetField("isDrawing", "false");
			Main.Instance.Script.AfterDelay(1000, delegate ()
			{
				Main.Instance.Common.ToggleFly(sender);
			});
		}

		private void AddEditTP(Entity sender, string[] args, string type, string modelHash, bool hidden = false, int paramIndex = 1)
		{
			if (!sender.HasField("isDrawing") || sender.GetField<string>("isDrawing") == "false")
			{
				sender.SetField("isDrawing", "true");
				sender.SetField("Start", sender.Origin);
				sender.Call("iprintlnbold", new Parameter[]
				{
					"^2Start Set"
				});
				Main.Instance.Common.ToggleFly(sender);
				return;
			}
			ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
			{
				Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
				Model = type,
				Angles = "null",
				Origin = "null",
				Parameters = new Dictionary<string, string>
				{
					{
						"Type",
						type
					},
					{
						"Start",
						sender.GetField<Vector3>("Start").ToString()
					},
					{
						"End",
						sender.Origin.ToString()
					},
					{
						"Hide",
						hidden.ToString()
					},
					{
						"Model-Type",
						modelHash
					},
					{
						"Members-Only",
						"False"
					},
					{
						"PressToUse",
						"False"
					},
					{
						"Allow-INF-Members",
						"True"
					}
				}
			};
			if (args.Length > paramIndex)
			{
				foreach (string text in args.Skip(paramIndex))
				{
					modelInfo.Parameters[text.Split(new char[]
					{
						':'
					})[0]] = text.Split(new char[]
					{
						':'
					})[1];
				}
			}
			Main.Instance.ScriptModel.Add(modelInfo);
			Main.Instance.Common.RawSayTo(sender, string.Concat(new object[]
			{
				hidden ? "^9Hidden ^7" : "^7",
				type,
				" ^2Saved^7, Id: ^2",
				modelInfo.Id
			}));
			sender.SetField("isDrawing", "false");
			Main.Instance.Script.AfterDelay(1000, delegate ()
			{
				Main.Instance.Common.ToggleFly(sender);
			});
		}

		private void AddEditZL(Entity sender, string[] args, string type, string modelHash, bool hidden = false, int paramIndex = 1)
		{
			if (!sender.HasField("isDrawing") || sender.GetField<string>("isDrawing") == "false")
			{
				sender.SetField("isDrawing", "true");
				sender.SetField("Start", sender.Origin);
				sender.Call("iprintlnbold", new Parameter[]
				{
					"^2Start Set"
				});
				Main.Instance.Common.ToggleFly(sender);
				return;
			}
			ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo
			{
				Id = Main.Instance.ScriptModel.ModelsList.Count - 1,
				Model = type,
				Angles = sender.GetField<Vector3>("angles").ToString(),
				Origin = "null",
				Parameters = new Dictionary<string, string>
				{
					{
						"Type",
						type
					},
					{
						"Start",
						sender.GetField<Vector3>("Start").ToString()
					},
					{
						"End",
						sender.Origin.ToString()
					},
					{
						"Hide",
						"False"
					},
					{
						"PressToUse",
						"True"
					}
				}
			};
			if (args.Length > paramIndex)
			{
				foreach (string text in args.Skip(paramIndex))
				{
					modelInfo.Parameters[text.Split(new char[]
					{
						':'
					})[0]] = text.Split(new char[]
					{
						':'
					})[1];
				}
			}
			Main.Instance.ScriptModel.Add(modelInfo);
			Main.Instance.Common.RawSayTo(sender, string.Concat(new object[]
			{
				hidden ? "^9Hidden ^7" : "^7",
				type,
				" ^2Saved^7, Id: ^2",
				modelInfo.Id
			}));
			sender.SetField("isDrawing", "false");
			Main.Instance.Script.AfterDelay(1000, delegate ()
			{
				Main.Instance.Common.ToggleFly(sender);
			});
		}

		private void AddCommand(string name, Action<Entity, string[]> run, string usage = null, List<string> aliases = null)
		{
			this.Commands.AddCommand(name, run, usage, aliases);
		}

		public bool HandleCommand(Entity player, string message)
		{
			string[] array = message.Split(new char[]
			{
				' '
			});
			array[0] = array[0].ToLowerInvariant();
			foreach (Commands commands in from x in Commands.CommandList where x.Active select x)
			{
				if (!(commands.Name.ToLower() == array[0].ToLower()))
				{
					List<string> alias = commands.Alias;
					if (alias == null || !alias.Contains(array[0]))
					{
						continue;
					}
				}
				if (array.Length > 1 && array[1] == "/?")
				{
					Main.Instance.Common.RawSayTo(player, "^5Command: " + commands.Name);
					Main.Instance.Common.RawSayTo(player, "^5Usage: " + commands.Usage);
					return true;
				}
				try
				{
					commands.Run(player, array);
					return true;
				}
				catch (Exception ex)
				{
					Log.Error("There was an error while handling a command: {0}", new object[]
					{
						ex.Message
					});
					return false;
				}
			}
			return false;
		}

		public string NextWord(string m_str)
		{
			return this.NextWord(m_str, " ");
		}

		public string NextWord(string m_str, string seperator)
		{
			int length = m_str.Length;
			if (this.m_pos >= length)
			{
				return "";
			}
			int num;
			while ((num = this.CustomIndexOf(m_str, seperator, this.m_pos, '\'')) == 0)
			{
				this.m_pos += seperator.Length;
			}
			if (num < 0)
			{
				if (this.m_pos == length)
				{
					return "";
				}
				num = length;
			}
			string result = m_str.Substring(this.m_pos, num - this.m_pos);
			this.m_pos = num + seperator.Length;
			if (this.m_pos > length)
			{
				this.m_pos = length;
			}
			return result;
		}

		private int CustomIndexOf(string m_str, string searchedChar, int startIndex, char containerChar)
		{
			if (searchedChar.Length > 1)
			{
				throw new ArgumentException("searchedChar can only be a char into a string");
			}
			char c = searchedChar[0];
			int num = startIndex;
			bool flag = false;
			do
			{
				if (m_str[num] == containerChar)
				{
					flag = !flag;
				}
				num++;
				if (num >= m_str.Length)
				{
					return -1;
				}
			}
			while (flag || m_str[num] != c);
			return num;
		}
	}
}
