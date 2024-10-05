using Microsoft.EntityFrameworkCore;
using StarredSeaMUON.World.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database.Objects
{
    [PrimaryKey("MobID")]
    internal class DbMobType
    {
        public int MobID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BaseHealth { get; set; }
        public string? Metadata { get; set; }
    }
    [PrimaryKey("InstanceID")]
    internal class DbMobInstance
    {
        public int InstanceID { get; set; }
        public long MobTypeID { get; set; }
        public string? DescriptionOverride { get; set; }
        public string? Metadata { get; set; }
        public bool IsInRoom { get; set; }
        public long? CRoomID;
    }
}
