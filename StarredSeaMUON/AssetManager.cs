using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StarredSeaMUON
{
    internal class AssetManager
    {
        const string resourceRoot = "./resources/web/assets/";

        public static MSPSound SND_NOT_FOUND = new MSPSound("missing.ogg");
        public static Dictionary<string, MSPSound> sounds = new Dictionary<string, MSPSound>();

        public static bool PlaySoundToPlayer(RemotePlayer c, MSPSound sound, int fadein = 0, int fadeout = 0, int vol = 100, int repeats = 1, int priority = 50, bool continueIfPlayedAgain = true)
        {
            if (c.options.mspSupport == MSPSupportType.MSP_ON)
            {
                MSPUtils.SendMSPSoundTrigger(c, sound.filePath, sound.url, vol, repeats, priority, continueIfPlayedAgain);
                return true;
            }
            else if (c.options.mspSupport == MSPSupportType.MSP_OVER_GMCP)
            {
                JsonObject body = new JsonObject();
                body.Add("name", sound.filePath);
                body.Add("url", sound.url);
                body.Add("type", sound.isMusic ? "music" : "sound");
                if (sound.tag != "") body.Add("tag", sound.tag);
                if (vol != 100) body.Add("volume", vol);
                if (fadein != 100) body.Add("fadein", fadein);
                if (fadeout != 100) body.Add("fadeout", fadeout);
                if (repeats != 1) body.Add("loops", repeats);
                if (priority != 50) body.Add("priority", priority);
                if (continueIfPlayedAgain == false) body.Add("continue", false);
                if(sound.isMusic) body.Add("key", "background-music");

                Logger.Log("GENERATED MSP JSON: " + body.ToString());
                c.SendGMCP("Client.Media.Play", body.ToString());
                return true;
            }
            return false;
        }

        internal static MSPSound GetSound(string path)
        {
            if(sounds.ContainsKey(path.ToLower()))
            {
                return sounds[path.ToLower()];
            }
            if (!File.Exists(Path.Combine(resourceRoot, path))) return SND_NOT_FOUND;
            MSPSound newSound = new MSPSound(path);
            sounds.Add(path.ToLower(), newSound);
            return newSound;
        }
    }

    internal class MSPSound
    {
        public bool isMusic = false;
        public string filePath = "";
        public static string URLBase = "http://StarredSea.mooo.com:9878/assets/";
        public string url = "";
        public string tag = "";
        public MSPSound(string filePath)
        {
            this.filePath = filePath;
            this.url = URLBase;
            if (filePath.ToLower().Contains("/music/"))
                isMusic = true;
        }
    }
}
