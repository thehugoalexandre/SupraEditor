using Newtonsoft.Json;
using System;
using System.Net;

namespace MapEdit
{
    public class Webhook
    {
        public static string thumbnail_url;

        private Uri _Uri;
        public Webhook(string URL)
        {
            if (!Uri.TryCreate(URL, UriKind.Absolute, out _Uri))
            {
                throw new UriFormatException();
            }
        }

        public static string PostData(string url, WebhookObject data)
        {
            using (WebClient wb = new WebClient())
            {
                wb.Headers.Add(HttpRequestHeader.ContentType, "application/json");
                return wb.UploadString(url, "POST", JsonConvert.SerializeObject(data));
            }
        }

        public static string Thumbnail(string mapname)
        {
            switch (mapname)
            {
                case "mp_dome":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/f/f1/Bare_Load_Screen_Dome_MW3.png";
                    break;
                case "mp_bravo":
                    thumbnail_url = "https://i.imgur.com/hWPeNLU.png";
                    break;
                case "mp_alpha":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/a/a6/Lockdown_loading_screen_MW3.PNG";
                    break;
                case "mp_bootleg":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/0/08/Bare_Load_Screen_Bootleg_MW3.png";
                    break;
                case "mp_carbon":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/c/c8/Bare_Load_Screen_Carbon_MW3.png";
                    break;
                case "mp_exchange":
                    thumbnail_url = "https://i.imgur.com/PDFkfoR.png";
                    break;
                case "mp_harthat":
                    thumbnail_url = "https://i.imgur.com/vS2gOpu.png";
                    break;
                case "mp_radar":
                    thumbnail_url = "https://i.imgur.com/8lqGMRi.png";
                    break;
                case "mp_interchange":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/4/4b/Bare_Load_Screen_Interchange_MW3.png";
                    break;
                case "mp_lambeth":
                    thumbnail_url = "https://i.imgur.com/NV28kez.png";
                    break;
                case "mp_mogadishu":
                    thumbnail_url = "https://i.imgur.com/jS4sy4Z.png";
                    break;
                case "mp_paris":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/a/a7/Iron_Lady_MW3.png";
                    break;
                case "mp_plaza2":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/6/6d/Mall_Interior_Arkaden_MW3.png";
                    break;
                case "mp_seatown":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/a/a7/Bare_Load_Screen_Seatown_MW3.png";
                    break;
                case "mp_underground":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/0/09/Bare_Load_Screen_Underground_MW3.png";
                    break;
                case "mp_village":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/f/f4/Bare_Load_Screen_Village_MW3.png";
                    break;
                case "mp_terminal_cls":
                    thumbnail_url = "https://vignette.wikia.nocookie.net/callofduty/images/6/68/Terminal_Loading_Screen_MW3.png";
                    break;
                case "mp_cement":
                    thumbnail_url = "https://i.imgur.com/aWSiz3s.png";
                    break;
                case "mp_italy":
                    thumbnail_url = "https://i.imgur.com/Pa4j3mR.png";
                    break;
                case "mp_meteora":
                    thumbnail_url = "https://i.imgur.com/hZyDxzq.png";
                    break;
                case "mp_morningwood":
                    thumbnail_url = "https://i.imgur.com/IyI2rf6.png";
                    break;
                case "mp_overwatch":
                    thumbnail_url = "https://i.imgur.com/Q8uafkG.png";
                    break;
                case "mp_park":
                    thumbnail_url = "https://i.imgur.com/z0ml6AX.png";
                    break;
                case "mp_qadeem":
                    thumbnail_url = "https://i.imgur.com/CphWMSl.png";
                    break;
                case "mp_boardwalk":
                    thumbnail_url = "https://i.imgur.com/nHLhijj.png";
                    break;
                case "mp_nola":
                    thumbnail_url = "https://i.imgur.com/YVaLIvl.png";
                    break;
                case "mp_roughneck":
                    thumbnail_url = "https://i.imgur.com/64rGuXs.png";
                    break;
                case "mp_shipbreaker":
                    thumbnail_url = "https://i.imgur.com/Dvl63Qe.png";
                    break;
                case "mp_aground_ss":
                    thumbnail_url = "https://i.imgur.com/RiIGRYl.png";
                    break;
                case "mp_courtyard_ss":
                    thumbnail_url = "https://i.imgur.com/I8E6iYz.png";
                    break;
                case "mp_hillside_ss":
                    thumbnail_url = "https://i.imgur.com/GTfzYQu.png";
                    break;
                case "mp_restrepo_ss":
                    thumbnail_url = "https://i.imgur.com/INh5IRf.png";
                    break;
                case "mp_crosswalk_ss":
                    thumbnail_url = "https://i.imgur.com/KYE1kNy.png";
                    break;
                case "mp_burn_ss":
                    thumbnail_url = "https://i.imgur.com/AlcDzpW.png";
                    break;
                case "mp_six_ss":
                    thumbnail_url = "https://i.imgur.com/v4fXCfP.png";
                    break;
                default:
                    thumbnail_url = "https://cdn0.iconfinder.com/data/icons/flat-design-basic-set-1/24/error-exclamation-512.png";
                    break;
            }
            return thumbnail_url;
        }

    }

    public struct WebhookObject
    {
        public string content;
        public string username;
        public string avatar_url;
        public Embed[] embeds;
    }

    public struct Embed
    {
        public string title;
        public string url;
        public int color;
        public Footer footer;
        public Thumbnail thumbnail;
        public Author author;
        public Field[] fields;
    }

    public struct Field
    {
        public string name;
        public string value;
        public bool inline;
    }

    public struct Footer
    {
        public string text;
        public string icon_url;
    }

    public struct Thumbnail
    {
        public string url;
    }

    public struct Author
    {
        public string name;
        public string url;
        public string icon_url;
    }
}