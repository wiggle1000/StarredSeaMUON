using Microsoft.EntityFrameworkCore;
using StarredSeaMUON.World.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database.Objects
{
    [PrimaryKey("ItemID")]
    internal class DbItemType
    {
        public int ItemID { get; set; }
        public string Name { get; set; }
        public string LookText { get; set; }
        public string Description { get; set; }
        public string Metadata { get; set; }
        public float Condition { get; set; }
        public float Weight { get; set; }
        public float Volume { get; set; }
        //public WearableLocation WearableFlags { get; set; }
    }
    [PrimaryKey("InstanceID")]
    internal class DbItemInstance
    {
        public int InstanceID { get; set; }
        public long ItemTypeID { get; set; }
        public string? DescriptionOverride { get; set; }
        public string? Metadata { get; set; }
        public bool IsInRoom { get; set; }
        public long? CRoomID;
    }
}
