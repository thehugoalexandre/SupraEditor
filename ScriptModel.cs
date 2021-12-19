using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using InfinityScript;
using Newtonsoft.Json;

namespace MapEdit
{
	public class ScriptModel
	{
		public static int _Score_Ranking = 0;
		public static string[] modellist2;
		public Entity _airdropCollision;
		//public Entity _testCollision;
		private Random _rng = new Random();

		public readonly string MapJson = string.Format("scripts/SupraEditor/{0}.json", Common._mapnameFull(true));

		public static int[] Spawns;

		public static bool IsWinnerInMap = false;

		public int MoveTime { get; private set; }
		public bool IsUsing { get; private set; }

		public class ModelInfo
		{
			[JsonProperty("Id", Required = Required.Always)]
			public int Id;

			[JsonProperty("Model", Required = Required.Always)]
			public string Model;

			[JsonProperty("Origin", Required = Required.Always)]
			public string Origin;

			[JsonProperty("Angles", Required = Required.Always)]
			public string Angles;

			[JsonProperty("Parameters", Required = Required.Default)]
			public Dictionary<string, string> Parameters;
		}

		public List<ScriptModel.ModelInfo> ModelsList { get; private set; }
		public List<Entity> SpawnedModels { get; private set; }


		public ScriptModel()
		{
			Entity entity = Function.Call<Entity>("getent", new Parameter[]
			{
				"care_package",
				"targetname"
			});
			this._airdropCollision = Function.Call<Entity>("getent", new Parameter[]
			{
				entity.GetField<string>("target"),
				"targetname"
			});
			this.ModelsList = new List<ScriptModel.ModelInfo>();
			this.SpawnedModels = new List<Entity>();
			this.Initialize();
		}

		public void Initialize()
		{
			bool flag = true;
			if (!Directory.Exists(Utilitys.MapEditFolder))
			{
				Directory.CreateDirectory(Utilitys.MapEditFolder);
			}
			if (!File.Exists(this.MapJson))
			{
				File.WriteAllLines(this.MapJson, new string[0]);
				flag = false;
			}
			if (this.ModelsList == null)
			{
				this.ModelsList = new List<ScriptModel.ModelInfo>();
			}
			if (!flag)
			{
				List<ScriptModel.ModelInfo> modelsList = this.ModelsList;
				ScriptModel.ModelInfo modelInfo = new ScriptModel.ModelInfo();
				modelInfo.Id = -1;
				modelInfo.Model = "DO NOT EDIT THIS";
				Vector3 vector = default(Vector3);
				modelInfo.Angles = vector.ToString();
				vector = default(Vector3);
				modelInfo.Origin = vector.ToString();
				modelInfo.Parameters = new Dictionary<string, string>
				{
					{
						"Type",
						"Script Model"
					}
				};
				modelsList.Add(modelInfo);
				this.SaveMap();
			}
			this.LoadMap();
		}

		public void LoadMap()
		{
			if (this.ModelsList == null)
			{
				this.ModelsList = new List<ScriptModel.ModelInfo>();
			}
			if (File.Exists(this.MapJson))
			{
				this.ModelsList = JsonConvert.DeserializeObject<List<ScriptModel.ModelInfo>>(File.ReadAllText(this.MapJson));
			}
		}

		public void SaveMap()
		{
			if (this.ModelsList != null && this.ModelsList.Count > 0)
			{
				File.WriteAllText(this.MapJson, JsonConvert.SerializeObject(this.ModelsList, Formatting.Indented));
			}
		}

		public void ReloadMap()
		{
			this.SaveMap();
			this.LoadMap();
		}

		public void Add(string name, Vector3 origin, Vector3 angles, Dictionary<string, string> dict)
		{
			this.Add(new ScriptModel.ModelInfo
			{
				Id = this.ModelsList.Count - 1,
				Model = name,
				Origin = origin.ToString(),
				Angles = angles.ToString(),
				Parameters = dict
			});
		}

		public void Add(ScriptModel.ModelInfo model)
		{
			this.ModelsList.Add(model);
			this.SaveMap();
		}

		public ScriptModel.ModelInfo Get(int id)
		{
			if (!this.Exists(id))
			{
				return null;
			}
			return (from x in this.ModelsList where x.Id == id select x).FirstOrDefault<ScriptModel.ModelInfo>();
		}

		public List<ScriptModel.ModelInfo> Get(string name)
		{
			if (!this.Exists(name))
			{
				return new List<ScriptModel.ModelInfo>();
			}
			return (from x in this.ModelsList where x.Model == name select x).ToList<ScriptModel.ModelInfo>();
		}

		public void Remove(int id)
		{
			if (!this.Exists(id))
			{
				return;
			}
			this.Remove((from x in this.ModelsList where x.Id == id select x).FirstOrDefault<ScriptModel.ModelInfo>());
		}
		Func<ScriptModel.ModelInfo, bool> test;
		public void Remove(ScriptModel.ModelInfo modelinfo)
		{
			this.ModelsList.Remove(modelinfo);
			IEnumerable<ScriptModel.ModelInfo> modelsList = this.ModelsList;
			Func<ScriptModel.ModelInfo, bool> predicate;
			predicate = test;
			if (predicate == null)
			{
				predicate = (test = ((ScriptModel.ModelInfo x) => x.Id > modelinfo.Id));
			}
			foreach (ScriptModel.ModelInfo modelInfo in modelsList.Where(predicate))
			{
				modelInfo.Id--;
			}
			this.SaveMap();
		}

		public bool Exists(int id)
		{
			using (List<ScriptModel.ModelInfo>.Enumerator enumerator = this.ModelsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Id == id)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool Exists(string name)
		{
			using (List<ScriptModel.ModelInfo>.Enumerator enumerator = this.ModelsList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					if (enumerator.Current.Model == name)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void SpawnAll()
		{
			if (this.SpawnedModels == null)
			{
				this.SpawnedModels = new List<Entity>();
			}
			foreach (ScriptModel.ModelInfo model in this.ModelsList)
			{
				this.Spawn(model);
			}
		}

		public void createFX(Vector3 vector, string loadfx)
		{
			int v = Function.Call<int>("loadfx", new Parameter[]
			{
				loadfx
			});
			Entity v2 = Function.Call<Entity>("spawnFx", new Parameter[]
			{
				v,
				vector
			});
			Function.Call<Entity>("triggerfx", new Parameter[]
			{
				v2
			});
		}

		public void createTurret(Entity entity, Vector3 origin, Vector3 angles, string Weapon, string model, string Laser)
		{
			entity = Function.Call<Entity>("spawnturret", new Parameter[]
			{
				"misc_turret",
				origin,
				Weapon
			});

			entity.Call("setmodel", new Parameter[]
			{
				model
			});


			/*entity.Call("sethintstring", new Parameter[]
			{
				"^7Press ^3[{+activate}] ^7to use the turret."
			});
			*/

			entity.SetField("angles", new Parameter(angles));

			if (Laser.ToLower() == "true")
			{
				entity.Call("laseron", new Parameter[0]);
			}
		}


		public Entity Spawn(ScriptModel.ModelInfo model)
		{
			Entity entity = null;
			bool flag = false;
			string param = this.GetParam(model, "Type");
			string laser = this.GetParam(model, "Laser");
			string Weapon = this.GetParam(model, "Weapon");
			string modelt = this.GetParam(model, "Model");
			string model_type = this.GetParam(model, "Model-Type");
			string Members_Only = this.GetParam(model, "Members-Only");
			string PressToUse = this.GetParam(model, "PressToUse");
			string Allow_infected_Members = this.GetParam(model, "Allow-INF-Members");
			string getFX = this.GetParam(model, "FX");

			string height = this.GetParam(model, "Height");

			string Position = this.GetParam(model, "Position");

			bool hide = this.ContainsParam(model, "Hide") && bool.Parse(this.GetParam(model, "Hide"));


			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);

			if ((!this.TryParse(model.Angles, out vector2) || !this.TryParse(model.Origin, out vector)) && (param == "Model" || param == "Turret"))
			{
				Log.Error("There is an currpoted Vector3 at model id: " + model.Id);
				return null;
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "Model" && !flag))
			{
				entity = Function.Call<Entity>("spawn", new Parameter[]
				{
					this.GetParam(model, "Spawn-Type"),
					vector
				});
				entity.Call("setmodel", new Parameter[]
				{
					model_type
				});
				entity.SetField("angles", vector2);
				entity.Call(33353, new Parameter[]
				{
					this._airdropCollision
				});
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "Turret" && !flag))
			{
				createTurret(entity, vector, vector2, Weapon, modelt, laser);
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "Box" && !flag))
			{
				entity = Function.Call<Entity>("spawn", new Parameter[]
				{
					this.GetParam(model, "Spawn-Type"),
					vector
				});

				if (model_type != null)
				{
					entity.Call("setmodel", new Parameter[]
					{
						model_type
					});
				}

				if (Position.ToLower() == "normal" || Position.ToLower() == "horizontal" || Position.ToLower() == "default" || Position.ToLower() == "")
				{
					entity.SetField("angles", new Parameter(vector2));
				}
				else if (Position.ToLower() == "diagonal")
				{
					entity.SetField("angles", new Parameter(new Vector3(vector2.X + 50, vector2.Y, vector2.Z)));
				}
				else if (Position.ToLower() == "vertical")
				{
					entity.SetField("angles", new Parameter(new Vector3(vector2.X + 90, vector2.Y, vector2.Z)));
				}

				entity.Call(33353, new Parameter[]
				{
					this._airdropCollision
				});
			}
			if (param == "Trampoline" && !flag)
			{
				entity = Function.Call<Entity>("spawn", new Parameter[]
				{
					this.GetParam(model, "Spawn-Type"),
					vector
				});

				if (model_type != null)
				{
					entity.Call("setmodel", new Parameter[]
					{
						model_type
					});
				}

				if (Position.ToLower() == "normal" || Position.ToLower() == "horizontal" || Position.ToLower() == "default" || Position.ToLower() == "")
				{
					entity.SetField("angles", new Parameter(vector2));
				}
				else if (Position.ToLower() == "diagonal")
				{
					entity.SetField("angles", new Parameter(new Vector3(vector2.X + 50, vector2.Y, vector2.Z)));
				}
				else if (Position.ToLower() == "vertical")
				{
					entity.SetField("angles", new Parameter(new Vector3(vector2.X + 90, vector2.Y, vector2.Z)));
				}

				entity.Call(33353, new Parameter[]
				{
					this._airdropCollision
				});

				int id = model.Id;
				if (!hide)
				{
					Main.Instance.Script.Call(431, new Parameter[]
					{
						id,
						"active"
					});
					Main.Instance.Script.Call(435, new Parameter[]
					{
						id,
						new Parameter(entity.Origin)
					});
					if (this.ContainsParam(model, "icon") && !hide)
					{
						Main.Instance.Script.Call(434, new Parameter[]
						{
							id,
							this.GetParam(model, "icon")
						});
					}
				}
				else
				{
					entity.Call("setmodel", new Parameter[]
					{
						"tag_origin"
					});
				}

				entity.SetField("type", "Trampoline");
				entity.SetField("Height", height);
				entity.SetField("PressToUse", PressToUse);
				this.SpawnedModels.Add(entity);
			}
			if (param == "Winner" && !flag)
			{
				entity = Function.Call<Entity>("spawn", new Parameter[]
				{
					"script_model",
					vector
				});

				entity.Call("setmodel", new Parameter[]
				{
					"tag_origin"
				});

				int id = model.Id;
				if (!hide)
				{
					Main.Instance.Script.Call(431, new Parameter[]
					{
						id,
						"active"
					});
					Main.Instance.Script.Call(435, new Parameter[]
					{
						id,
						new Parameter(entity.Origin)
					});
					if (this.ContainsParam(model, "icon"))
					{
						Main.Instance.Script.Call(434, new Parameter[]
						{
							id,
							this.GetParam(model, "icon")
						});
					}
				}

				entity.SetField("angles", new Parameter(vector2));

				if (IsWinnerInMap == false)
				{
					IsWinnerInMap = true;
				}

				entity.SetField("type", "Winner");
				entity.SetField("PressToUse", PressToUse);
				this.SpawnedModels.Add(entity);
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "Flame" && !flag))
			{
				if (Common._mapnameFull(false) == "mp_dome")
				{
					createFX(vector, "fire/firelp_med_pm_nodistort");
				}
				else if (Common._mapnameFull(false) == "mp_seatown")
				{
					createFX(vector, "fire/car_fire_mp");
				}
				else if (Common._mapnameFull(false) == "mp_hardhat")
				{
					createFX(vector, "fire/car_fire_mp_far");
				}
				else
				{
					createFX(vector, "fire/firelp_med_pm");
				}
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "Smoke" && !flag))
			{
				createFX(vector, "smoke/thin_black_smoke_s_fast");
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "Tacgreen" && !flag))
			{
				createFX(vector, "misc/flare_ambient_green");
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "Tacred" && !flag))
			{
				createFX(vector, "misc/flare_ambient");
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "LaserGlow" && !flag))
			{
				createFX(vector, "misc/laser_glow");
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "RedCircle" && !flag))
			{
				createFX(vector, Utilitys.Fx_redcircle);
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "GoldCircle" && !flag))
			{
				createFX(vector, Utilitys.Fx_goldcircle);
			}
			if (model.Parameters.Count<KeyValuePair<string, string>>() == 0 || model.Parameters == null || (param == "WhiteCircle" && !flag))
			{
				createFX(vector, Utilitys.fx_whitecircle);
			}
			else
			{
				if (this.GetParam(model, "Start") == null && this.GetParam(model, "End") == null)
				{
					return null;
				}

				Vector3 Start = default(Vector3);
				Vector3 End = default(Vector3);
				this.TryParse(this.GetParam(model, "Start"), out Start);
				this.TryParse(this.GetParam(model, "End"), out End);

				bool flag2 = this.ContainsParam(model, "Hide") && bool.Parse(this.GetParam(model, "Hide"));

				if (param == "Ramp" && !flag)
				{
					int num = (int)Math.Ceiling((double)(Start.DistanceTo(End) / 30f));
					Vector3 vector3 = new Vector3((Start.X - End.X) / (float)num, (Start.Y - End.Y) / (float)num, (Start.Z - End.Z) / (float)num);
					Vector3 vector4 = Main.Instance.Script.Call<Vector3>("vectortoangles", new Parameter[]
					{
						Start - End
					});
					Vector3 angles = new Vector3(vector4.Z, vector4.Y + 90f, vector4.X);
					for (int i = 0; i <= num; i++)
					{
						this.SpawnCrate(End + vector3 * (float)i, angles, flag2 ? null : model_type);
					}
					flag = true;
				}
				if (param == "Wall" && !flag)
				{
					Vector3 start = Start;
					Vector3 end = End;
					float num2 = new Vector3(start.X, start.Y, 0f).DistanceTo(new Vector3(end.X, end.Y, 0f));
					double num3 = (double)new Vector3(0f, 0f, start.Z).DistanceTo(new Vector3(0f, 0f, end.Z));
					int num4 = (int)Math.Round((double)(num2 / 55f), 0);
					int num5 = (int)Math.Round(num3 / 30.0, 0);
					Vector3 vector5 = end - start;
					Vector3 vector6 = new Vector3(vector5.X / (float)num4, vector5.Y / (float)num4, vector5.Z / (float)num5);
					float num6 = vector6.X / 4f;
					float num7 = vector6.Y / 4f;
					Vector3 vector7 = Main.Instance.Script.Call<Vector3>("vectortoangles", new Parameter[]
					{
						vector5
					});
					vector7 = new Vector3(0f, vector7.Y, 90f);
					entity = Main.Instance.Script.Call<Entity>("spawn", new Parameter[]
					{
						"script_origin",
						new Vector3((start.X + end.X) / 2f, (start.Y + end.Y) / 2f, (start.Z + end.Z) / 2f)
					});
					for (int j = 0; j < num5; j++)
					{
						Entity entity2 = this.SpawnCrate(start + new Vector3(num6, num7, 10f) + new Vector3(0f, 0f, vector6.Z) * (float)j, vector7, flag2 ? null : model_type);
						entity2.Call("enablelinkto", new Parameter[0]);
						entity2.Call("linkto", new Parameter[]
						{
							entity
						});
						for (int k = 0; k < num4; k++)
						{
							Entity entity3 = this.SpawnCrate(start + new Vector3(vector6.X, vector6.Y, 0f) * (float)k + new Vector3(0f, 0f, 10f) + new Vector3(0f, 0f, vector6.Z) * (float)j, vector7, flag2 ? null : model_type);
							entity3.Call("enablelinkto", new Parameter[0]);
							entity3.Call("linkto", new Parameter[]
							{
								entity
							});
							/*if (model_type == "com_plasticcase_beige_big" || model_type == "com_plasticcase_green_big_us_dirt")
							{
								entity3.Call(33353, new Parameter[]
								{
								this._testCollision
								});
							}*/
						}
						Entity entity4 = this.SpawnCrate(new Vector3(end.X, end.Y, start.Z) + new Vector3(num6 * -1f, num7 * -1f, 10f) + new Vector3(0f, 0f, vector6.Z) * (float)j, vector7, flag2 ? null : model_type);
						entity4.Call("enablelinkto", new Parameter[0]);
						entity4.Call("linkto", new Parameter[]
						{
							entity
						});
						if (model_type == "com_plasticcase_beige_big" || model_type == "com_plasticcase_green_big_us_dirt")
						{
							/*
							entity4.Call(33353, new Parameter[]
							{
								this._testCollision
							});*/
						}
					}
					flag = true;
				}
				if (param == "Floor" && !flag)
				{
					this.CreateFloor(Start, End, flag2, model_type);
				}
				if (param == "Zipline")
				{
					Entity entity5 = Main.Instance.Script.Call<Entity>("spawn", new Parameter[]
					{
						"script_model",
						vector
					});

					entity5.Call("setmodel", new Parameter[]
					{
						"com_plasticcase_enemy"
					});

					entity5.SetField("angles", new Parameter(vector2));

					int id = model.Id;
					if (!hide)
					{
						Main.Instance.Script.Call(431, new Parameter[]
						{
							id,
							"active"
						});
						Main.Instance.Script.Call(435, new Parameter[]
						{
							id,
							new Parameter(entity5.Origin)
						});
						if (this.ContainsParam(model, "icon") && !hide)
						{
							Main.Instance.Script.Call(434, new Parameter[]
							{
								id,
								this.GetParam(model, "icon")
							});
						}
					}

					entity5.SetField("type", "Zipline");
					entity5.SetField("PressToUse", PressToUse.ToLower());
					entity5.SetField("start", Start);
					entity5.SetField("end", End);
					this.SpawnedModels.Add(entity5);
				}
				if (param == "Teleport")
				{
					Entity entity5 = Main.Instance.Script.Call<Entity>("spawn", new Parameter[]
					{
						"script_model",
						Start
					});
					Entity entity6 = Main.Instance.Script.Call<Entity>("spawn", new Parameter[]
					{
						"script_model",
						End
					});
					Entity entity7 = Main.Instance.Script.Call<Entity>("spawn", new Parameter[]
					{
						"script_model",
						Start
					});
					int id = model.Id;
					if (!flag2)
					{
						entity5.Call("setModel", new Parameter[]
						{
							this.GetFlagModel(Common._mapnameFull(false))
						});
						entity6.Call("setModel", new Parameter[]
						{
							"weapon_oma_pack"
						});
						if (Members_Only.ToLower() == "true")
						{
							entity7.Call("setmodel", new Parameter[]
							{
								"tag_origin"
							});
						}
						else
						{
							entity7.Call("setmodel", new Parameter[]
							{
								"weapon_scavenger_grenadebag"// weapon_scavenger_grenadebag
							});
						}
						Main.Instance.Script.Call(431, new Parameter[]
						{
							id,
							"active"
						});
						Main.Instance.Script.Call(435, new Parameter[]
						{
							id,
							new Parameter(entity5.Origin)
						});
						Main.Instance.Script.Call(434, new Parameter[]
						{
							id,
							"compass_waypoint_bomb"// iw5_cardicon_bomb
						});
					}
					else
					{
						entity7 = null;
						entity5.Call("setModel", new Parameter[]
						{
							"weapon_scavenger_grenadebag"
						});
						entity6.Call("setModel", new Parameter[]
						{
							"weapon_oma_pack"
						});
					}
					if (this.ContainsParam(model, "model-enter"))
					{
						entity5.Call("setmode", new Parameter[]
						{
							this.GetParam(model, "model-enter")
						});
					}
					if (this.ContainsParam(model, "model-exit"))
					{
						entity6.Call("setmode", new Parameter[]
						{
							this.GetParam(model, "model-exit")
						});
					}
					if (this.ContainsParam(model, "model-enter-ext") && !flag2)
					{
						entity7.Call("setmode", new Parameter[]
						{
							this.GetParam(model, "model-enter-ext")
						});
					}
					if (this.ContainsParam(model, "icon") && !flag2)
					{
						Main.Instance.Script.Call(434, new Parameter[]
						{
							id,
							this.GetParam(model, "icon")
						});
					}
					Main.Instance.Script.OnInterval(100, delegate
					{
						foreach (Entity player in Main.Instance.Script.Players)
						{
							if (player == null)
							{
								return false;
							}
							float distance = 50f;
							if (player.Origin.DistanceTo(Start) <= distance)
							{
								if (PressToUse.ToLower() == "false" || PressToUse.ToLower() == "off")
								{
									if (Members_Only.ToLower() == "true" || Members_Only.ToLower() == "yes")
									{
										if (player.Origin.DistanceTo(Start) <= distance)
										{
											if (Common.GetPlayerTeam(player) == "axis" && Allow_infected_Members.ToLower() != "true")
											{
												//To do
											}
											else
											{
												if (player.GetField<string>("IsMember") == "true" || player.GetField<string>("IsAdmin") == "true")
												{
													player.Call("setorigin", new Parameter[]
													{
														new Parameter(End)
													});
													if (player.GetField<Vector3>("LastLocationPoint").ToString() != End.ToString())
													{
														player.SetField("LastLocationPoint", End);
														Utilities.RawSayTo(player, "Location ^2saved!");
														Log.Debug(player.Name + " Location saved");
													}
												}
											}
										}
									}
									else
									{
										player.Call("setorigin", new Parameter[]
										{
											new Parameter(End)
										});
										if (player.GetField<Vector3>("LastLocationPoint").ToString() != End.ToString())
										{
											player.SetField("LastLocationPoint", End);
											Utilities.RawSayTo(player, "Location ^2saved!");
											Log.Debug(player.Name + " Location saved");
										}
									}
								}
							}
						}
						return true;
					});
					entity5.SetField("type", "Teleport");
					entity5.SetField("MembersOnly", Members_Only.ToLower());
					entity5.SetField("PressToUse", PressToUse.ToLower());
					entity5.SetField("AllowINFmembers", Allow_infected_Members.ToLower());
					entity5.SetField("start", Start);
					entity5.SetField("end", End);
					this.SpawnedModels.Add(entity5);
				}
				if (param == "Door" && !flag)
				{
					int num8 = int.Parse(this.GetParam(model, "Size"));
					int v = int.Parse(this.GetParam(model, "Health"));
					int v2 = int.Parse(this.GetParam(model, "Heigth"));
					int v3 = int.Parse(this.GetParam(model, "Size"));
					int num9 = int.Parse(this.GetParam(model, "Heigth"));
					double num10 = ((double)(num8 / 2) - 0.5) * -1.0;

					Entity entity8 = Main.Instance.Script.Call<Entity>("spawn", new Parameter[]
					{
						"script_model",
						new Parameter(Start)
					});
					for (int l = 0; l < num8; l++)
					{
						Entity entity9 = this.SpawnCrate(Start + new Vector3(0f, 30f, 0f) * (float)num10, new Vector3(0f, 0f, 0f), null);
						entity9.Call("setModel", new Parameter[]
						{
							model_type
						});
						entity9.Call("enablelinkto", new Parameter[]
						{
							0
						});
						entity9.Call("linkto", new Parameter[]
						{
							entity8
						});
						for (int m = 1; m < num9; m++)
						{
							Entity entity10 = this.SpawnCrate(Start + new Vector3(0f, 30f, 0f) * (float)num10 - new Vector3(70f, 0f, 0f) * (float)m, new Vector3(0f, 0f, 0f), null);
							entity10.Call("setModel", new Parameter[]
							{
								Utilitys._DoorModel
							});
							entity10.Call("enablelinkto", new Parameter[]
							{
								0
							});
							entity10.Call("linkto", new Parameter[]
							{
								entity8
							});
						}
						num10 += 1.0;
					}
					entity8.SetField("type", "Door");
					entity8.SetField("range", 100);
					entity8.SetField("angles", new Parameter(vector2));
					entity8.SetField("state", "open");
					entity8.SetField("hp", v);
					entity8.SetField("maxhp", v);
					entity8.SetField("Heigth", v2);
					entity8.SetField("Size", v3);
					entity8.SetField("open", new Parameter(Start));
					entity8.SetField("close", new Parameter(End));
					this.SpawnedModels.Add(entity8);
					flag = true;
				}
				if (param == "Elevator" && !flag)
				{
					Entity ent1 = Main.Instance.Script.Call<Entity>("spawn", new Parameter[]
					{
						"script_origin",
						Start
					});
					Entity entity11 = this.CreateFloor(new Vector3(Start.X - 45f, Start.Y - 45f, Start.Z), new Vector3(Start.X + 45f, Start.Y + 45f, Start.Z), flag2, model_type);
					entity11.Call("enablelinkto", new Parameter[]
					{
						0
					});
					entity11.Call("linkto", new Parameter[]
					{
						ent1
					});
					ent1.SetField("currentPos", "pos1");
					ent1.SetField("pos1", Start);
					ent1.SetField("pos2", End);
					Action action = null;
					Func<bool> func = null;
					Action action2 = null;
					Main.Instance.Script.OnInterval(5000, delegate ()
					{
						if (ent1.GetField<string>("currentPos") == "pos1")
						{
							Main.Instance.Script.Call("playsoundatpos", new Parameter[]
							{
								ent1.Origin,
								"elev_run_start"
							});

							ent1.Call("moveto", new Parameter[]
							{
								ent1.GetField<Vector3>("pos2"),
								2f
							});
							BaseScript script = Main.Instance.Script;
							int num12 = 500;
							if (action == null)
							{
								action = (action = delegate ()
								{
									ent1.SetField("currentPos", "pos2");
								});
							}
							script.AfterDelay(num12, action);
							BaseScript script2 = Main.Instance.Script;
							int num13 = 50;
							if (func == null)
							{
								func = (func = delegate ()
								{
									foreach (Entity entity12 in Main.Instance.Script.Players)
									{
										if (entity12.Origin.DistanceTo(ent1.Origin) <= 80f)
										{
											entity12.Call("setorigin", new Parameter[]
											{
												ent1.Origin + new Vector3(0f, 0f, 15f)
											});
										}
									}
									if (ent1.Origin.ToString() == ent1.GetField<Vector3>("pos2").ToString())
									{
										Main.Instance.Script.Call("playsoundatpos", new Parameter[]
										{
											ent1.Origin,
											"elev_bell_ding"
										});
										return false;
									}
									return true;
								});
							}
							script2.OnInterval(num13, func);
						}
						else
						{
							ent1.Call("moveto", new Parameter[]
							{
								ent1.GetField<Vector3>("pos1"),
								2
							});
							BaseScript script3 = Main.Instance.Script;
							int num14 = 500;
							if (action2 == null)
							{
								action2 = (action2 = delegate ()
								{
									ent1.SetField("currentPos", "pos1");
								});
							}
							script3.AfterDelay(num14, action2);
						}
						return true;
					});
				}
			}
			if (entity != null)
			{
				foreach (KeyValuePair<string, string> keyValuePair in model.Parameters)
				{
					Vector3 right;
					Vector3 right2;
					if (keyValuePair.Key == "origin" && this.TryParse(keyValuePair.Value, out right))
					{
						entity.SetField("origin", vector + right);
					}
					else if (keyValuePair.Key == "angles" && this.TryParse(keyValuePair.Value, out right2))
					{
						entity.SetField("angles", vector2 + right2);
					}
					else if (keyValuePair.Key.StartsWith("precache"))
					{
						Main.Instance.Script.Call(keyValuePair.Key, new Parameter[]
						{
							keyValuePair.Value
						});
					}
					else if (keyValuePair.Key.StartsWith("call|"))
					{
						entity.Call(keyValuePair.Key.Split(new char[]
						{
							'|'
						})[1], new Parameter[]
						{
							keyValuePair.Value
						});
					}
					else
					{
						entity.SetField(keyValuePair.Key, keyValuePair.Value);
					}
				}
				this.SpawnedModels.Add(entity);
			}
			return entity;
		}

		public Entity CreateFloor(Vector3 corner1, Vector3 corner2, bool hidden, string model)
		{
			float num = corner1.X - corner2.X;
			if (num < 0f)
			{
				num *= -1f;
			}
			float num2 = corner1.Y - corner2.Y;
			if (num2 < 0f)
			{
				num2 *= -1f;
			}
			int num3 = (int)Math.Round((double)(num / 50f), 0);
			int num4 = (int)Math.Round((double)(num2 / 30f), 0);
			Vector3 vector = corner2 - corner1;
			Vector3 vector2 = new Vector3(vector.X / (float)num3, vector.Y / (float)num4, 0f);
			Entity entity = Main.Instance.Script.Call<Entity>("spawn", new Parameter[]
			{
				"script_origin",
				new Vector3((corner1.X + corner2.X) / 2f, (corner1.Y + corner2.Y) / 2f, corner1.Z)
			});
			for (int i = 0; i < num3; i++)
			{
				for (int j = 0; j < num4; j++)
				{
					Entity entity2 = this.SpawnCrate(corner1 + new Vector3(vector2.X, 0f, 0f) * (float)i + new Vector3(0f, vector2.Y, 0f) * (float)j, new Vector3(0f, 0f, 0f), hidden ? null : model);
					entity2.Call("enablelinkto", new Parameter[]
					{
						0
					});
					entity2.Call("linkto", new Parameter[]
					{
						entity
					});
				}
			}
			return entity;
		}

		public Entity SpawnCrate(Vector3 origin, Vector3 angles, string model = null)
		{
			Entity entity = Main.Instance.Script.Call<Entity>("spawn", new Parameter[]
			{
				"script_model",
				origin
			});
			entity.SetField("angles", angles);
			entity.Call(33353, new Parameter[]
			{
				this._airdropCollision
			});
			if (model != null)
			{
				entity.Call("setmodel", new Parameter[]
				{
					model
				});
			}
			return entity;
		}

		public string GetFlagModel(string mapname)
		{
			switch (mapname)
			{
				case "mp_alpha":
				case "mp_dome":
				case "mp_hardhat":
				case "mp_interchange":
				case "mp_cement":
				case "mp_crosswalk_ss":
				case "mp_hillside_ss":
				case "mp_morningwood":
				case "mp_overwatch":
				case "mp_park":
				case "mp_qadeem":
				case "mp_restrepo_ss":
				case "mp_terminal_cls":
				case "mp_roughneck":
				case "mp_boardwalk":
				case "mp_moab":
				case "mp_nola":
				case "mp_radar":
				case "mp_six_ss":
					return "prop_flag_delta";
				case "mp_exchange":
					return "prop_flag_american05";
				case "mp_bootleg":
				case "mp_bravo":
				case "mp_mogadishu":
				case "mp_village":
				case "mp_shipbreaker":
					return "prop_flag_pmc";
				case "mp_paris":
					return "prop_flag_gign";
				case "mp_plaza2":
				case "mp_aground_ss":
				case "mp_courtyard_ss":
				case "mp_italy":
				case "mp_meteora":
				case "mp_underground":
					return "prop_flag_sas";
				case "mp_seatown":
				case "mp_carbon":
				case "mp_lambeth":
					return "prop_flag_seal";
			}
			return "";
		}

		public void HandleUseables(Entity player)
		{
			if (player.GetField<string>("sessionteam") == "spectator")
			{
				return;
			}
			foreach (Entity entity in this.SpawnedModels)
			{
				if (player.Origin.DistanceTo(entity.Origin) <= 100f)
				{
					switch (entity.GetField<string>("type"))
					{
						case "Door":
							this.usedDoor(entity, player);
							break;
						default:
							break;
					}
				}
				if (player.Origin.DistanceTo(entity.Origin) <= 50f)
				{
					switch (entity.GetField<string>("type"))
					{
						case "Trampoline":
							this.useTrampoline(entity, player);
							break;
						case "Winner":
							this.useChampionParkour(entity, player);
							break;
						case "Teleport":
							this.useTeleport(entity, player);
							break;
						case "Zipline":
							this.useZipline(entity, player);
							break;
						default:
							break;
					}
				}
			}
		}

		public static bool playerIsMember(Entity player)
		{
			bool _isM = false;
			if (player.HasField("isMember") && player.GetField<string>("isMember") == "true")
			{
				_isM = true;
			}
			return _isM;
		}

		public void UsablesHud(Entity player)
		{
			HudElem message = HudElem.CreateFontString(player, "hudbig", 0.6f);
			message.SetPoint("CENTER", "CENTER", 0, 70);
			Main.Instance.Script.OnInterval(100, delegate
			{
				bool flag = false;
				foreach (Entity entity in this.SpawnedModels)
				{
					if (player.Origin.DistanceTo(entity.Origin) <= 100f)
					{
						switch (entity.GetField<string>("type"))
						{
							case "Door":
								message.SetText(this.getDoorText(entity, player));
								flag = true;
								break;
							default:
								message.SetText("");
								break;
						}
					}
					if (player.Origin.DistanceTo(entity.Origin) <= 50f)
					{
						switch (entity.GetField<string>("type"))
						{
							case "Teleport":
								if (entity.HasField("PressToUse") && entity.GetField<string>("PressToUse") == "true")
								{
									if (entity.GetField<string>("MembersOnly") == "true")
									{
										if (player.HasField("isMember") && player.GetField<string>("isMember") == "true")
										{
											if (Common.GetPlayerTeam(player) == "axis" && entity.GetField<string>("AllowINFmembers").ToLower() != "true")
											{
												message.SetText("You cannot use this ^3flag ^7because you are infected");
											}
											else
											{
												message.SetText("Press ^3[{+activate}] ^7to use the Teleport.");
											}
										}
									}
									else
									{
										message.SetText("Press ^3[{+activate}] ^7to use the Teleport.");
									}
								}
								flag = true;
								break;
							case "Trampoline":
								if (player.IsAlive || Common.GetPlayerTeam(player) != "spectator")
								{
									if (entity.HasField("PressToUse") && entity.GetField<string>("PressToUse").ToLower() == "true")
									{
										message.SetText("Press ^3[{+activate}] ^7to use the Trampoline");
									}
									else
									{
										Vector3 vector = player.Call<Vector3>("getvelocity", new Parameter[0]);
										player.Call("setvelocity", new Parameter[]
										{
											new Vector3(vector.X, vector.Y, float.Parse(entity.GetField<string>("Height")))
										});
									}
								}
								flag = true;
								break;
							case "Winner":
								if (player.IsAlive || Common.GetPlayerTeam(player) != "spectator")
								{
									if (entity.HasField("PressToUse") && entity.GetField<string>("PressToUse").ToLower() == "true")
									{
										message.SetText("To guarantee your victory Press ^3[{+activate}]");
									}
									else
									{
										/*foreach (Entity playerIn in Main.Instance.Script.Players)
										{
											playerIn.Call("iprintlnbold", new Parameter[]
											{
												player.Name + "^2 Has finish the parkour"
											});
										}*/
									}
								}
								flag = true;
								break;
							case "Zipline":
								if (player.IsAlive || Common.GetPlayerTeam(player) != "spectator")
								{
									if (entity.HasField("PressToUse") && entity.GetField<string>("PressToUse").ToLower() == "true")
									{
										message.SetText("Press ^3[{+activate}] ^7to use zipline.");
									}
								}
								flag = true;
								break;
							default:
								message.SetText("");
								break;
						}
					}
				}
				if (!flag)
				{
					message.SetText("");
				}
				return true;
			});
		}

		public string getDoorText(Entity door, Entity player)
		{
			int field = door.GetField<int>("hp");
			int field2 = door.GetField<int>("maxhp");
			int field3 = door.GetField<int>("Heigth");
			int field4 = door.GetField<int>("Size");
			if (player.GetField<string>("sessionteam") == "allies")
			{
				string field5 = door.GetField<string>("state");
				if (!(field5 == "open"))
				{
					if (!(field5 == "close"))
					{
						if (field5 == "broken")
						{
							if (player.CurrentWeapon == "defaultweapon_mp")
							{
								return string.Concat(new object[]
								{
									"Door is Broken. Press ^3[{+activate}] ^7to repair it. (",
									field,
									"/",
									field2,
									")"
								});
							}
							return "^1Door is Broken.";
						}
					}
					else
					{
						if (player.CurrentWeapon == "defaultweapon_mp")
						{
							return string.Concat(new object[]
							{
								"Door is Closed. Press ^3[{+activate}] ^7to repair it. (",
								field,
								"/",
								field2,
								")"
							});
						}
						return string.Concat(new object[]
						{
							"Door is Closed. Press ^3[{+activate}] ^7to open it. (",
							field,
							"/",
							field2,
							")"
						});
					}
				}
				else
				{
					if (player.CurrentWeapon == "defaultweapon_mp")
					{
						return string.Concat(new object[]
						{
							"Door is Open. Press ^3[{+activate}] ^7to repair it. (",
							field,
							"/",
							field2,
							")"
						});
					}
					return string.Concat(new object[]
					{
						"Door is Open. Press ^3[{+activate}] ^7to close it. (",
						field,
						"/",
						field2,
						")"
					});
				}
			}
			else if (player.GetField<string>("sessionteam") == "axis")
			{
				string field6 = door.GetField<string>("state");
				if (field6 == "open")
				{
					return "Door is Open.";
				}
				if (field6 == "close")
				{
					return string.Concat(new string[]
					{
						"Press ^3[{+activate}] ^7to attack the door. (",
						field3.ToString(),
						"/",
						field4.ToString(),
						")"
					});
				}
				if (field6 == "broken")
				{
					return "^1Door is Broken .";
				}
			}
			return "";
		}

		private void repairDoor(Entity door, Entity player)
		{
			if (player.GetField<int>("repairsleft") == 0)
			{
				return;
			}
			if (door.GetField<int>("hp") < door.GetField<int>("maxhp"))
			{
				door.SetField("hp", door.GetField<int>("hp") + 1);
				player.SetField("repairsleft", player.GetField<int>("repairsleft") - 1);
				player.Call("iprintlnbold", new Parameter[]
				{
					"Repaired Door! (" + player.GetField<int>("repairsleft").ToString() + " repairs left)"
				});
				if (door.GetField<string>("state") == "broken")
				{
					door.Call(33399, new Parameter[]
					{
						new Parameter(door.GetField<Vector3>("close")),
						1
					});
					Main.Instance.Script.AfterDelay(300, delegate
					{
						door.SetField("state", "close");
					});
					return;
				}
			}
			else
			{
				player.Call("iprintlnbold", new Parameter[]
				{
					"Door has full health!"
				});
			}
		}


		private void useTrampoline(Entity tramp, Entity player)
		{
			if (tramp.HasField("PressToUse") && tramp.GetField<string>("PressToUse").ToLower() == "true")
			{
				Vector3 vector = player.Call<Vector3>("getvelocity", new Parameter[0]);
				player.Call("setvelocity", new Parameter[]
				{
					new Vector3(vector.X, vector.Y, float.Parse(tramp.GetField<string>("Height")))
				});
			}
		}

		private void useZipline(Entity zipl, Entity player)
		{
			//Move(zipl, player);
			if (zipl.HasField("PressToUse") && zipl.GetField<string>("PressToUse").ToLower() == "true")
			{
				Vector3 Start = zipl.GetField<Vector3>("start");
				Vector3 End = zipl.GetField<Vector3>("end");

				IsUsing = true;
				int MoveTime = 50;

				Log.Debug("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa");
				player.Call("playerlinkto", zipl);
				zipl.Call("moveto", End, MoveTime);

				Main.Instance.Script.AfterDelay(5000, delegate
				{
					Log.Debug("bruh");
				});

				Main.Instance.Script.AfterDelay(MoveTime * 1000, delegate
				{
					if (player.Call<int>("islinked") != 0)
					{
						player.Call("unlink");
						player.Call("setorigin", End);
					}
					zipl.Call("moveto", Start, 1);
				});
				Main.Instance.Script.AfterDelay(MoveTime * 1000 + 2000, delegate
				{
					IsUsing = false;
					zipl.Call("clonebrushmodeltoscriptmodel", _airdropCollision);
				});
			}
		}

		private void Move(Entity zipl, Entity player)
		{
			Vector3 start = zipl.GetField<Vector3>("start");
			Vector3 Exit = zipl.GetField<Vector3>("end");

			int MoveTime2 = 13;
			player.Call("playerlinkto", zipl);
			zipl.Call("moveto", Exit, MoveTime2);

			//Log.Debug(player.GetField<int>("islinked").ToString());

			Main.Instance.Script.AfterDelay(MoveTime2 * 2, delegate
			{
				if (player.Call<int>("islinked") != 0)
				{
					player.Call("unlink");
					player.Call("setorigin", Exit);
				}
				zipl.Call("moveto", start, 1);
			});
		}

		private void useTeleport(Entity tp, Entity player)
		{
			Vector3 Start = tp.GetField<Vector3>("start");
			Vector3 End = tp.GetField<Vector3>("end");

			if (tp.HasField("PressToUse") && tp.GetField<string>("PressToUse").ToLower() == "true")
			{
				if (tp.GetField<string>("MembersOnly").ToLower() == "true" || tp.GetField<string>("MembersOnly").ToLower() == "yes")
				{
					if (Common.GetPlayerTeam(player) == "axis" && tp.GetField<string>("AllowINFmembers").ToLower() != "true")
					{

					}
					else
					{
						if (player.GetField<string>("IsMember") == "true" || player.GetField<string>("IsAdmin") == "true")
						{
							player.Call("setorigin", new Parameter[]
							{
								new Parameter(End)
							});
							if (player.GetField<Vector3>("LastLocationPoint").ToString() != End.ToString())
							{
								player.SetField("LastLocationPoint", End);
								Utilities.RawSayTo(player, "Location ^2saved!");
								Log.Debug(player.Name + " Location saved");
							}
						}
					}
				}
				else
				{
					player.Call("setorigin", new Parameter[]
					{
						new Parameter(End)
					});
					if (player.GetField<Vector3>("LastLocationPoint").ToString() != End.ToString())
					{
						player.SetField("LastLocationPoint", End);
						Utilities.RawSayTo(player, "Location ^2saved!");
						Log.Debug(player.Name + " Location saved");
					}
				}
			}
		}

		private void useChampionParkour(Entity tramp, Entity player)
		{
			if (tramp.HasField("PressToUse") && tramp.GetField<string>("PressToUse").ToLower() == "true")
			{
				if (player.GetField<string>("finishPK") == "false")
				{
					_Score_Ranking++;

					if (_Score_Ranking == 1)
					{
						MapEdit.WinnerData.Add(player.Name, _Score_Ranking);

						foreach (KeyValuePair<string, int> Winner in MapEdit.WinnerData)
						{
							Log.Debug($"Winners: {Winner.Key} - {Winner.Value}");
						}
					}
					else
					{
						if (_Score_Ranking > 1)
						{
							if (_Score_Ranking <= 3)
							{
								MapEdit.WinnersData.Add(player.Name, _Score_Ranking);
							}
						}
					}

					player.SetField("PKranking", _Score_Ranking);
					Log.Debug(player.GetField<int>("PKranking").ToString());

					Parameter[] time = new Parameter[] { 100, 0x1b58, 600 };
					foreach (Entity playersOn in Main.Instance.Script.Players)
					{
						HudElem hudElem = HudElem.CreateFontString(playersOn, "objective", 1.8f);
						hudElem.SetPoint("CENTER", "CENTER", 0, -110);
						hudElem.SetText(player.Name + "^2 Has finish the parkour");
						hudElem.Call("SetPulseFX", time);
						hudElem.HideWhenInMenu = true;
						Main.Instance.Script.Call("playsoundatpos", new Parameter[]
						{
							playersOn.Origin,
							"new_title_unlocks"
						});
					}
					player.SetField("finishPK", "true");
					for (int i = 0; i < 5; i++)
					{
						Common.DropMoney(player.Origin);
					}
				}
				else
				{
					if (player.GetField<string>("finishPK") == "true")
					{
						player.Call("iprintlnbold", new Parameter[]
						{
							player.Name + "^2 You already finished the map"
						});
					}
				}
			}
		}

		private void usedDoor(Entity door, Entity player)
		{
			if (!player.IsAlive)
			{
				return;
			}
			if (player.CurrentWeapon.Equals("defaultweapon_mp"))
			{
				this.repairDoor(door, player);
				return;
			}
			if (door.GetField<int>("hp") > 0)
			{
				if (player.GetField<string>("sessionteam") == "allies")
				{
					if (door.GetField<string>("state") == "open")
					{
						door.Call(33399, new Parameter[]
						{
							door.GetField<Vector3>("close"),
							1
						});
						Main.Instance.Script.AfterDelay(300, delegate
						{
							door.SetField("state", "close");
						});
						return;
					}
					if (door.GetField<string>("state") == "close")
					{
						door.Call(33399, new Parameter[]
						{
							door.GetField<Vector3>("open"),
							1
						});
						Main.Instance.Script.AfterDelay(300, delegate
						{
							door.SetField("state", "open");
						});
						return;
					}
				}
				else if (player.GetField<string>("sessionteam") == "axis" && door.GetField<string>("state") == "close" && player.GetField<int>("attackeddoor") == 0)
				{
					int num = 0;
					string a = player.Call<string>("getstance", new Parameter[]
					{
						0
					});
					if (!(a == "prone"))
					{
						if (!(a == "couch"))
						{
							if (a == "stand")
							{
								num = 90;
							}
						}
						else
						{
							num = 45;
						}
					}
					else
					{
						num = 20;
					}
					if (this._rng.Next(100) < num)
					{
						door.SetField("hp", door.GetField<int>("hp") - 1);
						player.Call("iprintlnbold", new Parameter[]
						{
							string.Concat(new object[]
							{
								"Hit: ",
								door.GetField<int>("hp"),
								"/",
								door.GetField<int>("maxhp")
							})
						});
					}
					else
					{
						player.Call("iprintlnbold", new Parameter[]
						{
							"^1MISS"
						});
					}
					player.SetField("attackeddoor", 1);
					player.AfterDelay(1000, delegate (Entity e)
					{
						player.SetField("attackeddoor", 0);
					});
					return;
				}
			}
			else if (door.GetField<int>("hp") == 0 && door.GetField<string>("state") != "broken")
			{
				if (door.GetField<string>("state") == "close")
				{
					door.Call(33399, new Parameter[]
					{
						door.GetField<Vector3>("open"),
						1f
					});
				}
				door.SetField("state", "broken");
			}
		}

		public bool ContainsParam(ScriptModel.ModelInfo model, string key)
		{
			return model.Parameters.ContainsKey(key);
		}

		public string GetParam(ScriptModel.ModelInfo model, string key)
		{
			if (model.Parameters.ContainsKey(key))
			{
				return model.Parameters[key];
			}
			return null;
		}


		public Vector3 Parse(string str)
		{
			str = str.Replace(" ", string.Empty);
			if (!str.StartsWith("(") && !str.EndsWith(")"))
			{
				throw new Exception("Wrong Vector3 Format At " + str);
			}
			str = str.Replace("(", "").Replace(")", "");
			string[] array = str.Split(new char[]
			{
				','
			});
			if (array.Length < 3)
			{
				throw new Exception("Wrong Vector3 Format At " + str);
			}
			return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
		}

		public bool TryParse(string str, out Vector3 Vector3)
		{
			str = str.Replace(" ", string.Empty);
			if (!str.StartsWith("(") && !str.EndsWith(")"))
			{
				Vector3 = default(Vector3);
				return false;
			}
			str = str.Replace("(", "").Replace(")", "");
			string[] array = str.Split(new char[]
			{
				','
			});
			if (array.Length < 3)
			{
				Vector3 = default(Vector3);
				return false;
			}
			Vector3 = new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
			return true;
		}
	}
}
