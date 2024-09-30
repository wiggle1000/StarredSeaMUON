using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Server
{
    class UserEntry
    {
        public bool isAuthenticated = false;
        public long ID = -1;
        public string Name = null;
        public string Email = null;
        public DateTime creationDate;
        public string[] perms = null;
        public long cRoomID = 1;

        public void ParsePerms(string permString)
        {
            if (permString.Length == 0)
            {
                perms = new string[0];
                return;
            }
            if (!permString.Contains(';'))
            {
                perms = new string[] { permString };
                return;
            }
            perms = permString.Split(';');
        }
        public string UnparsePerms()
        {
            if (perms.Length == 0) return "";
            if (perms.Length == 1) return perms[0];

            string output = perms[0];
            for(int i = 1; i < perms.Length; i++)
            {
                output += ";";
                output += perms[i];
            }
            return output;
        }
    }
}
