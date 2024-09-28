using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace StarredSeaMUON
{
    internal class AssetManager
    {
        public static void PlaySoundToPlayer(ClientConnection c, MSPSound sound)
        {
            if(c.capabilities.canMSP)
            {
                MSPUtils.SendMSPSoundTrigger(c, sound.filePath, sound.url);
            }
        }
    }

    internal class MSPSound
    {
        public bool isMusic = false;
        public string filePath = "";
        public static string URLBase = "http://localhost:9878/assets/";
        public string url = "";
        public MSPSound(string filePath)
        {
            this.filePath = filePath;
            this.url = URLBase + filePath;
            if (filePath.ToLower().Contains(".mid"))
                isMusic = true;
        }
    }
}
