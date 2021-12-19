using System;
using System.Collections.Generic;
using System.Linq;
using InfinityScript;

namespace MapEdit
{
	public class MapEdit : BaseScript
	{
		public static Dictionary<string, int> WinnerData = new Dictionary<string, int>();
		public static Dictionary<string, int> WinnersData = new Dictionary<string, int>();

		private string WinName;
		private int WinRanking;

		public bool ExitsAdmins = false;

		private Main Main
		{
			get
			{
				return Main.Instance;
			}
		}
		public MapEdit()
		{
			this.Main.InitMain(this);
			this.Main.ScriptModel.SpawnAll();
			Common.WriteLog.Info("SupraEditor v" + ((Main._dllVersion != null) ? Main._dllVersion : null) + " Developed by 0x450 & ReV#PaR$");
			//Call("precacheShader", "cardicon_skull_black");
			base.Call("precachemodel", new Parameter[]
			{
				this.Main.ScriptModel.GetFlagModel(Common._mapnameFull(false))
			});
			base.Call("precachemodel", new Parameter[]
			{
				"prop_flag_neutral"
			});
			base.Call("precacheshader", new Parameter[]
			{
				"waypoint_flag_friendly"
			});
			base.Call("precacheshader", new Parameter[]
			{
				"compass_waypoint_target"
			});
			base.Call("precacheshader", new Parameter[]
			{
				"compass_waypoint_bomb"
			});
			base.Call("precachemodel", new Parameter[]
			{
				"weapon_scavenger_grenadebag"
			});
			base.Call("precachemodel", new Parameter[]
			{
				"weapon_oma_pack"
			});
			base.PlayerConnected += this.SE_PlayerConnected;
			base.PlayerDisconnected += this.SE_PlayerDisconnected;

			base.OnNotify("game_ended", delegate (Parameter level)
			{

				foreach (Entity player in Main.Instance.Script.Players)
				{
					//player.Call("setblurforplayer", 10, 0.3f);

					if (ScriptModel._Score_Ranking >= 1)//>= 1
					{
						HudElem bar223 = HudElem.NewHudElem();
						bar223.Parent = HudElem.UIParent;
						bar223.SetPoint("CENTER", "CENTER", 0, -110);
						bar223.SetShader("black", 500, 100);
						bar223.Foreground = false;
						bar223.HideWhenInMenu = false;
						bar223.Alpha = .275f;
						foreach (KeyValuePair<string, int> Winner in WinnerData)
						{
							HudElem Tekno1 = HudElem.CreateFontString(player, "hudbig", 0.9f);
							Tekno1.SetPoint("CENTER", "CENTER", 0, -115);
							Tekno1.SetText("The Winner is: ^2" + Winner.Key);
							//Tekno1.SetText("The Winner is: ^2" + Winner.Key + "^7[^2" + Winner.Value +"^7]");
							//Utilities.SayTo(player, $"Winner: {Winner.Key} - {Winner.Value}");
							WinName = Winner.Key;
							WinRanking = Winner.Value;
						}
						if (Common.IsNullOrEmpty(WinnerData) == false && Common.IsNullOrEmpty(WinnersData) == false)
						{
							Common.WriteChatToPlayer(player, $"^2====> ^7Top 3 ^2<====");
							Utilities.SayTo(player, $"^2{WinName} ^7=> ^3{WinRanking}");
							foreach (KeyValuePair<string, int> Winners in WinnersData)
							{
								Utilities.SayTo(player, $"^2{Winners.Key} ^7=> ^3{Winners.Value}");
							}
						}

						if (WinName != "" || WinName != null)
						{
							if (ScriptAdmins.DiscordWebHook != "" || ScriptAdmins.DiscordWebHook != null)
							{
								WebhookObject winnerSE = new WebhookObject()
								{
									username = "SupraEditor",
									avatar_url = "https://i.imgur.com/A6ANnWw.png",
									embeds = new Embed[]
									{
									new Embed()
									{
										color = int.Parse("80E61F", System.Globalization.NumberStyles.HexNumber),
										author = new Author()
										{
											name = "Call of Duty: Modern Warfare 3",
											url = "https://github.com/0x450",
											icon_url = "https://i.imgur.com/A6ANnWw.png"
										},
										thumbnail = new Thumbnail()
										{
											url = Webhook.Thumbnail(Common._mapnameFull(false))
										},
										fields = new Field[]{
											new Field()
											{
												name = "Sever",
												value = Common._svname(),
												inline = true
											},
											new Field()
											{
												name = "Winner",
												value = WinName,
												inline = true
											}
										},
										footer = new Footer()
										{
											text = $"Developed by 0x450  -  {DateTime.UtcNow}",
											icon_url = "https://i.imgur.com/A6ANnWw.png",
										}
									}
									}
								};
								Webhook.PostData(ScriptAdmins.DiscordWebHook, winnerSE);
							}
						}
					}
					else
					{
						if (ScriptModel.IsWinnerInMap == true)
						{
							Common.WriteChatToPlayer(player, "Could not find any winner");
						}
					}
				}
			});

		}

		private void SE_PlayerDisconnected(Entity player)
		{
			player.SetField("IsAdmin", false);
			player.SetField("IsMember", false);
		}

		private void SE_PlayerConnected(Entity player)
		{
			Common.SetSpeed(player, 1.5f);
			Common.WriteLog.Info(Common._mapnameFull(true));
			Common.WriteLog.Info(Common._mapnameFull());

			player.Health = 999998;

			player.SetField("finishPK", "false");
			player.SetField("LastLocationPoint", new Vector3(0, 0, 0));

			player.SetField("IsMember", "false");
			player.SetField("IsAdmin", "false");
			player.SetField("isDrawing", "false");
			player.SetField("attackeddoor", 0);
			player.SetField("repairsleft", 5);

			ScriptAdmins.AdminsInfo adminsInfo = Main.Instance.ScriptAdmins.Get(player.HWID);
			if (adminsInfo != null)
			{
				if (ExitsAdmins == false)
				{
					ExitsAdmins = true;
				}
				player.SetField("IsAdmin", "true");
				player.SetField("IsMember", "true");
				foreach (KeyValuePair<string, string> keyValuePair in adminsInfo.Parameters)
				{
					player.SetField(keyValuePair.Key, keyValuePair.Value.ToLower());
				}
				if (adminsInfo.Name != player.Name)
				{
					(from x in Main.Instance.ScriptAdmins.AdminsList where x.HWID == player.HWID select x).FirstOrDefault<ScriptAdmins.AdminsInfo>().Name = player.Name;
					Main.Instance.ScriptAdmins.SaveAdmins();
				}
				Log.Debug("SupraEditor-Admin " + player.Name + " has connected.");
			}

			ScriptAdmins.MembersInfo membersInfo = Main.Instance.ScriptAdmins.GetFromMembers(player.HWID);
			if (membersInfo != null)
			{
				player.SetField("IsMember", "true");
				foreach (KeyValuePair<string, string> keyValuePair in membersInfo.Parameters)
				{
					player.SetField(keyValuePair.Key, keyValuePair.Value.ToLower());
				}
				if (membersInfo.Name != player.Name)
				{
					(from x in Main.Instance.ScriptAdmins.MembersList where x.HWID == player.HWID select x).FirstOrDefault<ScriptAdmins.MembersInfo>().Name = player.Name;
					Main.Instance.ScriptAdmins.SaveMembers();
				}
				Log.Debug("SupraEditor-Member " + player.Name + " has connected.");
			}

			player.Call("notifyonplayercommand", "used-pressed", "+activate");
			player.OnNotify("used-pressed", new Action<Entity>(Main.Instance.ScriptModel.HandleUseables));
			Main.Instance.ScriptModel.UsablesHud(player);

			player.Call("notifyonplayercommand", "4", "+actionslot 4");
			player.SetField("ThirdPerson", "0");
			player.OnNotify("4", delegate (Entity ent)
			{
				string text = (ent.GetField<string>("ThirdPerson") == "0") ? "1" : "0";
				ent.SetField("ThirdPerson", text);
				ent.SetClientDvar("cg_thirdperson", text);
				ent.SetClientDvar("cg_thirdPersonRange", "170");
			});

			player.Call("notifyonplayercommand", "lastlocation", "vote yes");
			//player.Call("notifyonplayercommand", new Parameter[] { "lastlocation", "vote yes" });
			player.OnNotify("lastlocation", delegate (Entity ent)
			{
				try
				{
					Vector3 lol = new Vector3(0, 0, 0);
					if (player.GetField<Vector3>("LastLocationPoint").ToString() != lol.ToString())
					{
						Vector3 setOrigin = player.GetField<Vector3>("LastLocationPoint");
						player.Call("setorigin", setOrigin);
					}
				}
				catch (Exception ex)
				{
					Log.Debug("Error: " + ex);
				}
			});


			Log.Debug("Player " + player.Name + " has connected. - " + player.IP.Address + " - " + player.GUID.ToString());

			if (player.GetField<string>("IsAdmin") == "true")
			{
				player.Call("notifyonplayercommand", "fly", "+actionslot 3");
				player.OnNotify("fly", Common.fly);
				player.Call("iprintlnbold", "^3Flight activated on the button: [{+actionslot 3}]");
			}

			player.SpawnedPlayer += delegate ()
			{
				player.Health = 999998;
			};

			Credits(player);

			if (ScriptAdmins.OnPlayerConnectedAddTrail.ToLower() == "true" || ScriptAdmins.OnPlayerConnectedAddTrail.ToLower() == "yes" || ScriptAdmins.OnPlayerConnectedAddTrail.ToLower() == "enable")
			{
				player.OnInterval(500, delegate
				{
					Common.DropMoney(player.Origin);
					return true;
				});
			}

			Log.Debug(ScriptModel.playerIsMember(player).ToString());
		}

		public static void Credits(Entity player)
		{
			if (ScriptAdmins.HudCredits.ToLower() == "true" || ScriptAdmins.HudCredits.ToLower() == "yes" || ScriptAdmins.HudCredits.ToLower() == "enable")
			{
				HudElem Tekno = HudElem.CreateFontString(player, "hudbig", 0.7f);
				Tekno.SetPoint("CENTER", "TOP", 0, 5);
				Tekno.SetText("TeknoMw3");
				Tekno.Alpha = .275f;

				HudElem credits = HudElem.CreateFontString(player, "hudbig", 0.9f);
				credits.SetPoint("CENTER", "BOTTOM", 0, -30);
				credits.Call("settext", new Parameter[]
				{
					"SupraEditor"
				});
				credits.Alpha = 0f;

				HudElem credits2 = HudElem.CreateFontString(player, "hudbig", 0.6f);
				credits2.SetPoint("CENTER", "BOTTOM", 0, -15);
				credits2.Call("settext", new Parameter[]
				{
					"</Developed by 0x450>"
				});//Created by ReV#PaR$ and modified by 0x450
				credits2.Alpha = 0f;

				HudElem hudElem = HudElem.NewHudElem();
				hudElem.Parent = HudElem.UIParent;
				hudElem.SetPoint("CENTER", "BOTTOM", 0, -65);
				hudElem.SetShader("cardicon_sniper", 50, 50);
				hudElem.Alpha = 0f;

				//Show Hud
				player.Call("notifyonplayercommand", new Parameter[]
				{
					"tab",
					"+scores"
				});
				player.OnNotify("tab", delegate (Entity entity)
				{
					credits.Alpha = .275f;
					//credits.Alpha = 1f;
					credits2.Alpha = .275f;
					hudElem.Alpha = 1f;
				});

				//Hidde Hud
				player.Call("notifyonplayercommand", new Parameter[]
				{
					"-tab",
					"-scores"
				});
				player.OnNotify("-tab", delegate (Entity entity)
				{
					credits.Alpha = 0f;
					credits2.Alpha = 0f;
					hudElem.Alpha = 0f;
				});
			}
		}

		public override BaseScript.EventEat OnSay3(Entity player, BaseScript.ChatType type, string name, ref string message)
		{
			string[] msg = message.Split(new char[] { '\x20', '\t', '\r', '\n', '\f', '\b', '\v', ';' }, StringSplitOptions.RemoveEmptyEntries);
			if (msg[0].Equals("!rank"))
			{
				if (player.HasField("PKranking") == true)
				{
					Utilities.RawSayAll(player.Name + "=> " + player.GetField<int>("PKranking").ToString());
				}
				return BaseScript.EventEat.EatGame;
			}
			else if (msg[0].Equals("!endgame") || msg[0].Equals("!eg"))
			{
				player.Notify("menuresponse", "menu", "endround");
				return BaseScript.EventEat.EatGame;
			}
			else if (msg[0].Equals("!win"))
			{
				if (ScriptModel.IsWinnerInMap == true)
				{
					if (ScriptModel._Score_Ranking >= 1)
					{
						if (Common.IsNullOrEmpty(WinnerData) == false && Common.IsNullOrEmpty(WinnersData) == false)
						{
							Common.WriteChatToPlayer(player, $"^2====> ^7Top 3 ^2<====");
							Utilities.SayTo(player, $"^2{WinName} ^7=> ^3{WinRanking}");
							foreach (KeyValuePair<string, int> Winners in WinnersData)
							{
								Utilities.SayTo(player, $"^2{Winners.Key} ^7=> ^3{Winners.Value}");
							}
						}
					}
					else
					{
						Common.WriteChatToPlayer(player, "Could not find any winner");
					}
				}
				return BaseScript.EventEat.EatGame;
			}
			else if (msg[0].Equals("!rage"))
			{
				Utilities.ExecuteCommand("kick " + player.Name);
			}
			else if (msg[0].Equals("!trail"))
			{
				player.OnInterval(500, delegate
				{
					Common.DropMoney(player.Origin);
					return true;
				});
			}
			if (msg[0].Equals("!IAdmin"))
			{
				if (ExitsAdmins == false)
				{
					ExitsAdmins = true;
					if ((from x in Main.Instance.ScriptAdmins.AdminsList where x.Id == 0 select x) != null)
					{
						Main.Instance.ScriptAdmins.Add(player.Name, player.HWID, new Dictionary<string, string>
						{
							{
								"Administrator",
								"true"
							}
						});
						player.SetField("IsAdmin", "true");
						player.SetField("Administrator", "true");
						player.Call("iprintlnbold", "^2Aministrator Permissions ^1Granted");
						Log.Debug("Player " + player.Name + " has logged in to SupraEditor as Administrator");
						return BaseScript.EventEat.EatGame;
					}
				}
			}
			else if (player.HasField("IsAdmin") && player.GetField<string>("IsAdmin") == "true")
			{
				if (!Main.Instance.CommandManager.HandleCommand(player, message.Substring(1)))
				{
					return BaseScript.EventEat.EatNone;
				}
				return BaseScript.EventEat.EatGame;
			}
			return BaseScript.EventEat.EatNone;
		}
	}
}
