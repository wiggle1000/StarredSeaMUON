using StarredSeaMUON.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Gamestate.Contexts
{
    internal abstract class InputContext
    {
        internal RemotePlayer player;

        public InputContext(RemotePlayer player)
        {
            this.player = player;
        }

        public virtual void ProcessPlayerInput(string line){ }

        public virtual void Tick(float dt) { }
        public virtual void OnActivate() { }
        public virtual void OnDeactivate() { }

    }
}
