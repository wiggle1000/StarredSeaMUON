using StarredSeaMUON.World.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Skills
{
    internal class SkillRegistry
    {
        public static List<Skill> SKILLS = new List<Skill>();

        public static void LoadSkills()
        {
            foreach(Language lang in LanguageRegistry.LANGUAGES)
            {
                SKILLS.Add(lang.speakingSkill);
            }
        }
    }
}
