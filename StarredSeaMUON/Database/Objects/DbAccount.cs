using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database.Objects
{
    [PrimaryKey("UserID")]
    [Index(nameof(Username), IsUnique = true)]
    internal class DbAccount
    {
        public int UserID { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string PassHash { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public List<DbPlayerCharacter> Characters { get; set; } = new();
        public int CharacterIndex { get; set; }

        //generated
        public DbPlayerCharacter? CurrentCharacter { get
            {
                if (CharacterIndex > Characters.Count) return null;
                return Characters[CharacterIndex];
            }
        }
    }

    [PrimaryKey("CharacterID")]
    [Index(nameof(CharName), IsUnique = true)]
    internal class DbPlayerCharacter
    {
        public int CharacterID { get; set; }
        public string CharName { get; set; }
        public DateTime CreationDate { get; set; } = DateTime.Now;
        public List<string>? Permissions { get; set; }
        public DbMobInstance? ControllingMob { get; set; } = null;
    }
}
