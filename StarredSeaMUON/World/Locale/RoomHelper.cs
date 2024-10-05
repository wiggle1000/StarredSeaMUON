using Microsoft.Data.Sqlite;
using StarredSeaMUON.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using StarredSeaMUON.Server;
using StarredSeaMUON.Database.Objects;

namespace StarredSeaMUON.World.Locale
{
    internal class RoomHelper
    {
        public static void DisplayToUser(RemotePlayer client, DbRoom room)
        {
            StreamWriter writer = client.telnet.writer;
            TerminalTheme theme = client.options.terminalTheme;
            client.OutputCentered(room.Title, theme.room_title);
            client.Output(" ", theme.normal);
            if (room.Metadata.HasMeta("image"))
            {
                string filename = room.Metadata.GetMeta("image");
                if (filename.ToLower().EndsWith("hurp"))
                    AsciiArtHelper.SendAsciiArt(client, filename);
                else
                    AsciiArtHelper.SendAsciiArt(client, filename);
            }
            client.Output(" ", theme.normal);
            client.Output(room.Description, theme.room_desc);
            client.Output(" ", theme.normal);
            List<Exit> visExits = room.VisibleExits;
            if (visExits.Count == 0)
            {
                client.Output("There are no obvious exits.", theme.room_exits);
            }
            else if (visExits.Count == 1)
            {
                client.OutputCentered("There is one obvious exit:", theme.room_exits);
                foreach (Exit a in visExits)
                {
                    if(a.Name.ToLower() == a.ExitString.ToLower())
                    {
                        client.OutputCentered(a.Name + " - " + a.Description, theme.room_exits);
                    }
                    else
                    {
                        client.OutputCentered(a.Name + " (" + a.ExitString + ") - " + a.Description, theme.room_exits);
                    }
                }
            }
            else
            {
                client.OutputCentered("There are " + visExits.Count + " obvious exits:", theme.room_exits);
                foreach (Exit a in visExits)
                {
                    if (a.Name.ToLower() == a.ExitString.ToLower())
                    {
                        client.OutputCentered(a.Name + " - " + a.Description, theme.room_exits);
                    }
                    else
                    {
                        client.OutputCentered(a.Name + " (" + a.ExitString + ") - " + a.Description, theme.room_exits);
                    }
                }
            }
        }
    }
}
