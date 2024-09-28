using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace StarredSeaMUON.Database
{
    internal class InputVerifiers
    {
        public static string verifyName(string textIn)
        {
            if (textIn.Length > 20) return "Name too long.";
            if (textIn.Length < 1) return "Name too short.";
            if (Regex.IsMatch(textIn, @"[^a-zA-Z0-9\\ \\_\\-\\']")) return "Name contains invalid character. Please only use latin letters, numbers, space, underscore, dash, and apostrophe when naming a character.";
            return ""; //ok
        }

        public static string verifyPassword(string textIn)
        {
            if (textIn.Length > 64) return "Password too long.";
            if (textIn.Length < 8) return "Password too short";
            if (!Regex.IsMatch(textIn, @"[a-z]")) return "Please make sure you include at least one lowercase letter!";
            if (!Regex.IsMatch(textIn, @"[A-Z]")) return "Please make sure you include at least one capital letter!";
            if (!Regex.IsMatch(textIn, @"[0-9]")) return "Please make sure you include at least one number!";
            if (!Regex.IsMatch(textIn, @"[^a-zA-Z0-9]")) return "Please make sure you include at least one special character!";
            return ""; //ok
        }
        public static string verifyEmail(string textIn)
        {
            if (textIn.Length > 128) return "E-Mail too long.";
            if (textIn.Length < 5) return "E-Mail too short";
            if (!Regex.IsMatch(textIn, @"(?:[a-z0-9!#$%&'*+\/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+\/=?^_`{|}~-]+)*|""(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*"")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9]))\.){3}(?:(2(5[0-5]|[0-4][0-9])|1[0-9][0-9]|[1-9]?[0-9])|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])"))
                return "Invalid E-Mail!";
            return ""; //ok
        }
    }
}
