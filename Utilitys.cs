using InfinityScript;
using System.Collections.Generic;

namespace MapEdit
{
	public class Utilitys
	{
		public static readonly string MapEditFolder = "scripts/SupraEditor";

		public static readonly string ConfigFolder = MapEditFolder + "/Config";

		public static readonly string AdminsJson = ConfigFolder + "/Admins.json";

		public static readonly string MembersJson = ConfigFolder + "/Members.json";

		public static readonly string IniFile = ConfigFolder + "/Config.xml";

		public static string[] ConfigXML_Contents = new string[]
		{
			"<Settings>",
			"\t<Setting Name=\"ClanTag\">^1Tekno</Setting>",
			"\t<Setting Name=\"HudCredits\">Enable</Setting>",
			"\t<Setting Name=\"CheckForUpdates\">False</Setting>",
			"\t<Setting Name=\"OnPlayerConnectedAddTrail\">True</Setting>",
			"\t<Setting Name=\"DiscordWebHook\"></Setting>",
			"</Settings>"
		};

		public static readonly string UpdatesFolder = MapEditFolder + "/SupraUpdates";

		public static string _DoorModel = "com_plasticcase_enemy";
		public static string _FloorModel = "com_plasticcase_enemy";
		public static string _WallModel = "com_plasticcase_enemy";
		public static string _RampModel = "com_plasticcase_enemy";
		public static string _ElevatorModel = "com_plasticcase_enemy";

		public static string Fx_redcircle = "misc/ui_flagbase_red";
		public static string Fx_goldcircle = "misc/ui_flagbase_gold";
		public static string fx_whitecircle = "misc/ui_flagbase_silver";


		public static string fx_glowstickGlow_green = "misc/glow_stick_glow_green";
		public static string fx_glowstickGlow_red = "misc/glow_stick_glow_red";
		public static string fx_flashbang = "explosions/flashbang";
		public static string fx_fire = "fire/tank_fire_engine";
		public static string fx_radiation = "distortion/distortion_tank_muzzleflash";
		public static string fx_sentryExplode = "explosions/sentry_gun_explosion";
		public static string fx_sentrySmoke = "smoke/car_damage_blacksmoke";
		public static string fx_reflectorSparks = "explosions/sparks_a";
		public static string fx_radioLight = "misc/aircraft_light_cockpit_red";
		public static string fx_mineLaunch = "impacts/bouncing_betty_launch_dirt";
		public static string fx_mineSpin = "dust/bouncing_betty_swirl";
		public static string fx_mineExplode = "explosions/bouncing_betty_explosion";
		public static string fx_empExplode = "explosions/emp_grenade";
		public static string fx_tracer_single = "impacts/exit_tracer";
		public static string fx_tracer_shotgun = "impacts/shotgun_default";
		public static string fx_disappear = "impacts/small_snowhit";
		public static string fx_laser = "isc/claymore_laser";
		public static string fx_ballGlow = "misc/aircraft_light_wingtip_green";
		public static string fx_ballContrail = "misc/light_semtex_geotrail";

		public static Dictionary<string, string> Models = new Dictionary<string, string>()
		{
			{   "tank", "vehicle_m1a1_abrams_dmg"   },
			{   "heli", "vehicle_little_bird_armed" },
			{   "ac130", "vehicle_ac130_coop"    },
			{   "heli2", "vehicle_mi24p_hind_mp" },
			{   "bus", "vehicle_bus_destructible_mp" },
			{   "policecar", "uk_police_estate_destructible" },
			{   "barrel", "com_barrel_benzin"    },
			{   "oxygen", "machinery_oxygen_tank02"  }
		};
	}
}
