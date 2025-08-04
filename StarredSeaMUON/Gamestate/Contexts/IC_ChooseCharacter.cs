using StarredSeaMUON.Server;
using StarredSeaMUON.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.Gamestate.Contexts
{
    class IC_ChooseCharacter : InputContext
    {
        UserPrompt characterSelectionPrompt;

        public IC_ChooseCharacter(RemotePlayer player) : base(player)
        {
        }

        public override void OnActivate()
        {
            base.OnActivate();
            characterSelectionPrompt = new UserPrompt(
                "Select a character, or enter ^red;NEW^r; to create a new one.",
                "test",
                "aaaaaaaaaaaa",
                "dwa huidw  huig",
                "among us",
                "weeeee!!!!!"
                ).WithElement("NEW", "(Create a new character)");
            characterSelectionPrompt.DisplayTo(player);
        }
        public override void ProcessPlayerInput(string line)
        {
            if(characterSelectionPrompt.options.ContainsKey(line))
            {
                player.Output("Ok option");
            }
            else
            {
                player.OutputError("Invalid entry: \"" + line + "\". Please try again.");
                characterSelectionPrompt.DisplayTo(player);
            }
        }
    }
}
