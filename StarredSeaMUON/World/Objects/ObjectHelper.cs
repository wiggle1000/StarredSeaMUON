using Microsoft.Data.Sqlite;
using StarredSeaMUON.Database;
using StarredSeaMUON.World.Locale;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Objects
{
    internal class ObjectHelper
    {
        public static WorldObject? loadObjectFromDB(long objID)
        {
            using (DBHelper db = new DBHelper())
            {
                Logger.Log("LOADING OBJECT FROM DB: " + objID);
                using (SqliteDataReader dat = db.GetObjectReader(objID))
                {
                    if (!dat.Read()) return null;
                    WorldObject? o = ObjectClassRegistry.GetNewObj((string)dat.GetValue("class"));
                    if(o == null) { return null; }
                    o.name = (string)dat.GetValue("name");
                    if (!dat.IsDBNull("roomid"))
                        o.roomID = (long)dat.GetValue("roomid");
                    if (!dat.IsDBNull("description"))
                        o.description = (string)dat.GetValue("description");
                    return o;
                }
            }
            return null;
        }
    }
}
