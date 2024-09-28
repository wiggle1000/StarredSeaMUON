using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Commands
{
    internal class CommandParam
    {
        public string value;
        public CommandParamType type;
        public CommandParam(string value, CommandParamType type)
        {
            this.value = value;
            this.type = type;
        }
    }

    public enum CommandParamType
    {
        TARGET = 0,
        PLAYER = 1,
        OBJECT = 2,

        INT = 10,
        DOUBLE = 11,
        STRING = 12
    }
}
