using StarredSeaMUON.Database;
using StarredSeaMUON.Server;
using StarredSeaMUON.World.Locale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World
{
    internal class World
    {
        static Dictionary<long, Room> rooms = new Dictionary<long, Room>();

        public static Room TryGetRoom(long id)
        {
            if(rooms.ContainsKey(id))
            {
                return rooms[id];
            }
            rooms.Add(id, RoomHelper.loadRoomFromDB(id));
            Logger.Log(rooms[id].ToString());
            return rooms[id];
        }
    }
}
