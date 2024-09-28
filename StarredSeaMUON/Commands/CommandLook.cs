using Microsoft.Data.Sqlite;
using StarredSeaMUON.Database;
using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Commands
{
    internal class CommandLook : Command
    {
        public CommandLook()
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
        public override void Call(ClientConnection caller, CommandParam[] parameters)
        {
            base.Call(caller, parameters);
            int paramCount = parameters.Count();
            if (paramCount == 0) //look at current room
            {
                using (DBHelper db = new DBHelper())
                {
                    using (SqliteDataReader r = db.GetUserReader(caller.authenticatedID))
                    {
                        if (r.Read())
                        {
                            long roomID = (long)r.GetValue("roomid");
                            caller.visitRoom(roomID);
                        }
                    }
                }
            }
            else //looking at a target
            {
                MessageSender.SendText(caller.writer, "looking at " + parameters[0].value + ". (placeholder response)");
            }
        }
    }
}
