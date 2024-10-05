using StarredSeaMUON.Database;
using StarredSeaMUON.Database.Objects;
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

        public void LookRoom(RemotePlayer caller, DbRoom room)
        {
            RoomHelper.DisplayToUser(caller, room);
            if (room.Metadata.HasMeta("music"))
            {
                caller.PlaySound(room.Metadata.GetMeta("music"));
            }
        }
        public override void Call(RemotePlayer caller, CommandParam[] parameters)
        {
            base.Call(caller, parameters);
            int paramCount = parameters.Count();
            if (paramCount == 0) //look at current room
            {
                if (caller.userInfo == null) return;
                if (caller.userInfo.CurrentCharacter == null ) return;
                if (caller.userInfo.CurrentCharacter.ControllingMob == null) return;
                if (caller.userInfo.CurrentCharacter.ControllingMob.CRoomID == null) return;

                long roomID = caller.userInfo.CurrentCharacter.ControllingMob.CRoomID.Value;
                DbRoom? room = caller.db.GetRoomByID(roomID);
                if (room == null) return;
                LookRoom(caller, room);
            }
            else //looking at a target
            {
               caller.Output("looking at " + parameters[0].value + ". (placeholder response)");
            }
        }
    }
}
