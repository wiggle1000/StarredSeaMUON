using StarredSeaMUON.World.Languages;
using StarredSeaMUON.World.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Skills
{
    internal class SkillLanguage : Skill
    {
        Language language;

        public SkillLanguage(Language lang)
        {
            this.language = lang;
            this.ProficiencyCap = 5;
        }

        public override string GetProficiencyString(float proficiency)
        {
            if (proficiency < 1) return "None";
            if (proficiency < 2) return "Limited";
            if (proficiency < 3) return "Beginner";
            if (proficiency < 4) return "Intermediate";
            if (proficiency < 5) return "Advanced";
            return "Native";
        }

    }
}
