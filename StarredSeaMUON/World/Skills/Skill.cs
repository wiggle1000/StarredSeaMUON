using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Skills
{
    internal class Skill
    {
        public string SkillName = "Unnamed Skill";
        public float ProficiencyCap = 10;

        public virtual string GetProficiencyString(float proficiency)
        {
            if (proficiency < 1)  return "None";
            if (proficiency < 2)  return "I";
            if (proficiency < 3)  return "II";
            if (proficiency < 4)  return "III";
            if (proficiency < 5)  return "IV";
            if (proficiency < 6)  return "V";
            if (proficiency < 7)  return "VI";
            if (proficiency < 8)  return "VII";
            if (proficiency < 9)  return "VIII";
            if (proficiency < 10) return "IX";
            if (proficiency < 11) return "X";
            return "X+";
        }
    }
}
