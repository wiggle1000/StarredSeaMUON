using Microsoft.Data.Sqlite;
using StarredSeaMUON.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace StarredSeaMUON.World.Locale
{
    internal class RoomHelper
    {
        public static Room loadRoomFromDB(long roomID)
        {
            using (DBHelper db = new DBHelper())
            {
                Logger.Log("LOADING ROOM FROM DB: " + roomID);
                using (SqliteDataReader dat = db.GetRoomReader(roomID))
                {
                    if (!dat.Read()) return null;
                    Room r = new Room();
                    r.name = (string)dat.GetValue("name");
                    if (!dat.IsDBNull("title"))
                        r.title = (string)dat.GetValue("title");
                    else
                        r.title = (string)dat.GetValue("name");
                    if (!dat.IsDBNull("description"))
                        r.description = (string)dat.GetValue("description");
                    if (!dat.IsDBNull("exits"))
                        r.parseExits((string)dat.GetValue("exits"));
                    if (!dat.IsDBNull("metadata"))
                        r.ParseMeta((string)dat.GetValue("metadata"));
                    return r;
                }
            }
            return null;
        }
    }
}
