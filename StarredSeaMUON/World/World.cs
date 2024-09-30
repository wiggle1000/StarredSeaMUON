using StarredSeaMUON.Database;
using StarredSeaMUON.Server;
using StarredSeaMUON.World.Locale;
using StarredSeaMUON.World.Objects;
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
        static Dictionary<long, WorldObject> objects = new Dictionary<long, WorldObject>();

        public static Room TryGetRoom(long id)
        {
            if (rooms.ContainsKey(id))
            {
                return rooms[id];
            }
            rooms.Add(id, RoomHelper.loadRoomFromDB(id));
            Logger.Log(rooms[id].ToString());
            return rooms[id];
        }

        public static WorldObject TryGetObject(long id)
        {
            if (objects.ContainsKey(id))
            {
                return objects[id];
            }
            objects.Add(id, ObjectHelper.loadObjectFromDB(id));
            Logger.Log(objects[id].ToString());
            return objects[id];
        }
    }
}
