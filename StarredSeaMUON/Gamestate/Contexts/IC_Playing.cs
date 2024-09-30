using StarredSeaMUON.Server;
using StarredSeaMUON.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Gamestate.Contexts
{
    internal class IC_Playing : InputContext
    {
        public IC_Playing(RemotePlayer player) : base(player)
        {
        }

        public override void OnActivate()
        {

        }

        public override void Tick(float dt)
        {
            base.Tick(dt);
        }

        public override void ProcessPlayerInput(string line)
        {
            base.ProcessPlayerInput(line);
            player.Output("hi!");
        }
    }
}
