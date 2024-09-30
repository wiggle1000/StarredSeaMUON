using StarredSeaMUON.World.Objects;
using StarredSeaMUON.World.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Languages
{
    internal class Language
    {
        public static Random langRand;
        public Skill speakingSkill;
        public List<string>[] gibbers;

        static Language()
        {
            langRand = new Random(DateTime.Now.Millisecond);
        }

        public Language()
        {
            speakingSkill = new SkillLanguage(this);
            AddGibber("Glibbity");
            AddGibber("Globbity");
            AddGibber("Glibbit");
            AddGibber("Glubbit");
            AddGibber("Glibby");
            AddGibber("Gloppy");
            AddGibber("Gloop");
            AddGibber("Glump");
            AddGibber("Glorp");
            AddGibber("Glarp");
            AddGibber("Glup");
            AddGibber("Glop");
            AddGibber("Glap");
            AddGibber("Gop");
            AddGibber("Gup");
            AddGibber("Go");
            AddGibber("Ga");
        }

        public void AddGibber(string gib)
        {
            int l = gib.Length;
            if (gibbers[l] == null) gibbers[l] = new List<string>();
            gibbers[l].Add(gib);
        }
        public string GetGibber(int wordLength)
        {
            if (gibbers[wordLength] != null) return gibbers[wordLength][langRand.Next(0, gibbers[wordLength].Count)];
            int rGibNum = langRand.Next(0, gibbers.Length);
            return gibbers[rGibNum][langRand.Next(0, gibbers[wordLength].Count)];
        }

        public virtual string GetWordGibberish(string input, float proficiency)
        {
            string gibber = GetGibber(input.Length);
            if(proficiency > 1) //understanding of tone
            {
                string output = "";
                bool isUpper = false;
                for (int i = 0; i < gibber.Length; i++)
                {
                    if (i < input.Length) isUpper = Char.IsAsciiLetterUpper(input[i]);
                    output += isUpper ? Char.ToUpper(gibber[i]) : Char.ToLower(gibber[i]);
                }

                return output;
            }
            else
            {
                return gibber.ToUpper();
            }
        }

        public virtual string TranslateFor(string input, Animal listener)
        {
            float proficiency = listener.GetLangProficiency(this);
            if (proficiency >= 5) return input; //listener has perfect understanding!

            if(input.Contains(' '))
            {
                string output = "";
                string[] words = input.Split(' ');
                for (int i = 0; i < words.Length; i++)
                {
                    if (i != 0) output += ' ';
                    if (input.Length < proficiency * 1.5f) //listener understands word
                        output += words[i];
                    else
                        output += GetWordGibberish(words[i], proficiency);
                }
                return output;
            }
            else
            {
                if (input.Length < proficiency * 1.5f) //listener understands word
                    return input;
                else
                    return GetWordGibberish(input, proficiency);
            }
        }
    }
}
