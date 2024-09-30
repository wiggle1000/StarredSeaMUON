using StarredSeaMUON.World.Skills;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StarredSeaMUON.World.Languages
{
    internal class LanguageRegistry
    {
        public static List<Language> LANGUAGES = new List<Language>();

        public static void LoadLanguages()
        {
            LANGUAGES.Add(new LanguageCommon());
        }
    }
}
