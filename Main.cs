using System;
using System.IO;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;
using InfinityScript;

namespace MapEdit
{
	public sealed class Main
	{
		private static Main instance = new Main();
		public MapEdit Script;
		public CommandManager CommandManager;
		public ScriptModel ScriptModel;
		public ScriptAdmins ScriptAdmins;
		public Common Common;
		public Webhook Webhook;

		public static Main Instance { get { return Main.instance; } }
		public class UpdateData
		{
			public string version { get; set; }
			public string downloadfile { get; set; }
			public string newfilename { get; set; }
		}

		[DllImport("wininet.dll")]
		private extern static Boolean InternetGetConnectedState(out int Description, int ReservedValue);

		public static string _dllVersion;
		public static string _dllName;

		public static string website = "put here your website";

		private static WebClient webClient;

		public void InitMain(MapEdit script)
		{
			try
			{
				Version version = base.GetType().Assembly.GetName().Version;
				_dllVersion = version.ToString();

				string dllName = base.GetType().Assembly.GetName().Name;
				_dllName = dllName;

				Log.Info("Initializing MapEdit...");
				this.Script = script;
				Log.Info("Initializing CommandManager...");
				this.CommandManager = new CommandManager();
				Log.Info("Initializing ScriptModel...");
				this.ScriptModel = new ScriptModel();
				Log.Info("Initializing ScriptAdmins...");
				this.ScriptAdmins = new ScriptAdmins();
				Log.Info("Initializing Common...");
				this.Common = new Common();

				if (ScriptAdmins.CheckForUpdates.ToLower() == "true" || ScriptAdmins.CheckForUpdates.ToLower() == "yes" || ScriptAdmins.CheckForUpdates.ToLower() == "enable")
				{
					if (IsConnected())
					{
						checkForUpdates(website);
					}
					else
					{
						Common.WriteLog.Info(_dllName + " - Your device is not connected to the internet. To look for updates, you need to have an Internet connection.");
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("There was an error while initializing script: {0}", new object[]
				{
					ex.Message
				});
			}
		}

		public static Boolean IsConnected()
		{
			int Description;
			return InternetGetConnectedState(out Description, 0);
		}

		public static bool checkForUpdates(string url)
		{
			HttpWebRequest webRequest = WebRequest.Create(url) as HttpWebRequest;
			webRequest.Method = "GET";
			HttpWebResponse response = webRequest.GetResponse() as HttpWebResponse;

			try
			{
				if (response.StatusCode == HttpStatusCode.OK)
				{
					StreamReader reader = new StreamReader(response.GetResponseStream());
					string _UpdateData_json = reader.ReadToEnd();
					UpdateData json = Newtonsoft.Json.JsonConvert.DeserializeObject<UpdateData>(_UpdateData_json);

					if (json.version != _dllVersion)
					{
						if (!Directory.Exists(Utilitys.UpdatesFolder))
						{
							Directory.CreateDirectory(Utilitys.UpdatesFolder);
						}
						if (!Directory.Exists(Utilitys.UpdatesFolder + "/" + json.version.ToString()))
						{
							Directory.CreateDirectory(Utilitys.UpdatesFolder + "/" + json.version.ToString());

							Main.Instance.Script.AfterDelay(1000, () => {
								string newUpdate = Environment.CurrentDirectory + "/" + Utilitys.UpdatesFolder + "/" + json.version.ToString() + "/" + json.newfilename;
								DownloadFile(json.downloadfile, newUpdate);
								Common.WriteLog.Info(_dllName + " - Last version downloaded successfully. (:");
							});
						}
					}
					else
					{
						Common.WriteLog.Info(_dllName + " - You have the latest version.");
					}
				}
				else
				{
					Common.WriteLog.Info(_dllName + " - Unable to search for updates.");
				}

				response.Close();
			}
			catch
			{
				return false;
			}
			return true;
		}

		public static bool DownloadFile(string urlAddress, string location)
		{
			using (webClient = new WebClient())
			{
				Uri URL = new Uri(urlAddress);
				try
				{
					Thread thread = new Thread(delegate ()
					{
						webClient.DownloadFileAsync(URL, location);
					});
					thread.Start();
				}
				catch (Exception)
				{
					Common.WriteLog.Info(_dllName + " - There was a problem because the new script cannot be download :: Failed");
					return false;
				}
			}
			return true;
		}
	}
}