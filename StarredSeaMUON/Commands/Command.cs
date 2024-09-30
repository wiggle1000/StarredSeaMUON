using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Commands
{
    internal class Command
    {
        public List<CommandParamType[]> commandParamLists = new List<CommandParamType[]>();
        public string commandName = "";
        public string commandUsage = "";

        /// <summary>
        /// returns 0 if false, otherwise the number of characters to strip from the front to get only params
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public virtual int DoesNameMatch(string name)
        {
            return 0;
        }
        public virtual void Call(RemotePlayer caller, CommandParam[] parameters)
        {

        }
    }
}
