using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using StarredSeaMUON.World.Locale;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database.Objects
{


    [PrimaryKey("RoomID")]
    internal class DbRoom
    {
        public int RoomID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public RoomMaterial Material = RoomMaterial.Generic;
        public List<long> MobInstanceIDs { get; set; } = new();
        public List<long> ItemInstanceIDs { get; set; } = new();
        public List<Exit> Exits { get; set; } = new();
        public Metadata Metadata { get; set; } = new();
        public DateTime CreationDate { get; set; } = DateTime.Now;

        //generated
        [NotMapped]
        public List<Exit> VisibleExits { get { return Exits.Where(b => b.IsSecret == false).ToList(); } }
    }


    public class Exit
    {
        public string ExitString { get; set; }
        public bool IsSecret { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }

        public long LinkToID { get; set; }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override bool Equals(object? obj)
        {
            return base.Equals(obj);
        }
    }

    public class ValueConverterListExit : ValueConverter<List<Exit>, string>
    {
        public ValueConverterListExit() : base(
            v => DBTableConversionHelper.Serialize(v),
            v => DBTableConversionHelper.Deserialize<List<Exit>>(v))
        { }
    }
}
