using StarredSeaMUON.World.Languages;
using StarredSeaMUON.World.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Objects
{
    internal class Animal : WorldObject
    {
        public float height = 1;
        public float maxHealth = 10;
        public float health = 10;
        public Dictionary<Skill, float> skills = new Dictionary<Skill, float>();

        public float GetProficiency(Skill skill)
        {
            if (!skills.ContainsKey(skill)) return 0;
            return skills[skill];
        }
        public float GetLangProficiency(Language lang)
        {
            if (!skills.ContainsKey(lang.speakingSkill)) return 0;
            return skills[lang.speakingSkill];
        }

    }
}
