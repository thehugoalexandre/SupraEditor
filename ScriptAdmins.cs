using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using InfinityScript;
using Newtonsoft.Json;
using Formatting = Newtonsoft.Json.Formatting;

namespace MapEdit
{
	public class ScriptAdmins
	{
		public class AdminsInfo
		{
			[JsonProperty("Id", Required = Required.Always)]
			public int Id;

			[JsonProperty("Name", Required = Required.Always)]
			public string Name;

			[JsonProperty("HWID", Required = Required.Always)]
			public string HWID;
			[JsonProperty("Parameters", Required = Required.Default)]
			public Dictionary<string, string> Parameters;
		}

		public class MembersInfo
		{
			[JsonProperty("Id", Required = Required.Always)]
			public int Id;

			[JsonProperty("Name", Required = Required.Always)]
			public string Name;

			[JsonProperty("HWID", Required = Required.Always)]
			public string HWID;

			[JsonProperty("Parameters", Required = Required.Default)]
			public Dictionary<string, string> Parameters;
		}

		public List<ScriptAdmins.AdminsInfo> AdminsList { get; private set; }
		public List<ScriptAdmins.MembersInfo> MembersList { get; private set; }
		public static string HudCredits { get; private set; }
		public static string CheckForUpdates { get; private set; }
		public static string OnPlayerConnectedAddTrail { get; private set; }

		public static string DiscordWebHook { get; private set; }

		public static string ClanTag;
		//public static string ClanTag { get; private set; }
		public ScriptAdmins()
		{
			this.AdminsList = new List<ScriptAdmins.AdminsInfo>();
			this.MembersList = new List<ScriptAdmins.MembersInfo>();
			this.Initialize();
		}

		public void Initialize()
		{
			bool flag = true;
			bool flag2 = true;
			if (!Directory.Exists(Utilitys.MapEditFolder))
			{
				Directory.CreateDirectory(Utilitys.MapEditFolder);
			}
			if (!Directory.Exists(Utilitys.ConfigFolder))
			{
				Directory.CreateDirectory(Utilitys.ConfigFolder);
			}
			if (!File.Exists(Utilitys.AdminsJson))
			{
				File.WriteAllLines(Utilitys.AdminsJson, new string[0]);
				flag = false;
			}
			if (!File.Exists(Utilitys.MembersJson))
			{
				File.WriteAllLines(Utilitys.MembersJson, new string[0]);
				flag2 = false;
			}
			if (this.AdminsList == null && MembersList == null)
			{
				this.AdminsList = new List<ScriptAdmins.AdminsInfo>();
				this.MembersList = new List<ScriptAdmins.MembersInfo>();
			}
			if (!flag)
			{
				this.AdminsList.Add(new ScriptAdmins.AdminsInfo
				{
					Id = -1,
					Name = "Admin Name",
					HWID = "00000000-00000000-00000000",
					Parameters = new Dictionary<string, string>
					{
						{
							"Perms",
							"NONE"
						}
					}
				});
				this.SaveAdmins();
			}
			if (!flag2)
			{
				this.MembersList.Add(new ScriptAdmins.MembersInfo
				{
					Id = -1,
					Name = "Member Name",
					HWID = "00000000-00000000-00000000",
					Parameters = new Dictionary<string, string>
					{
						{
							"IsMember",
							"NONE"
						}
					}
				});
				this.SaveMembers();
			}
			this.LoadAdmins();
			this.LoadMembers();

			if (!File.Exists(Utilitys.IniFile))
			{
				File.Create(Utilitys.IniFile).Close();
				File.WriteAllLines(Utilitys.IniFile, Utilitys.ConfigXML_Contents);
				XmlDocument xmlDocument = new XmlDocument();
				xmlDocument.Load(Utilitys.IniFile);
				XmlNodeList elementsByTagName = xmlDocument.GetElementsByTagName("Setting");
				foreach (object obj in elementsByTagName)
				{
					XmlNode xmlNode = (XmlNode)obj;
					string value = xmlNode.Attributes.GetNamedItem("Name").Value;
					if (value == "HudCredits")
					{
						HudCredits = xmlNode.InnerText.ToLower();
					}
					if (value == "CheckForUpdates")
					{
						CheckForUpdates = xmlNode.InnerText.ToLower();
					}
					if (value == "OnPlayerConnectedAddTrail")
					{
						OnPlayerConnectedAddTrail = xmlNode.InnerText.ToLower();
					}
					if (value == "ClanTag")
					{
						ClanTag = xmlNode.InnerText;
					}
					if (value == "DiscordWebHook")
					{
						DiscordWebHook = xmlNode.InnerText;
					}
				}
				return;
			}
			else if (Common.EmptyFile(Utilitys.IniFile))
			{
				File.Delete(Utilitys.IniFile);
				File.Create(Utilitys.IniFile).Close();
				File.WriteAllLines(Utilitys.IniFile, Utilitys.ConfigXML_Contents);
				XmlDocument xmlDocument2 = new XmlDocument();
				xmlDocument2.Load(Utilitys.IniFile);
				XmlNodeList elementsByTagName2 = xmlDocument2.GetElementsByTagName("Setting");
				foreach (object obj2 in elementsByTagName2)
				{
					XmlNode xmlNode2 = (XmlNode)obj2;
					string value2 = xmlNode2.Attributes.GetNamedItem("Name").Value;
					if (value2 == "HudCredits")
					{
						HudCredits = xmlNode2.InnerText.ToLower();
					}
					if (value2 == "CheckForUpdates")
					{
						CheckForUpdates = xmlNode2.InnerText.ToLower();
					}
					if (value2 == "OnPlayerConnectedAddTrail")
					{
						OnPlayerConnectedAddTrail = xmlNode2.InnerText.ToLower();
					}
					if (value2 == "ClanTag")
					{
						ClanTag = xmlNode2.InnerText;
					}
					if (value2 == "DiscordWebHook")
					{
						DiscordWebHook = xmlNode2.InnerText;
					}
				}
				return;
			}
			else
			{
				XmlDocument xmlDocument3 = new XmlDocument();
				xmlDocument3.Load(Utilitys.IniFile);
				XmlNodeList elementsByTagName3 = xmlDocument3.GetElementsByTagName("Setting");
				foreach (object obj3 in elementsByTagName3)
				{
					XmlNode xmlNode3 = (XmlNode)obj3;
					string value3 = xmlNode3.Attributes.GetNamedItem("Name").Value;
					if (value3 == "HudCredits")
					{
						HudCredits = xmlNode3.InnerText.ToLower();
					}
					if (value3 == "CheckForUpdates")
					{
						CheckForUpdates = xmlNode3.InnerText.ToLower();
					}
					if (value3 == "OnPlayerConnectedAddTrail")
					{
						OnPlayerConnectedAddTrail = xmlNode3.InnerText.ToLower();
					}
					if (value3 == "ClanTag")
					{
						ClanTag = xmlNode3.InnerText;
					}
					if (value3 == "DiscordWebHook")
					{
						DiscordWebHook = xmlNode3.InnerText;
					}
				}
			}
		}

		public void LoadAdmins()
		{
			if (this.AdminsList == null)
			{
				this.AdminsList = new List<ScriptAdmins.AdminsInfo>();
			}
			if (File.Exists(Utilitys.AdminsJson))
			{
				this.AdminsList = JsonConvert.DeserializeObject<List<ScriptAdmins.AdminsInfo>>(File.ReadAllText(Utilitys.AdminsJson));
			}
		}

		public void LoadMembers()
		{
			if (this.MembersList == null)
			{
				this.MembersList = new List<ScriptAdmins.MembersInfo>();
			}
			if (File.Exists(Utilitys.MembersJson))
			{
				this.MembersList = JsonConvert.DeserializeObject<List<ScriptAdmins.MembersInfo>>(File.ReadAllText(Utilitys.MembersJson));
			}
		}

		public void SaveAdmins()
		{
			if (this.AdminsList != null && this.AdminsList.Count > 0)
			{
				File.WriteAllText(Utilitys.AdminsJson, JsonConvert.SerializeObject(this.AdminsList, Formatting.Indented));
			}
		}

		public void SaveMembers()
		{
			if (this.MembersList != null && this.MembersList.Count > 0)
			{
				File.WriteAllText(Utilitys.MembersJson, JsonConvert.SerializeObject(this.MembersList, Formatting.Indented));
			}
		}

		public void ReloadAdmins()
		{
			this.SaveAdmins();
			this.LoadAdmins();
		}

		public void Add(string name, string hwid, Dictionary<string, string> dict)
		{
			this.Add(new ScriptAdmins.AdminsInfo
			{
				Id = this.AdminsList.Count - 1,
				Name = name,
				HWID = hwid,
				Parameters = dict
			});
		}

		public void Add(ScriptAdmins.AdminsInfo model)
		{
			this.AdminsList.Add(model);
			this.SaveAdmins();
		}

		public void AddToMembers(string name, string hwid)
		{
			this.AddToMembers(new ScriptAdmins.MembersInfo
			{
				Id = this.AdminsList.Count - 1,
				Name = name,
				HWID = hwid,
				Parameters = new Dictionary<string, string>
				{
					{
						"IsMember",
						"True"
					}
				}
			});
		}

		public void AddToMembers(ScriptAdmins.MembersInfo model)
		{
			this.MembersList.Add(model);
			this.SaveMembers();
		}

		public ScriptAdmins.AdminsInfo Get(int id)
		{
			if (!this.Exists(id))
			{
				return null;
			}
			return (from x in this.AdminsList where x.Id == id select x).FirstOrDefault<ScriptAdmins.AdminsInfo>();
		}

		public ScriptAdmins.AdminsInfo Get(string hwid)
		{
			if (!this.Exists(hwid, false))
			{
				return null;
			}
			return (from x in this.AdminsList where x.HWID == hwid select x).FirstOrDefault<ScriptAdmins.AdminsInfo>();
		}

		public ScriptAdmins.MembersInfo GetFromMembers(string hwid)
		{
			if (!this.Exists2(hwid, false))
			{
				return null;
			}
			return (from x in this.MembersList where x.HWID == hwid select x).FirstOrDefault<ScriptAdmins.MembersInfo>();
		}

		public List<ScriptAdmins.AdminsInfo> GetAll(string name)
		{
			if (!this.Exists(name, false))
			{
				return new List<ScriptAdmins.AdminsInfo>();
			}
			return (from x in this.AdminsList where x.Name.ToLower().Contains(name.ToLower()) select x).ToList<ScriptAdmins.AdminsInfo>();
		}

		public void Remove(int id)
		{
			if (!this.Exists(id))
			{
				return;
			}
			this.Remove((from x in this.AdminsList where x.Id == id select x).FirstOrDefault<ScriptAdmins.AdminsInfo>());
		}

		public void Remove(string hwid)
		{
			if (!this.Exists(hwid, false))
			{
				return;
			}
			this.Remove((from x in this.AdminsList where x.HWID == hwid select x).FirstOrDefault<ScriptAdmins.AdminsInfo>());
		}

		Func<ScriptAdmins.AdminsInfo, bool> test;
		public void Remove(ScriptAdmins.AdminsInfo admininfo)
		{
			this.AdminsList.Remove(admininfo);
			IEnumerable<ScriptAdmins.AdminsInfo> adminsList = this.AdminsList;
			Func<ScriptAdmins.AdminsInfo, bool> predicate;
			predicate = test;
			if (predicate == null)
			{
				predicate = (test = ((ScriptAdmins.AdminsInfo x) => x.Id > admininfo.Id));
			}
			foreach (ScriptAdmins.AdminsInfo adminsInfo in adminsList.Where(predicate))
			{
				adminsInfo.Id--;
			}
			this.SaveAdmins();
		}

		public bool Exists(int id)
		{
			using (List<ScriptAdmins.AdminsInfo>.Enumerator enumerator = this.AdminsList.GetEnumerator())
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

		public bool Exists(string str, bool name = false)
		{
			foreach (ScriptAdmins.AdminsInfo adminsInfo in this.AdminsList)
			{
				if (name)
				{
					if (adminsInfo.Name.ToLower().Contains(str.ToLower()))
					{
						return true;
					}
				}
				else if (adminsInfo.HWID == str)
				{
					return true;
				}
			}
			return false;
		}

		public bool Exists2(string str, bool name = false)
		{
			foreach (ScriptAdmins.MembersInfo membersInfo in this.MembersList)
			{
				if (name)
				{
					if (membersInfo.Name.ToLower().Contains(str.ToLower()))
					{
						return true;
					}
				}
				else if (membersInfo.HWID == str)
				{
					return true;
				}
			}
			return false;
		}
	}
}
