using Microsoft.Data.Sqlite;
using StarredSeaMUON.Database;
using StarredSeaMUON.Server;
using StarredSeaMUON.World.Locale;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Commands
{
    internal class LuaCommands : Command
    {
        public LuaCommands()
        {
            commandParamLists.Add(new CommandParamType[] { });
            commandParamLists.Add(new CommandParamType[] { CommandParamType.TARGET });
            commandName = "look";
            commandUsage = "look, look <TARGET>";
        }

        public override int DoesNameMatch(string name)
        {
            if (name.ToLower() == "l") return 1;
            if (name.ToLower() == "look") return 4;
            if (name.ToLower().StartsWith("l ")) return 2;
            if (name.ToLower().StartsWith("look ")) return 5;
            return 0;
        }

        public void LookRoom(RemotePlayer caller, long roomID)
        {
            Room room = World.World.TryGetRoom(roomID);
            room.DisplayToUser(caller);
            if (room.hasMeta("music"))
            {
                caller.PlaySound(room.getMetaString("music"));
            }
        }
        public override void Call(RemotePlayer caller, CommandParam[] parameters)
        {
            base.Call(caller, parameters);
            int paramCount = parameters.Count();
            if (paramCount == 0) //look at current room
            {
                using (DBHelper db = new DBHelper())
                {
                    using (SqliteDataReader r = db.GetUserReader(caller.userInfo.ID))
                    {
                        if (r.Read())
                        {
                            long roomID = (long)r.GetValue("roomid");
                            LookRoom(caller, roomID);
                        }
                    }
                }
            }
            else //looking at a target
            {
               caller.Output("looking at " + parameters[0].value + ". (placeholder response)");
            }
        }
    }
}
